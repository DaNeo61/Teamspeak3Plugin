using SuchByte.MacroDeck.ActionButton;
using SuchByte.MacroDeck.Logging;
using SuchByte.MacroDeck.Plugins;
using Teamspeak3Plugin.Services;

namespace Teamspeak3Plugin.Actions;
public class InputMuteAction : PluginAction
{
    public override string Name => "Mute/Unmute Input";
    public override string Description => "Mutes or unmutes the teamspeak input";

    private TeamSpeak3Telnet Telnet => Teamspeak3PluginMain.Instance.Telnet;

    public override void Trigger(string clientId, ActionButton actionButton)
    {
        try
        {
            var cId = Telnet.GetClientId();
            if (cId == -1)
                return;

            var inputMuteStatus = Telnet.GetInputMuteStatus(cId);
            var newState = Telnet.SetInputMuteStatus(inputMuteStatus ? 0 : 1);

            Telnet.UpdateInputMuteVariable(newState);
        }

        catch (Exception ex)
        {
            if (Teamspeak3PluginMain.Instance != null)
                MacroDeckLogger.Warning(Teamspeak3PluginMain.Instance, $"Failed to SetOutputMuteStatus: {ex.Message}");
        }
    }
}