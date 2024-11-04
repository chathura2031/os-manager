using System.IO.Pipes;
using OSManager.Communications.Proto;
using OSManager.Core.Commands;
using OSManager.Core.Enums;
using OSManager.Daemon;
using OSManager.Daemon.Packages;
// using OSManager.Shared;

await StartServer(new CancellationToken());
Task.Delay(1000).Wait();

async Task StartServer(CancellationToken stoppingToken)
{
    NamedPipeServerStream server = new("PipesOfPiece");
    BinaryWriter writer = new(server);
    BinaryReader reader = new(server);
    
    // TODO: Figure out why running the bash program more than once breaks it -- for some reason the server sends off too many response commands
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
            if (installCommand.Package == Package.Discord)
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

            if (installCommand.DisconnectAfter)
            {
                server.Disconnect();
            }
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
            
            if (popCommand.DisconnectAfter)
            {
                server.Disconnect();
            }
        }
        else if (data.GetType() == typeof(FinaliseCommand))
        {
            Utilities.DeleteStacks();
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