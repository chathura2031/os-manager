using System.IO.Pipes;
using OSManager.Shared;
using OSManager.Shared.Commands;
using ProtoBuf;

await StartServer(new CancellationToken());
Task.Delay(1000).Wait();

async Task StartServer(CancellationToken stoppingToken)
{
    NamedPipeServerStream server = new("PipesOfPiece");
    await server.WaitForConnectionAsync();
    BinaryWriter writer = new(server);
    BinaryReader reader = new(server);

    // while (!stoppingToken.IsCancellationRequested)
    // {
        object data = Communication.Deserialize(Communication.GetData(reader), out Type type);
        
        ResponseCommand response = new()
        {
            Command = "sudo apt upgrade",
            StatusCode = 0
        };
        
        byte[] binary = Communication.Serialize(response);
        writer.Write(binary);
        writer.Flush();
    // }
}