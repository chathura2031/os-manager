using OSManager.Communications.Proto;
using OSManager.Daemon;
using OSManager.Daemon.Packages;
using OSManager.Plugins.Intercommunication;
using OSManager.Plugins.Intercommunication.Commands;
using OSManager.Plugins.Intercommunication.Enums;

await StartServer(new CancellationToken());
Task.Delay(1000).Wait();

async Task StartServer(CancellationToken stoppingToken)
{
    IIntercommServer server = new ProtoServer();
    
    // TODO: Move all communication stuff into the proto plugin
    while (true)
    {
        if (!server.IsConnected)
        {
            await server.WaitForClient();
        }

        ICommand data = await server.ReceiveCommand();

        if (data.GetType() == typeof(InitialiseCommand))
        {
            var initialiseCommand = (InitialiseCommand)data;
            Utilities.SlavePath = initialiseCommand.SlavePath;
            Utilities.GetOrCreateStacks(initialiseCommand.BaseStackPath, true);
        }
        else if (data.GetType() == typeof(InstallCommand))
        {
            var installCommand = (InstallCommand)data;
            int statusCode;
            if (installCommand.Package == Package.Discord)
            {
                statusCode = Discord.Instance.Install(installCommand.Stage, installCommand.Data ?? string.Empty);
            }
            else
            {
                throw new NotImplementedException();
            }

            await server.SendResponse(statusCode);

            if (installCommand.DisconnectAfter)
            {
                await server.DisconnectFromClient();
            }
        }
        else if (data.GetType() == typeof(PopStackCommand))
        {
            var popCommand = (PopStackCommand)data;
            for (int i = 0; i < popCommand.Count; i++)
            {
                Utilities.BashStack.Pop();
            }

            await server.SendResponse(0);
            
            if (popCommand.DisconnectAfter)
            {
                await server.DisconnectFromClient();
            }
        }
        else if (data.GetType() == typeof(FinaliseCommand))
        {
            Utilities.DeleteStacks();
            await server.DisconnectFromClient();
        }
        else
        {
            throw new NotImplementedException();
        }
    }
}