using System.IO.Pipes;

var client = new NamedPipeClientStream("PipesOfPiece");
client.Connect(1000);
StreamReader reader = new StreamReader(client);
StreamWriter writer = new StreamWriter(client);

while (true)
{
    string input = Console.ReadLine();
    if (String.IsNullOrEmpty(input)) break;
    writer.WriteLine(input);
    writer.Flush();
    Console.WriteLine(reader.ReadLine());
}

client.Close();