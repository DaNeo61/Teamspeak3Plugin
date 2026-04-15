using PrimS.Telnet;
using SuchByte.MacroDeck.Logging;
using System.Net.Sockets;
using Teamspeak3Plugin.Helper;
using Teamspeak3Plugin.Model;

namespace Teamspeak3Plugin.Services
{
    public class TeamSpeak3Telnet
    {
        private static TeamSpeak3Telnet MInstance;
        public static TeamSpeak3Telnet Instance => MInstance ??= new TeamSpeak3Telnet();

        private Client Client;
        private readonly object MLock = new object();

        public int ClientId = -1;

        private Teamspeak3PluginMain PluginInstance;

        public bool IsConnected => Client?.IsConnected ?? false;

        public void SetupTelnetClient(Teamspeak3PluginMain pluginInstance, string apiKey)
        {
            PluginInstance = pluginInstance;
            if (Client != null && Client.IsConnected)
                return;

            try
            {
                lock (MLock) 
                {
                    Client = new Client("127.0.0.1", 25639, new CancellationToken());

                    if (!Client.IsConnected)
                        return;

                    var welcomeMessage = Client.ReadAsync().Result;
                    if (!welcomeMessage.Contains("TS3 Client"))
                        return;

                    Client.WriteLineAsync($"auth apikey={apiKey}");
                    var authResponse = Client.ReadAsync().Result;
                    if (!authResponse.Contains("msg=ok"))
                    {
                        MacroDeckLogger.Warning(PluginInstance, $"Failed to auth telnet connection: {authResponse}");
                        return;
                    }

                    ClientId = GetClientId();
                }
            }
            catch (SocketException)
            {
                // No Message
            }
            catch(Exception ex) {
                if (PluginInstance != null )
                    MacroDeckLogger.Error(PluginInstance, $"Failed to setup telnet connection: {ex.Message}");
            }

        }

        private bool SelectCurrentServer()
        {
            lock (MLock)
            {
                if (Client == null || !Client.IsConnected){
                    MacroDeckLogger.Warning(PluginInstance, $"Failed to SelectCurrentServer Client not Connected"); 
                    return false;
                }

                Client.WriteLineAsync("use");
                var useResponse = Client.ReadAsync().Result;

                return useResponse.Contains("msg=ok");
            }
        }

        public int GetClientId()
        {
            lock (MLock)
            {
                if (!SelectCurrentServer()) 
                    return -1;

                var retries = 0;
                while (retries < 10)
                {
                    Client.WriteLineAsync("whoami");
                    var whoAmIResponse = Client.ReadAsync().Result;

                    if (whoAmIResponse.Contains("msg=ok"))
                        return int.Parse(whoAmIResponse.Split(new[] { "clid=" }, StringSplitOptions.None)[1].Split(' ')[0].Trim());

                    retries++;
                }
                return -1;
            }
        }

        #region Nickname Stuff

        public bool ChangeNickname(string nickname)
        {
            lock (MLock)
            {
                if (!Client.IsConnected) return false;

                if (string.IsNullOrWhiteSpace(nickname)) return false;

                nickname = nickname.FixTs3SpecificChars();

                Client.WriteLineAsync($"clientupdate client_nickname={nickname}");
                var changeNicknameResponse = Client.ReadAsync().Result;

                return changeNicknameResponse.Contains("msg=ok");
            }
        }

        #endregion

        #region Channel Stuff

        public bool ChannelSwitch(string channelName, int clientId)
        {
            lock (MLock)
            {
                if (!Client.IsConnected) return false;

                if (string.IsNullOrWhiteSpace(channelName)) 
                    return false;

                channelName = channelName.FixTs3SpecificChars();

                var channelList = GetChannelList();

                if (string.IsNullOrWhiteSpace(channelName)) 
                    return false;

                var channel = channelList.FirstOrDefault( o => o.Name == channelName);

                if (channel == null) 
                    return false;

                Client.WriteLineAsync($"clientmove cid={channel.ChannelId} clid={clientId}");
                var channelSwitchResponse = Client.ReadAsync().Result;

                return channelSwitchResponse.Contains("msg=ok");
            }
        }

        public List<TeamspeakChannel> GetChannelList()
        {
            lock (MLock)
            {
                if (!Client.IsConnected) return null;

                Client.WriteLineAsync("channellist");
                var channeListResponse = Client.ReadAsync().Result;

                if(!channeListResponse.Contains("msg=ok"))
                    return null;

                var channels = new List<TeamspeakChannel>();
                foreach (var tempChannel in channeListResponse.Split("|"))
                {
                    var channel = new TeamspeakChannel(tempChannel);
                    if (channel != null) channels.Add(channel);
                }

                return channels;
            }
        }

        #endregion

        #region Input Stuff

        public bool GetInputMuteStatus(int clientId)
        {
            lock (MLock)
            {
                if (!Client.IsConnected) return false;

                var retries = 0;
                while (retries < 10)
                {
                    Client.WriteLineAsync($"clientvariable clid={clientId} client_input_muted");
                    var inputMuteStatusIResponse = Client.ReadAsync().Result;
                    if (inputMuteStatusIResponse.Contains("msg=ok") && int.TryParse(inputMuteStatusIResponse.Split(new[] { "client_input_muted=" }, StringSplitOptions.None)[1].Split('\n')[0].Trim(), out var state))
                    {
                        switch (state)
                        {
                            case -1:
                            case 0:
                                return false;
                            case 1:
                                return true;
                        }
                    }
                    retries++;
                }

                return false;
            }
        }

        public bool SetInputMuteStatus(int inputMuteStatus)
        {
            lock (MLock)
            {
                if (!Client.IsConnected) return false;

                var retries = 0;
                while (retries < 10)
                {
                    Client.WriteLineAsync($"clientupdate client_input_muted={inputMuteStatus}");
                    var setInputMuteStatusResponse = Client.ReadAsync().Result;

                    if (setInputMuteStatusResponse.Contains("msg=ok")) return true;

                    retries++;
                }

                return false;
            }
        }

        #endregion

        #region Output Stuff

        public bool GetOutputMuteStatus(int clientId)
        {
            lock (MLock)
            {
                if (!Client.IsConnected) 
                    return false;

                var retries = 0;
                while (retries < 10)
                {
                    Client.WriteLineAsync($"clientvariable clid={clientId} client_output_muted");
                    var inputMuteStatusIResponse = Client.ReadAsync().Result;

                    if (inputMuteStatusIResponse.Contains("msg=ok") && int.TryParse(inputMuteStatusIResponse.Split(new[] { "client_output_muted=" }, StringSplitOptions.None)[1].Split('\n')[0].Trim(), out var state))
                    {
                        switch (state)
                        {
                            case -1:
                            case 0:
                                return false;
                            case 1:
                                return true;
                        }
                    }
                    retries++;
                }
                return false;
            }
        }

        public bool SetOutputMuteStatus(int outputMuteStatus)
        {
            lock (MLock)
            {
                if (!Client.IsConnected) return false;

                var retries = 0;
                while (retries < 10)
                {
                    Client.WriteLineAsync($"clientupdate client_output_muted={outputMuteStatus}");
                    var setInputMuteStatusResponse = Client.ReadAsync().Result;

                    if (setInputMuteStatusResponse.Contains("msg=ok")) return true;

                    retries++;
                }

                return false;
            }
        }

        #endregion
    }
}
