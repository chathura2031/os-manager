using System.IO.Pipes;
using OSManager.Daemon;
using OSManager.Daemon.Packages;
using OSManager.Shared;
using OSManager.Shared.Commands;

await StartServer(new CancellationToken());
Task.Delay(1000).Wait();

async Task StartServer(CancellationToken stoppingToken)
{
    NamedPipeServerStream server = new("PipesOfPiece");
    BinaryWriter writer = new(server);
    BinaryReader reader = new(server);
    
    while (true)
    {
        if (!server.IsConnected)
        {
            await server.WaitForConnectionAsync();
        }
        
        object data = Communication.Deserialize(Communication.GetData(reader), out Type type);

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
            if (installCommand.Package == Packages.Discord)
            {
                statusCode = Discord.Instance.Install(installCommand.Stage, installCommand.Data ?? string.Empty);
            }
            else
            {
                throw new NotImplementedException();
            }

            byte[] binary = Communication.Serialize(new ResponseCommand()
            {
                StatusCode = statusCode
            });
            writer.Write(binary);
            writer.Flush();
            
            server.Disconnect();
        }
        else if (data.GetType() == typeof(PopStackCommand))
        {
            var popCommand = (PopStackCommand)data;
            for (int i = 0; i < popCommand.Count; i++)
            {
                Utilities.BashStack.Pop();
            }
            
            byte[] binary = Communication.Serialize(new ResponseCommand()
            {
                StatusCode = 0
            });
            writer.Write(binary);
            writer.Flush();
            
            server.Disconnect();
        }
        else if (data.GetType() == typeof(DisconnectCommand))
        {
            server.Disconnect();
        }
        else
        {
            throw new NotImplementedException();
        }
    }
}