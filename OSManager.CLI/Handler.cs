using System.Reflection;
using OSManager.CLI.CliOptions;
using OSManager.Core.Enums;
using OSManager.Core.Extensions;
using OSManager.Plugins.Intercommunication;
using OSManager.Plugins.Intercommunication.Enums;

namespace OSManager.CLI;

public class Handler(IIntercommClient client)
{
    public int Initialise(InitialiseOptions options)
    {
        AssemblyName assembly = Assembly.GetEntryAssembly()!.GetName();
        Console.WriteLine($"Version {assembly.Version}");

        // TODO: Handle status code
        int statusCode = client.ConnectToServer(options.BaseStackPath, options.SlavePath);
        if (statusCode != 0)
        {
            throw new NotImplementedException();
        }
        
        // TODO: Add ability for the user to select what to do here
        int selection = 0;
        if (selection == 0)
        {
            statusCode = client.Install(Package.Discord, 1);
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
        foreach (Package package in Enum.GetValues(typeof(Package)))
        {
            if (options.Package == package.Name())
            {
                return client.Install(package, options.Stage, options.DataPath);
            }
        }
        
        throw new NotImplementedException();
    }

    public int PopStack(PopStackOptions options)
    {
        StackType stack = options.Stack switch
        {
            "bash" => StackType.BashStack,
            "program" => StackType.ProgramStack,
            _ => throw new NotImplementedException()
        };

        return client.PopStack(1, stack);
    }

    public int PushStack(PushStackOptions options)
    {
        string[] content;
        if (options.FilePath != null)
        {
            content = File.ReadAllLines(options.FilePath);
        }
        else if (options.Content != null)
        {
            content = [options.Content];
        }
        else
        {
            throw new Exception("Either content or a file path must be provided");
        }
        
        return client.PushStack(content);
    }

    public int Finalise(FinaliseOptions options)
    {
        return client.DisconnectFromServer();
    }
}