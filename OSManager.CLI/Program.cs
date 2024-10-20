using System.IO.Pipes;
using OSManager.Shared;
using OSManager.Shared.Commands;

var client = new NamedPipeClientStream("PipesOfPiece");
client.Connect(1000);
BinaryReader reader = new(client);
BinaryWriter writer = new(client);

InstallCommand discord = new()
{
    Package = Packages.Discord,
    Stage = 1
};

byte[] data = Communication.Serialize(discord);
writer.Write(data);
writer.Flush();

object response = Communication.Deserialize(Communication.GetData(reader), out Type type);

client.Close();