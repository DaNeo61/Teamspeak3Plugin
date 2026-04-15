using SuchByte.MacroDeck.GUI;
using SuchByte.MacroDeck.GUI.CustomControls;
using SuchByte.MacroDeck.Logging;
using SuchByte.MacroDeck.Plugins;
using SuchByte.MacroDeck.Variables;
using Teamspeak3Plugin.Actions;
using Teamspeak3Plugin.Helper;
using Teamspeak3Plugin.Services;
using Teamspeak3Plugin.View;

using Timer = System.Timers.Timer;

namespace Teamspeak3Plugin;

public class Teamspeak3PluginMain : MacroDeckPlugin
{
    private ContentSelectorButton StatusButton = new();
    public static Teamspeak3PluginMain? Instance { get; private set; }

    private string QueryKey { get; set; }
    public TeamSpeak3Telnet Telnet { get; private set; }

    private Timer? _refreshTimer;
    private int _disposed;
    private int _shutdownHooksRegistered;

    private MainWindow MacroDeckMainWindow;

    public override bool CanConfigure => true;

    private int RefreshIntervalMs = 1000;

    public Teamspeak3PluginMain()
    {
        SuchByte.MacroDeck.MacroDeck.OnMainWindowLoad += MacroDeckLoaded;
    }

    public override void Enable()
    {
        Instance = this;

        Telnet = new TeamSpeak3Telnet();

        QueryKey = PluginConfiguration.GetValue(this, "ts3_query_api") ?? "";

        RegisterShutdownHooks();

        Actions = new List<PluginAction>
        {
            new InputMuteAction(),
            new OutputMuteAction(),
            new SwitchChannelAction()
        };

        UpdateVariables();

        MacroDeckLogger.Info(this, $"Teamspeak 3 Integration got enabled!");

        _refreshTimer = new Timer(RefreshIntervalMs) { AutoReset = true };
        _refreshTimer.Elapsed += (_, _) => UpdateVariables();
        _refreshTimer.Start();
    }

    private void MacroDeckLoaded(object? sender, EventArgs e)
    {
        MacroDeckMainWindow = sender as MainWindow;

        StatusButton = new ContentSelectorButton();
        StatusButton.BackgroundImageLayout = ImageLayout.Zoom;
        StatusButton.Click += (_, _) => OpenConfigurator();

        MacroDeckMainWindow?.contentButtonPanel.Controls.Add(StatusButton);
        UpdateStatusIcon();
    }

    private void UpdateStatusIcon()
    {

        if (MacroDeckMainWindow != null && !MacroDeckMainWindow.IsDisposed && StatusButton != null && !StatusButton.IsDisposed)
        {
            MacroDeckMainWindow.Invoke(() =>
            {
                StatusButton.BackgroundImage = Telnet.IsConnected ? Properties.Resource.Teamspeak_Online : Properties.Resource.Teamspeak_Offline;
            });
        }
    }

    public void Disable()
    {
        DisposeResources();
    }

    public override void OpenConfigurator()
    {
        using (var pluginConfig = new ViewPluginConfiguration(this))
        {
            pluginConfig.ShowDialog();
        }
    }

    private void RegisterShutdownHooks()
    {
        if (Interlocked.Exchange(ref _shutdownHooksRegistered, 1) == 1) return;

        AppDomain.CurrentDomain.ProcessExit += (_, _) => DisposeResources();
        AppDomain.CurrentDomain.DomainUnload += (_, _) => DisposeResources();
    }

    private void DisposeResources()
    {
        if (Interlocked.Exchange(ref _disposed, 1) == 1) return;

        _refreshTimer?.Stop();
        _refreshTimer?.Dispose();
        _refreshTimer = null;

        Instance = null;
    }
    public void UpdateVariables()
    {
        var inputState = false;
        var outputState = false;
        try
        {
            if (!string.IsNullOrEmpty(QueryKey))
            {
                Telnet.SetupTelnetClient(this, QueryKey);

                var clientId = Telnet.ClientId;
                if (clientId == -1)
                    return;

                inputState = Telnet.GetInputMuteStatus(clientId);
                outputState = Telnet.GetOutputMuteStatus(clientId);
            }

            VariableManager.SetValue(ConstantsVars.InputState, inputState, VariableType.Bool, this, null);
            VariableManager.SetValue(ConstantsVars.OutputState, outputState, VariableType.Bool, this, null);
        }
        catch(Exception ex)
        {
            // None behaviour
        }
    }
}
