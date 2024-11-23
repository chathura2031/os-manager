using OSManager.Core.Enums;
using OSManager.Daemon.Packages;
using OSManager.Plugins.Intercommunication;
using OSManager.Plugins.Intercommunication.Enums;
using OSManager.Plugins.Intercommunication.EventArgs;

namespace OSManager.Daemon;

public class Handler
{
    public Handler(IIntercommServer server)
    {
        server.OnInitialise += OnInitialise;
        server.OnInstall += OnInstall;
        server.OnConfigure += OnConfigure;
        server.OnBackupConfig += OnBackupConfig;
        server.OnStackPop += OnStackPop;
        server.OnStackPush += OnStackPush;
        server.OnFinalise += OnFinalise;
    }
    
    void OnInitialise(object? sender, InitialiseEventArgs initialiseEventArgs)
    {
        Utilities.SlavePath = initialiseEventArgs.SlavePath;
        Utilities.GetOrCreateStacks(initialiseEventArgs.SessionId, true);
    }

    void OnInstall(object? sender, InstallEventArgs installEventArgs)
    {
        IPackage package = PackageRepository.GetPackage(installEventArgs.Package);
        package.Install(installEventArgs.Stage, installEventArgs.Data ?? string.Empty);
    }

    void OnConfigure(object? sender, ConfigureEventArgs configureEventArgs)
    {
        IPackage package = PackageRepository.GetPackage(configureEventArgs.Package);
        package.Configure(configureEventArgs.Stage);
    }

    void OnBackupConfig(object? sender, BackupConfigEventArgs backupConfigEventArgs)
    {
        IPackage package = PackageRepository.GetPackage(backupConfigEventArgs.Package);
        package.BackupConfig(backupConfigEventArgs.Stage);
    }

    void OnStackPop(object? sender, PopStackEventArgs popStackEventArgs)
    {
        for (int i = 0; i < popStackEventArgs.Count; i++)
        {
            if (popStackEventArgs.Stack == StackType.BashStack)
            {
                Utilities.BashStack.Pop();
            }
            else if (popStackEventArgs.Stack == StackType.ProgramStack)
            {
                Utilities.ProgramStack.Pop();
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        popStackEventArgs.StatusCode = 0;
    }

    void OnStackPush(object? sender, PushStackEventArgs pushStackEventArgs)
    {
        Utilities.ProgramStack.Push(pushStackEventArgs.Content);
        pushStackEventArgs.StatusCode = 0;
    }

    void OnFinalise(object? sender, System.EventArgs eventArgs)
    {
        Utilities.DeleteStacks();
    }
}