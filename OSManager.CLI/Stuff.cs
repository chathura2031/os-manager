using System.Reflection;
using OSManager.CLI.CliOptions;
using OSManager.Plugins.Intercommunication;
using OSManager.Plugins.Intercommunication.Commands;
using OSManager.Plugins.Intercommunication.Enums;

namespace OSManager.CLI;

public class Stuff(IIntercommServer server)
{
    // TODO: Use dependency injection
    private readonly IIntercommServer _server = server;
    
    public int Initialise(InitialiseOptions options)
    {
        AssemblyName assembly = Assembly.GetEntryAssembly()!.GetName();
        Console.WriteLine($"Version {assembly.Version}");

        int statusCode = _server.ConnectToServer();
        // TODO: Handle status code
        if (statusCode != 0)
        {
            throw new NotImplementedException();
        }

        statusCode = _server.SendCommand(new InitialiseCommand
        {
            BaseStackPath = options.BaseStackPath,
            SlavePath = options.SlavePath
        });
        // TODO: Handle status code
        if (statusCode != 0)
        {
            throw new NotImplementedException();
        }
        
        // TODO: Add ability for the user to select what to do here
        int selection = 0;
        switch (selection)
        {
            case 0:
                // TODO: Handle status code
                _server.SendCommand(new InstallCommand
                {
                    Package = Package.Discord,
                    Stage = 1,
                    DisconnectAfter = true
                });
        
                break;
            default:
                throw new ArgumentException("Received an invalid selection");
        }

        statusCode = _server.GetResponse(out IResponseCommand? response);
        // TODO: Handle the status code
        if (statusCode != 0)
        {
            throw new NotImplementedException();
        }
        
        return response!.StatusCode;
    }
    
    public int GotoStep(ContinueOptions options)
    {
        int statusCode = _server.ConnectToServer();
        // TODO: Handle status code
        if (statusCode != 0)
        {
            throw new NotImplementedException();
        }
        
        // TODO: Find a way to always have a response command after each send command
        // TODO: Handle status code
        statusCode = _server.SendCommand(new PopStackCommand { Count = 1, DisconnectAfter = false });
        // TODO: Handle the status code
        if (statusCode != 0)
        {
            throw new NotImplementedException();
        }
        
        statusCode = _server.GetResponse(out IResponseCommand? response);
        // TODO: Handle the status code
        if (statusCode != 0)
        {
            throw new NotImplementedException();
        }
        
        // TODO: Use some lookup or something idk
        if (options.Package == "discord")
        {
            // TODO: Handle status code
            _server.SendCommand(new InstallCommand
            {
                Package = Package.Discord,
                Stage = options.Stage,
                Data = options.DataPath,
                DisconnectAfter = true
            });
        }
        else
        {
            throw new NotImplementedException();
        }
        
        statusCode = _server.GetResponse(out IResponseCommand? response1);
        // TODO: Handle the status code
        if (statusCode != 0)
        {
            throw new NotImplementedException();
        }

        return response1!.StatusCode;
    }

    public int PopStack(PopStackOptions options)
    {
        int statusCode = _server.ConnectToServer();
        // TODO: Handle status code
        if (statusCode != 0)
        {
            throw new NotImplementedException();
        }

        // TODO: Handle status code
        _server.SendCommand(new PopStackCommand { Count = 1, DisconnectAfter = true });
        
        statusCode = _server.GetResponse(out IResponseCommand? response);
        // TODO: Handle the status code
        if (statusCode != 0)
        {
            throw new NotImplementedException();
        }
        
        return response!.StatusCode;
    }

    public int PushStack(PushStackOptions options)
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

    public int Finalise(FinaliseOptions options)
    {
        int statusCode = _server.ConnectToServer();
        // TODO: Handle status code
        if (statusCode != 0)
        {
            throw new NotImplementedException();
        }

        // TODO: Handle status code
        statusCode = _server.SendCommand(new FinaliseCommand());
        // TODO: Handle the status code
        if (statusCode != 0)
        {
            throw new NotImplementedException();
        }
        
        return 0;
    }
}