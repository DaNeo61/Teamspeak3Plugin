using SuchByte.MacroDeck.ActionButton;
using SuchByte.MacroDeck.Logging;
using SuchByte.MacroDeck.Plugins;
using SuchByte.MacroDeck.Variables;
using Teamspeak3Plugin.Helper;
using Teamspeak3Plugin.Services;


namespace Teamspeak3Plugin.Actions;

public class OutputMuteAction : PluginAction
{
    public override string Name => "Mute/Unmute Output";
    public override string Description => "Mutes or unmutes the teamspeak output";

    private TeamSpeak3Telnet Telnet => Teamspeak3PluginMain.Instance.Telnet;

    public override void Trigger(string clientId, ActionButton actionButton)
    {
        try
        {
            var cId = Telnet.GetClientId();
            if (cId == -1)
                return;

            var outputMuteStatus = Telnet.GetOutputMuteStatus(cId);
            var newState = Telnet.SetOutputMuteStatus(outputMuteStatus ? 0 : 1);

            SetOutputStatusState(newState);
        }

        catch (Exception ex)
        {
            if (Teamspeak3PluginMain.Instance != null)
                MacroDeckLogger.Warning(Teamspeak3PluginMain.Instance, $"Failed to SetOutputMuteStatus: {ex.Message}");
        }
    }
    private void SetOutputStatusState(bool state)
    {
        VariableManager.SetValue(ConstantsVars.OutputState, state, VariableType.Bool, Teamspeak3PluginMain.Instance, null);
    }
}