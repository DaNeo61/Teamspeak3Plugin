using Newtonsoft.Json;
using SuchByte.MacroDeck.ActionButton;
using SuchByte.MacroDeck.GUI;
using SuchByte.MacroDeck.GUI.CustomControls;
using SuchByte.MacroDeck.Plugins;
using System.Xml.Linq;
using Teamspeak3Plugin.Model;
using Teamspeak3Plugin.Services;
using Teamspeak3Plugin.View;
using MacroDeckRoundedComboBox = SuchByte.MacroDeck.GUI.CustomControls.RoundedComboBox;

namespace Teamspeak3Plugin.Actions;
public class SwitchChannelAction : PluginAction
{
    public override string Name => "Switch Channel";
    public override string Description => "Switches to selected Channel";
    public override bool CanConfigure => true;

    public string ChannelToSwitch { get; set; }

    private TeamSpeak3Telnet Telnet => Teamspeak3PluginMain.Instance.Telnet;

    public override void OnActionButtonLoaded()
    {
        var config = JsonConvert.DeserializeObject<PluginActionConfig>(Configuration); ;
        if (config?.Value == null)
            return;

        ChannelToSwitch = config.Value;
        ConfigurationSummary = $"Switch to {config.Value}";
    }

    public override void Trigger(string clientId, ActionButton actionButton)
    {
        Telnet.ChannelSwitch(ChannelToSwitch, Telnet.ClientId);
    }

    public override ActionConfigControl GetActionConfigControl(ActionConfigurator actionConfigurator)
    {
        return new SwitchChannelControlConfiguration(this, actionConfigurator);
    }
}

