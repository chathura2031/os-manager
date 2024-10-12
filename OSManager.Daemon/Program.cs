using System.IO.Pipes;
using OSManager.Daemon;
using ProtoBuf;

await StartServer(new CancellationToken());
Task.Delay(1000).Wait();

async Task StartServer(CancellationToken stoppingToken)
{
    var person = new Person {
        Id = 12345, Name = "Fred",
        Address = new Address {
            Line1 = "Flat 1",
            Line2 = "The Meadows"
        }
    };

    byte[] bytes;
    string s;
    {
        MemoryStream stream = new();
        Serializer.Serialize(stream, person);
        bytes = stream.ToArray();
        s = System.Text.Encoding.UTF8.GetString(bytes);
    }

    // TODO: Figure out how to serialise and deserialize
    Person person1;
    {
        MemoryStream stream = new(bytes);
        person1 = Serializer.Deserialize<Person>(stream);
    }
    
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