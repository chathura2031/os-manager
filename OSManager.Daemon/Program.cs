using OSManager.Communications.Proto;
using OSManager.Daemon;
using OSManager.Daemon.Packages;
using OSManager.Plugins.Intercommunication;
using OSManager.Plugins.Intercommunication.Enums;
using OSManager.Plugins.Intercommunication.EventArgs;

void OnInitialise(object? sender, InitialiseEventArgs initialiseEventArgs)
{
    Utilities.SlavePath = initialiseEventArgs.SlavePath;
    Utilities.GetOrCreateStacks(initialiseEventArgs.SessionId, true);
}

void OnInstall(object? sender, InstallEventArgs installEventArgs)
{
    if (installEventArgs.Package == Package.Discord)
    {
        installEventArgs.StatusCode = Discord.Instance.Install(installEventArgs.Stage, installEventArgs.Data ?? string.Empty);
    }
    else
    {
        throw new NotImplementedException();
    }
}

void OnStackPop(object? sender, PopStackEventArgs popStackEventArgs)
{
    for (int i = 0; i < popStackEventArgs.Count; i++)
    {
        Utilities.BashStack.Pop();
    }

    popStackEventArgs.StatusCode = 0;
}

void OnFinalise(object? sender, System.EventArgs eventArgs)
{
    Utilities.DeleteStacks();
}

IIntercommServer server = new ProtoServer();

server.OnInitialise += OnInitialise;
server.OnInstall += OnInstall;
server.OnStackPop += OnStackPop;
server.OnFinalise += OnFinalise;

await server.StartServer();