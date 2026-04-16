using PrimS.Telnet;
using SuchByte.MacroDeck.Logging;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Reflection.Metadata.Ecma335;
using Teamspeak3Plugin.Helper;
using Teamspeak3Plugin.Model;

namespace Teamspeak3Plugin.Services
{
    public class TeamSpeak3Telnet
    {
        private static TeamSpeak3Telnet MInstance;
        public static TeamSpeak3Telnet Instance => MInstance ??= new TeamSpeak3Telnet();

        private Client Client;
        private Teamspeak3PluginMain PluginInstance;

        private readonly object MLock = new object();

        public int ClientId = -1;
            
        public bool IsConnected => Client?.IsConnected ?? false;

        public void SetupTelnetClient(Teamspeak3PluginMain pluginInstance, string apiKey, bool forceConnect = false)
        {
            PluginInstance = pluginInstance;
            if(IsConnected && !forceConnect)
                return;

            try
            {
                lock (MLock) 
                {
                    var socket = new TcpByteStream("127.0.0.1", 25639);
                    Client = new Client(socket, new CancellationToken());
                    if (!Client.IsConnected)
                        throw new SocketException();

                    var welcomeMessage = Client.ReadAsync().Result;
                    if (!welcomeMessage.Contains("TS3 Client"))
                        return;

                    var authResponse = GetTelnetResponse($"auth apikey={apiKey}");
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
                Client = null;
            }
            catch(Exception ex) {
                if (PluginInstance != null )
                    MacroDeckLogger.Error(PluginInstance, $"Failed to setup telnet connection: {ex.Message}");
            }

        }

        public void Dispose()
        {
            Client.Dispose();
            Client = null;
        }

        private bool SelectCurrentServer()
        {
            lock (MLock)
            {
                if (Client == null || !IsConnected){
                    MacroDeckLogger.Warning(PluginInstance, $"Failed to SelectCurrentServer Client not Connected"); 
                    return false;
                }

                var useResponse = GetTelnetResponse("use");
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
                    var whoAmIResponse = GetTelnetResponse("whoami");
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
                if (!IsConnected) return false;

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
                if (!IsConnected) return false;

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
                var channels = new List<TeamspeakChannel>();

                if (!IsConnected) 
                    return channels;

                Client.WriteLineAsync("channellist");
                var channeListResponse = Client.ReadAsync().Result;

                if(!channeListResponse.Contains("msg=ok"))
                    return null;

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
                if (!IsConnected) return false;

                var retries = 0;
                while (retries < 10)
                {
                    var inputMuteStatusIResponse = GetTelnetResponse($"clientvariable clid={clientId} client_input_muted");
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
                if (!IsConnected) return false;

                var retries = 0;
                while (retries < 10)
                {
                    var setInputMuteStatusResponse = GetTelnetResponse($"clientupdate client_input_muted={inputMuteStatus}");
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
                if (!IsConnected) 
                    return false;

                var retries = 0;
                while (retries < 10)
                {
                    var inputMuteStatusIResponse = GetTelnetResponse($"clientvariable clid={clientId} client_output_muted");
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
                if (!IsConnected) return false;

                var retries = 0;
                while (retries < 10)
                {
                    var setInputMuteStatusResponse = GetTelnetResponse($"clientupdate client_output_muted={outputMuteStatus}");
                    if (setInputMuteStatusResponse.Contains("msg=ok")) return true;

                    retries++;
                }

                return false;
            }
        }

        #endregion

        private string GetTelnetResponse(string command)
        {
            lock (MLock)
            {
                if (!IsConnected) 
                    return string.Empty;

                try
                {
                    Client.WriteLineAsync(command);
                    return Client.ReadAsync().Result;

                }
                catch (Exception ex)
                {
                    Client.Dispose();
                    Client = null;
                    return string.Empty;
                }
            }
        }
    }
}
