using System.IO.Pipes;
using System.Reflection;
using CommandLine;
using OSManager.CLI;
using OSManager.CLI.CliOptions;
using OSManager.Shared;
using OSManager.Shared.Commands;


return CommandLine.Parser.Default.ParseArguments<InitialiseOptions, ContinueOptions, PushStackOptions, PopStackOptions>(args)
    .MapResult(
        (InitialiseOptions opts) => Initialise(opts),
        (ContinueOptions opts) => GotoStep(opts),
        (PushStackOptions opts) => PushStack(opts),
        (PopStackOptions opts) => PopStack(opts),
        errs => 1);

int Initialise(InitialiseOptions options)
{
    AssemblyName assembly = Assembly.GetEntryAssembly()!.GetName();
    Console.WriteLine($"Version {assembly.Version}");

    var client = new NamedPipeClientStream("PipesOfPiece");
    client.Connect(1000);
    BinaryWriter writer = new(client);
    BinaryReader reader = new(client);

    byte[] data = Communication.Serialize(new InitialiseCommand()
    {
        BaseStackPath = options.BaseStackPath,
        SlavePath = options.SlavePath
    });
    writer.Write(data);
    writer.Flush();
    
    // TODO: Add ability for the user to select what to do here
    int selection = 0;
    switch (selection)
    {
        case 0:
            data = Communication.Serialize(new InstallCommand()
            {
                Package = Packages.Discord,
                Stage = 1
            });
            writer.Write(data);
            writer.Flush();
    
            break;
        default:
            throw new ArgumentException("Received an invalid selection");
    }
    
    var response = (ResponseCommand)Communication.Deserialize(Communication.GetData(reader), out Type type);
    
    client.Close();
    client.Dispose();
    
    return response.StatusCode;
}

int GotoStep(ContinueOptions options)
{
    // TODO: Pop stack without closing connection on server side
    var client = new NamedPipeClientStream("PipesOfPiece");
    client.Connect(1000);
    BinaryWriter writer = new(client);
    BinaryReader reader = new(client);
    byte[] data;
    
    // TODO: Use some lookup or something idk
    if (options.Package == "discord")
    {
        data = Communication.Serialize(new InstallCommand()
        {
            Package = Packages.Discord,
            Stage = options.Stage,
            Data = options.DataPath
        });
        writer.Write(data);
        writer.Flush();
    }
    else
    {
        throw new NotImplementedException();
    }
    
    var response = (ResponseCommand)Communication.Deserialize(Communication.GetData(reader), out Type type);

    client.Close();
    client.Dispose();
    
    return response.StatusCode;
}

int PopStack(PopStackOptions options)
{
    var client = new NamedPipeClientStream("PipesOfPiece");
    client.Connect(1000);
    BinaryWriter writer = new(client);
    BinaryReader reader = new(client);

    byte[] data = Communication.Serialize(new PopStackCommand());
    writer.Write(data);
    writer.Flush();
    
    var response = (ResponseCommand)Communication.Deserialize(Communication.GetData(reader), out Type type);
    
    client.Close();
    client.Dispose();
    
    return response.StatusCode;
}

int PushStack(PushStackOptions options)
{
    // Utilities.GetOrCreateStacks(options.BaseStackPath);
    // string tmpNodePath = $"{Utilities.ProgramStack.Path}.tmp";
    // string content = File.ReadAllText(tmpNodePath);
    // Utilities.ProgramStack.Push(content);
    // File.Delete(tmpNodePath);
    //
    // return 0;
    throw new NotImplementedException();
}