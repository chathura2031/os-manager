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

    // byte[] bytes;
    // string s;
    // {
    //     MemoryStream stream = new();
    //     Serializer.Serialize(stream, person);
    //     bytes = stream.ToArray();
    //     s = System.Text.Encoding.UTF8.GetString(bytes);
    // }

    // // TODO: Figure out how to serialise and deserialize
    // Person person1;
    // {
    //     person1 = Serializer.Deserialize<Person>(new MemoryStream(bytes));
    // }
    
    NamedPipeServerStream server = new("PipesOfPiece");
    await server.WaitForConnectionAsync();
    BinaryWriter binaryWriter = new(server);
    StreamReader reader = new(server);
    // StreamWriter writer = new(server);

    while (!stoppingToken.IsCancellationRequested)
    {
        string? line = reader.ReadLine();
        if (line != null)
        {
            byte[] data = Serialize(person);
            binaryWriter.Write(data);
            binaryWriter.Flush();
            // writer.WriteLine(String.Join("", line.Reverse()));
            // writer.Flush();
        }
    }
}

byte[] Serialize<T>(T obj)
{
    MemoryStream stream = new();
    Serializer.Serialize(stream, obj);
    var data = new byte[stream.Length + 1];
    Array.Copy(stream.ToArray(), data, stream.Length);
    data[^1] = 0;
    
    return data;
}
