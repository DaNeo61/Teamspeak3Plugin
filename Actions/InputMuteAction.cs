using SuchByte.MacroDeck.ActionButton;
using SuchByte.MacroDeck.Logging;
using SuchByte.MacroDeck.Plugins;
using SuchByte.MacroDeck.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teamspeak3Plugin.Helper;
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

            var outputMuteStatus = Telnet.GetInputMuteStatus(cId);
            var newState = Telnet.SetInputMuteStatus(outputMuteStatus ? 0 : 1);

            SetInputStatusState(newState);
        }

        catch (Exception ex)
        {
            if (Teamspeak3PluginMain.Instance != null)
                MacroDeckLogger.Warning(Teamspeak3PluginMain.Instance, $"Failed to SetOutputMuteStatus: {ex.Message}");
        }
    }
    private void SetInputStatusState(bool state)
    {
        VariableManager.SetValue(ConstantsVars.InputState, state, VariableType.Bool, Teamspeak3PluginMain.Instance, null);
    }
}