using Newtonsoft.Json;
using SuchByte.MacroDeck.GUI;
using SuchByte.MacroDeck.GUI.CustomControls;
using SuchByte.MacroDeck.Plugins;
using Teamspeak3Plugin.Model;

namespace Teamspeak3Plugin.View
{
    public partial class ViewPluginConfiguration : DialogForm
    {
        private Teamspeak3PluginMain Plugin { get; set;}

        public ViewPluginConfiguration(Teamspeak3PluginMain plugin)
        {
            Plugin = plugin;
            InitializeComponent();
            LoadConfig();
        }

        private void LoadConfig()
        {
            QueryInputTextBox.Text = PluginConfiguration.GetValue(Plugin, "ts3_query_api") ?? "";
            RefreshRateInput.Text = PluginConfiguration.GetValue(Plugin, "ts3_refresh_ms") ?? "1000";
        }


        private void SaveButtonClick(object sender, EventArgs e)
        {
            PluginConfiguration.SetValue(Plugin, "ts3_query_api", QueryInputTextBox.Text);
            PluginConfiguration.SetValue(Plugin, "ts3_refresh_ms", RefreshRateInput.Text);
            Close();
        }
    }
}
