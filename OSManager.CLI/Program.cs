using System.IO.Pipes;
using OSManager.CLI;
using ProtoBuf;

var client = new NamedPipeClientStream("PipesOfPiece");
client.Connect(1000);
BinaryReader binaryReader = new(client);
StreamReader reader = new StreamReader(client);
StreamWriter writer = new StreamWriter(client);

while (true)
{
    string input = Console.ReadLine();
    if (String.IsNullOrEmpty(input)) break;
    writer.WriteLine(input);
    writer.Flush();

    MemoryStream stream = new(GetData());
    Person person = Serializer.Deserialize<Person>(stream);
}

client.Close();

byte[] GetData()
{
    List<byte> output = new();
    byte? val;
    
    while ((val = binaryReader.ReadByte()) != 0)
    {
        output.Add((byte)val);
    }

    return output.ToArray();
}
