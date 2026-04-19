using SuchByte.MacroDeck.ActionButton;
using SuchByte.MacroDeck.GUI;
using SuchByte.MacroDeck.GUI.CustomControls;
using SuchByte.MacroDeck.Logging;
using SuchByte.MacroDeck.Plugins;
using Teamspeak3Plugin.Model;
using Teamspeak3Plugin.Services;
using Teamspeak3Plugin.View;

namespace Teamspeak3Plugin.Actions;
public class ChangeNicknameAction : PluginAction
{
    public override string Name => "Change Nickname";
    public override string Description => "Changes the client's nickname";
    public override bool CanConfigure => true;

    public string NewNickname { get; set; }

    private TeamSpeak3Telnet Telnet => Teamspeak3PluginMain.Instance.Telnet;

    public override void OnActionButtonLoaded()
    {
        var config = Newtonsoft.Json.JsonConvert.DeserializeObject<PluginActionConfig>(Configuration);
        if (config?.Value == null)
            return;

        NewNickname = config.Value;
        ConfigurationSummary = $"Change nickname to {config.Value}";
    }

    public override ActionConfigControl GetActionConfigControl(ActionConfigurator actionConfigurator)
    {
        return new ChangeNicknameControlConfiguration(this, actionConfigurator);
    }
    
    public override void Trigger(string clientId, ActionButton actionButton)
    {
        try
        {
            var cId = Telnet.GetClientId();
            if (cId == -1)
                return;

            Telnet.ChangeNickname(NewNickname);
        }
        catch (Exception ex)
        {
            if (Teamspeak3PluginMain.Instance != null)
                MacroDeckLogger.Warning(Teamspeak3PluginMain.Instance, $"Failed to ChangeNickname: {ex.Message}");
        }
    }
}
