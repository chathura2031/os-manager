using System.IO.Pipes;

await StartServer(new CancellationToken());
Task.Delay(1000).Wait();

async Task StartServer(CancellationToken stoppingToken)
{
    NamedPipeServerStream server = new("PipesOfPiece");
    await server.WaitForConnectionAsync();
    StreamReader reader = new(server);
    StreamWriter writer = new(server);
    
    while (!stoppingToken.IsCancellationRequested)
    {
        string? line = reader.ReadLine();
        if (line != null)
        {
            writer.WriteLine(String.Join("", line.Reverse()));
            writer.Flush();
        }
    }
}