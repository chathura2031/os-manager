using System.Reflection;
using OSManager.CLI.CliOptions;
using OSManager.Plugins.Intercommunication;
using OSManager.Plugins.Intercommunication.Enums;

namespace OSManager.CLI;

public class Stuff(IIntercommClient client)
{
    // TODO: Use dependency injection
    private readonly IIntercommClient _client = client;
    
    public int Initialise(InitialiseOptions options)
    {
        AssemblyName assembly = Assembly.GetEntryAssembly()!.GetName();
        Console.WriteLine($"Version {assembly.Version}");

        // TODO: Handle status code
        int statusCode = _client.ConnectToServer(options.BaseStackPath, options.SlavePath);
        if (statusCode != 0)
        {
            throw new NotImplementedException();
        }
        
        // TODO: Add ability for the user to select what to do here
        int selection = 0;
        if (selection == 0)
        {
            statusCode = _client.Install(Package.Discord, 1);
            return statusCode;
        }
        else
        {
            throw new ArgumentException("Received an invalid selection");
        }

        return statusCode;
    }
    
    public int GotoStep(ContinueOptions options)
    {
        // TODO: Use some lookup or something idk
        int statusCode = 0;
        if (options.Package == "discord")
        {
            statusCode = _client.Install(Package.Discord, options.Stage, options.DataPath);
        }
        else
        {
            throw new NotImplementedException();
        }

        return statusCode;
    }

    public int PopStack(PopStackOptions options)
    {
        return _client.PopStack(1);
    }

    public int Finalise(FinaliseOptions options)
    {
        return _client.DisconnectFromServer();
    }
}