using OSManager.Plugins.Intercommunication.EventArgs;

namespace OSManager.Plugins.Intercommunication;

public interface IIntercommServer
{
    public event EventHandler<InitialiseEventArgs> OnInitialise;
    public event EventHandler<PopStackEventArgs> OnStackPop;
    public event EventHandler<PushStackEventArgs> OnStackPush;
    public event EventHandler OnFinalise;
    public event EventHandler<InstallEventArgs> OnInstall;
    public event EventHandler<ConfigureEventArgs> OnConfigure;
    public event EventHandler<BackupConfigEventArgs> OnBackupConfig;

    public Task StartServer();
}