using PrimS.Telnet;
using SuchByte.MacroDeck.Logging;
using SuchByte.MacroDeck.Variables;
using System.Net.Sockets;
using Teamspeak3Plugin.Helper;
using Teamspeak3Plugin.Model;

namespace Teamspeak3Plugin.Services
{
    public class TeamSpeak3Telnet : IDisposable
    {
        private static TeamSpeak3Telnet MInstance;
        public static TeamSpeak3Telnet Instance => MInstance ??= new TeamSpeak3Telnet();

        private Client Client;
        private Teamspeak3PluginMain PluginInstance;

        private readonly object MLock = new object();
        private const int CommandTimeoutMs = 5000;
        private const int RetryCount = 10;

        public int ClientId = -1;

        public bool IsConnected => Client?.IsConnected ?? false;

        public TeamSpeak3Telnet WithPluginInstance(Teamspeak3PluginMain pluginInstance)
        {
            PluginInstance = pluginInstance;
            return this;
        }

        public void ConnectClient(string apiKey, bool forceConnect = false)
        {
            if (IsConnected && !forceConnect)
                return;

            lock (MLock)
            {
                try
                {
                    if (IsConnected && !forceConnect)
                        return;

                    var socket = new TcpByteStream("127.0.0.1", 25639);
                    Client = new Client(socket, new CancellationToken());
                    if (!Client.IsConnected)
                        throw new SocketException();

                    var welcomeMessage = ReadWithTimeout();
                    if (string.IsNullOrEmpty(welcomeMessage) || !welcomeMessage.Contains("TS3 Client"))
                        return;

                    var authResponse = GetTelnetResponse($"auth apikey={apiKey}");
                    if (!authResponse.Contains("msg=ok"))
                    {
                        MacroDeckLogger.Warning(PluginInstance, $"Failed to auth telnet client: {authResponse}");
                        return;
                    }

                    ClientId = GetClientIdInternal();
                }
                catch (SocketException)
                {
                    DisconnectInternal();
                }
                catch (Exception ex)
                {
                    MacroDeckLogger.Error(PluginInstance, $"Critical error in ConnectClient: {ex.Message}");
                }
            }
        }

        public void Dispose()
        {
            lock (MLock)
            {
                DisconnectInternal();
            }
        }

        private void DisconnectInternal()
        {
            if (Client != null)
            {
                try
                {
                    Client.Dispose();
                }
                catch (Exception ex)
                {
                    MacroDeckLogger.Warning(PluginInstance, $"Error disposing client: {ex.Message}");
                }
                finally
                {
                    Client = null;
                }
            }
        }

        private bool SelectCurrentServer()
        {
            if (Client == null || !Client.IsConnected)
            {
                MacroDeckLogger.Warning(PluginInstance, "Failed to SelectCurrentServer Client not Connected"); 
                return false;
            }

            var useResponse = GetTelnetResponse("use");
            return useResponse.Contains("msg=ok");
        }

        public int GetClientId()
        {
            lock (MLock)
            {
                return GetClientIdInternal();
            }
        }

        private int GetClientIdInternal()
        {
            if (!SelectCurrentServer()) 
                return -1;

            for (int retries = 0; retries < RetryCount; retries++)
            {
                var whoAmIResponse = GetTelnetResponse("whoami");
                if (whoAmIResponse.Contains("msg=ok") && TryParseKeyIntValue(whoAmIResponse, "clid=", out var clientId))
                    return clientId;
            }
            return -1;
        }

        #region Nickname Stuff
        public bool ChangeNickname(string nickname)
        {
            lock (MLock)
            {
                if (!IsConnected) 
                    return false;

                if (string.IsNullOrWhiteSpace(nickname)) 
                    return false;

                nickname = nickname.FixTs3SpecificChars();

                try
                {
                    var changeNicknameResponse = GetTelnetResponse($"clientupdate client_nickname={nickname}");
                    return changeNicknameResponse.Contains("msg=ok");
                }
                catch (Exception ex)
                {
                    MacroDeckLogger.Warning(PluginInstance, $"Error changing nickname: {ex.Message}");
                    return false;
                }
            }
        }

        #endregion

        #region Channel Stuff
        public bool ChannelSwitch(string channelName, int clientId)
        {
            lock (MLock)
            {
                if (!IsConnected) return false;
                if (string.IsNullOrWhiteSpace(channelName)) return false;

                channelName = channelName.FixTs3SpecificChars();

                try
                {
                    var channelList = GetChannelListInternal();
                    if (channelList == null || channelList.Count == 0)
                        return false;

                    var channel = channelList.FirstOrDefault(o => o.Name == channelName);
                    if (channel == null) 
                        return false;

                    var channelSwitchResponse = GetTelnetResponse($"clientmove cid={channel.ChannelId} clid={clientId}");
                    return channelSwitchResponse.Contains("msg=ok");
                }
                catch (Exception ex)
                {
                    MacroDeckLogger.Warning(PluginInstance, $"Error switching channel: {ex.Message}");
                    return false;
                }
            }
        }

        public List<TeamspeakChannel> GetChannelList()
        {
            lock (MLock)
            {
                return GetChannelListInternal();
            }
        }

        private List<TeamspeakChannel> GetChannelListInternal()
        {
            var channels = new List<TeamspeakChannel>();

            if (!IsConnected)
                return channels;

            try
            {
                var channelListResponse = GetTelnetResponse("channellist");
                if (string.IsNullOrEmpty(channelListResponse) || !channelListResponse.Contains("msg=ok"))
                    return channels;

                foreach (var tempChannel in channelListResponse.Split("|"))
                {
                    if (string.IsNullOrWhiteSpace(tempChannel))
                        continue;

                    var channel = new TeamspeakChannel(tempChannel);
                    if (channel != null) 
                        channels.Add(channel);
                }

                return channels;
            }
            catch (Exception ex)
            {
                MacroDeckLogger.Warning(PluginInstance, $"Error getting channel list: {ex.Message}");
                return channels;
            }
        }

        public string GetCurrentChannel(int clientId)
        {
            lock (MLock)
            {
                if (!IsConnected) return string.Empty;
                for (int retries = 0; retries < RetryCount; retries++)
                {
                    try
                    {
                        var whoAmIResponse = GetTelnetResponse($"whoami");
                        if (whoAmIResponse.Contains("msg=ok") && TryParseKeyIntValue(whoAmIResponse, "cid=", out var channelId))
                        {
                            var channelList = GetChannelListInternal();
                            var channel = channelList.FirstOrDefault(c => c.ChannelId == channelId.ToString());
                            return channel?.Name ?? string.Empty;
                        }
                    }
                    catch (Exception ex)
                    {
                        MacroDeckLogger.Warning(PluginInstance, $"Error getting current channel: {ex.Message}");
                    }
                }
                return string.Empty;
            }
        }
        public void UpdateCurrentChannelVariable(string channel = null)
        {
            if (ClientId == -1)
                return;

            channel ??= GetCurrentChannel(ClientId);

            VariableManager.SetValue(ConstantsVars.CurrentChannelName, channel, VariableType.String, PluginInstance, null);
        }
        #endregion

        #region Input Stuff
        public bool GetInputMuteStatus(int clientId)
        {
            lock (MLock)
            {
                if (!IsConnected) return false;

                for (int retries = 0; retries < RetryCount; retries++)
                {
                    try
                    {
                        var inputMuteStatusResponse = GetTelnetResponse($"clientvariable clid={clientId} client_input_muted");
                        if (inputMuteStatusResponse.Contains("msg=ok") && TryParseKeyIntValue(inputMuteStatusResponse, "client_input_muted=", out var state))
                            return state == 1;
                    }
                    catch (Exception ex)
                    {
                        MacroDeckLogger.Warning(PluginInstance, $"Error getting input mute status: {ex.Message}");
                    }
                }
                return false;
            }
        }

        public bool SetInputMuteStatus(int inputMuteStatus)
        {
            lock (MLock)
            {
                if (!IsConnected) return false;

                for (int retries = 0; retries < RetryCount; retries++)
                {
                    try
                    {
                        var setInputMuteStatusResponse = GetTelnetResponse($"clientupdate client_input_muted={inputMuteStatus}");
                        if (setInputMuteStatusResponse.Contains("msg=ok")) 
                            return true;
                    }
                    catch (Exception ex)
                    {
                        MacroDeckLogger.Warning(PluginInstance, $"Error setting input mute status: {ex.Message}");
                    }
                }

                return false;
            }
        }

        public void UpdateInputMuteVariable(bool? state = null)
        {
            if (ClientId == -1) 
                return;

            state ??= GetInputMuteStatus(ClientId);

            VariableManager.SetValue(ConstantsVars.InputState, state, VariableType.Bool, PluginInstance, null);
        }
        #endregion

        #region Output Stuff
        public bool GetOutputMuteStatus(int clientId)
        {
            lock (MLock)
            {
                if (!IsConnected) 
                    return false;

                for (int retries = 0; retries < RetryCount; retries++)
                {
                    try
                    {
                        var outputMuteStatusResponse = GetTelnetResponse($"clientvariable clid={clientId} client_output_muted");
                        if (outputMuteStatusResponse.Contains("msg=ok") && TryParseKeyIntValue(outputMuteStatusResponse, "client_output_muted=", out var state))
                            return state == 1;
                    }
                    catch (Exception ex)
                    {
                        MacroDeckLogger.Warning(PluginInstance, $"Error getting output mute status: {ex.Message}");
                    }
                }
                
                return false;
            }
        }

        public bool SetOutputMuteStatus(int outputMuteStatus)
        {
            lock (MLock)
            {
                if (!IsConnected) return false;

                for (int retries = 0; retries < RetryCount; retries++)
                {
                    try
                    {
                        var setOutputMuteStatusResponse = GetTelnetResponse($"clientupdate client_output_muted={outputMuteStatus}");
                        if (setOutputMuteStatusResponse.Contains("msg=ok")) 
                            return true;
                    }
                    catch (Exception ex)
                    {
                        MacroDeckLogger.Warning(PluginInstance, $"Error setting output mute status: {ex.Message}");
                    }
                }

                return false;
            }
        }
        public void UpdateOutputMuteVariable(bool? state = null)
        {
            if (ClientId == -1)
                return;

            state ??= GetOutputMuteStatus(ClientId);

            VariableManager.SetValue(ConstantsVars.OutputState, state, VariableType.Bool, PluginInstance, null);
        }
        #endregion

        private string GetTelnetResponse(string command)
        {
            if (!IsConnected) 
                return string.Empty;

            try
            {
                WriteLineWithTimeout(command);
                return ReadWithTimeout();
            }
            catch (Exception ex)
            {
                DisconnectInternal();
                return string.Empty;
            }
        }

        private void WriteLineWithTimeout(string line)
        {
            if (Client == null)
                throw new InvalidOperationException("Client not initialized");

            var task = Client.WriteLineAsync(line);
            if (!task.Wait(CommandTimeoutMs))
                throw new TimeoutException($"Write command timed out: {line}");
        }

        private string ReadWithTimeout()
        {
            if (Client == null)
                throw new InvalidOperationException("Client not initialized");

            var task = Client.ReadAsync();
            if (!task.Wait(CommandTimeoutMs))
                throw new TimeoutException("Read operation timed out");

            return task.Result ?? string.Empty;
        }

        private bool TryParseKeyIntValue(string response, string key, out int state)
        {
            state = -1;
            try
            {
                var parts = response.Split(new[] { key }, StringSplitOptions.None);
                if (parts.Length > 1)
                {
                    var valuePart = parts[1].Split('\n', ' ')[0].Trim();
                    return int.TryParse(valuePart, out state);
                }
            }
            catch (Exception ex)
            {
                MacroDeckLogger.Warning(PluginInstance, $"Error parsing mute status: {ex.Message}");
            }
            return false;
        }
        private bool TryParseKeyStringValue(string response, string key, out string value)
        {
            value = string.Empty;
            try
            {
                var parts = response.Split(new[] { key }, StringSplitOptions.None);
                if (parts.Length > 1)
                {
                    value = parts[1].Split('\n', ' ')[0].Trim();
                    return true;
                }
            }
            catch (Exception ex)
            {
                MacroDeckLogger.Warning(PluginInstance, $"Error parsing mute status: {ex.Message}");
            }
            return false;
        }
    }
}
