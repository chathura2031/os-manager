using OSManager.Communications.Proto;
using OSManager.Daemon;
using OSManager.Daemon.Packages;
using OSManager.Plugins.Intercommunication;
using OSManager.Plugins.Intercommunication.Commands;
using OSManager.Plugins.Intercommunication.Enums;
using OSManager.Plugins.Intercommunication.Interfaces;

// using OSManager.Shared;

await StartServer(new CancellationToken());
Task.Delay(1000).Wait();

async Task StartServer(CancellationToken stoppingToken)
{
    IIntercommClient client = new ProtoClient();
    
    // TODO: Move all communication stuff into the proto plugin
    while (true)
    {
        if (!client.IsConnected)
        {
            await client.WaitForClient();
        }

        ICommand data = await client.ReceiveCommand();

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

            await client.SendResponse(statusCode);

            if (installCommand.DisconnectAfter)
            {
                await client.DisconnectFromClient();
            }
        }
        else if (data.GetType() == typeof(PopStackCommand))
        {
            var popCommand = (PopStackCommand)data;
            for (int i = 0; i < popCommand.Count; i++)
            {
                Utilities.BashStack.Pop();
            }

            await client.SendResponse(0);
            
            if (popCommand.DisconnectAfter)
            {
                await client.DisconnectFromClient();
            }
        }
        else if (data.GetType() == typeof(FinaliseCommand))
        {
            Utilities.DeleteStacks();
            await client.DisconnectFromClient();
        }
        else
        {
            throw new NotImplementedException();
        }
    }
}