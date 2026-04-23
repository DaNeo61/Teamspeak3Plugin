using SuchByte.MacroDeck.GUI;
using SuchByte.MacroDeck.GUI.CustomControls;
using SuchByte.MacroDeck.Logging;
using SuchByte.MacroDeck.Plugins;
using Teamspeak3Plugin.Actions;
using Teamspeak3Plugin.Services;
using Teamspeak3Plugin.View;

using Timer = System.Timers.Timer;

namespace Teamspeak3Plugin;

public class Teamspeak3PluginMain : MacroDeckPlugin
{
    public static Teamspeak3PluginMain Instance;
    private ContentSelectorButton StatusButton = new();

    private string QueryKey { get; set; }
    public TeamSpeak3Telnet Telnet { get; private set; }

    private Timer RefreshTimer;
    private SynchronizationContext Context => SynchronizationContext.Current ?? new SynchronizationContext();

    private int Disposed;
    private int ShutdownHooksRegistered;

    private MainWindow MacroDeckMainWindow;

    public override bool CanConfigure => true;

    private int RefreshIntervalMs = 2000;

    public Teamspeak3PluginMain()
    {
        Instance = this;

        StatusButton = new ContentSelectorButton(); 
        StatusButton.BackgroundImageLayout = ImageLayout.Zoom;
        StatusButton.Click += (_, _) => OpenConfigurator();

        SuchByte.MacroDeck.MacroDeck.OnMainWindowLoad += MacroDeckLoaded;
    }

    public override void Enable()
    {
        Telnet = new TeamSpeak3Telnet().WithPluginInstance(this);
        QueryKey = PluginConfiguration.GetValue(this, "ts3_query_api") ?? "";

        RegisterShutdownHooks();

        Actions = new List<PluginAction>
        {
            new InputMuteAction(),
            new OutputMuteAction(),
            new SwitchChannelAction(),
            new ChangeNicknameAction()
        };

        UpdateVariables();

        MacroDeckLogger.Info(this, $"Teamspeak 3 Integration got enabled!");

        RefreshTimer = new Timer(RefreshIntervalMs) { AutoReset = true };
        RefreshTimer.Elapsed += (_, _) => Context.Post( _ => UpdateVariables(), null);
        RefreshTimer.Start();

    }

    private void CreateAndAddStatusButton()
    {
        if (StatusButton != null && !StatusButton.IsDisposed)
        {
            MacroDeckMainWindow?.contentButtonPanel.Controls.Remove(StatusButton);
            StatusButton.Dispose();
        }

        StatusButton = new ContentSelectorButton();
        StatusButton.BackgroundImageLayout = ImageLayout.Zoom;
        StatusButton.Click += (_, _) => OpenConfigurator();

        MacroDeckMainWindow?.contentButtonPanel.Controls.Add(StatusButton);
        UpdateStatusIcon();
    }

    private void MacroDeckLoaded(object? sender, EventArgs e)
    {
        if (sender is MainWindow window)
        {
            MacroDeckMainWindow = window;
            CreateAndAddStatusButton();
        }
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
        if (Interlocked.Exchange(ref ShutdownHooksRegistered, 1) == 1) return;

        AppDomain.CurrentDomain.ProcessExit += (_, _) => DisposeResources();
        AppDomain.CurrentDomain.DomainUnload += (_, _) => DisposeResources();
    }

    private void DisposeResources()
    {
        if (Interlocked.Exchange(ref Disposed, 1) == 1) return;

        RefreshTimer?.Stop();
        RefreshTimer?.Dispose();
        RefreshTimer = null;
    }
    public void UpdateVariables()
    {
        try
        {
            UpdateStatusIcon();
            if (!string.IsNullOrEmpty(QueryKey))
            {
                Telnet.ConnectClient(QueryKey);

                if (Telnet.ClientId == -1)
                    return;


                Telnet.UpdateInputMuteVariable();
                Telnet.UpdateOutputMuteVariable();
                Telnet.UpdateCurrentChannelVariable();
            }
        }
        catch (Exception ex)
        {
            // None behaviour
        }
    }
}
