using Newtonsoft.Json;
using SuchByte.MacroDeck.GUI;
using SuchByte.MacroDeck.GUI.CustomControls;
using System.Data;
using Teamspeak3Plugin.Actions;
using Teamspeak3Plugin.Model;
using Teamspeak3Plugin.Services;

namespace Teamspeak3Plugin.View
{
    public partial class SwitchChannelControlConfiguration : ActionConfigControl
    {
        private readonly SwitchChannelAction Action;
        private TeamSpeak3Telnet Telnet => Teamspeak3PluginMain.Instance.Telnet;

        public SwitchChannelControlConfiguration(SwitchChannelAction action, ActionConfigurator actionConfigurator)
        {
            InitializeComponent();

            Action = action;

            PopulateChannels();
            LoadConfig();
        }
        private void refreshButton_Click(object sender, EventArgs e) => PopulateChannels();

        private void PopulateChannels()
        {
            var channelList = Telnet.GetChannelList().Select(o => o.Name).ToList() ?? new List<string>();
            AppComboBox.Items.Clear();
            foreach (var app in channelList)
                AppComboBox.Items.Add(app);
        }
        private void LoadConfig()
        {
            if (string.IsNullOrEmpty(Action.Configuration)) return;
            try
            {
                var config = JsonConvert.DeserializeObject<PluginActionConfig>(Action.Configuration);
                if (config != null && !string.IsNullOrEmpty(config.Value))
                {
                    if (!AppComboBox.Items.Contains(config.Value))
                    {
                        AppComboBox.Items.Add(config.Value);
                    }
                    AppComboBox.SelectedItem = config.Value;
                }
            }
            catch
            {
            }
        }
        public override bool OnActionSave()
        {
            var config = new PluginActionConfig { Value = AppComboBox.SelectedItem?.ToString() ?? "" };
            Action.Configuration = JsonConvert.SerializeObject(config);
            Action.ConfigurationSummary = $"Switch to {config.Value}";

            var newAppName = string.IsNullOrWhiteSpace(config.Value) ? null : config.Value;
            if (Action is SwitchChannelAction action)
                action.ChannelToSwitch = newAppName;

            return true;
        }
    }
}
