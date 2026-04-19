using Newtonsoft.Json;
using SuchByte.MacroDeck.GUI;
using SuchByte.MacroDeck.GUI.CustomControls;
using Teamspeak3Plugin.Actions;
using Teamspeak3Plugin.Model;
using Teamspeak3Plugin.Services;

namespace Teamspeak3Plugin.View
{
    public partial class ChangeNicknameControlConfiguration : ActionConfigControl
    {
        private readonly ChangeNicknameAction Action;

        public ChangeNicknameControlConfiguration(ChangeNicknameAction action, ActionConfigurator actionConfigurator)
        {
            Action = action;


            InitializeComponent();

            LoadConfig();
        }
        private void LoadConfig()
        {
            if (string.IsNullOrEmpty(Action.Configuration))
                return;

            try
            {
                var config = JsonConvert.DeserializeObject<PluginActionConfig>(Action.Configuration);
                if (config != null && !string.IsNullOrEmpty(config.Value))
                {
                    nickNameTextBox.Text = config.Value;
                }
            }
            catch
            {
            }
        }
        public override bool OnActionSave()
        {
            var config = new PluginActionConfig { Value = nickNameTextBox.Text ?? "" };
            Action.Configuration = JsonConvert.SerializeObject(config);
            Action.ConfigurationSummary = $"Change Nickname to {config.Value}";

            var newName = string.IsNullOrWhiteSpace(config.Value) ? null : config.Value;
            if (Action is ChangeNicknameAction action)
                action.NewNickname = newName;

            return true;
        }

    }
}
