using System.IO.Pipes;
using OSManager.Communications.Proto.Commands;
using OSManager.Core.Enums;
using OSManager.Plugins.Intercommunication;

namespace OSManager.Communications.Proto;

public class ProtoClient : IIntercommClient
{
    private readonly NamedPipeClientStream? _client;
    private readonly BinaryReader? _reader;
    private readonly BinaryWriter? _writer;

    public ProtoClient(string pipeName)
    {
        _client = new(pipeName);
        _writer = new(_client);
        _reader = new(_client);
    }

    ~ProtoClient()
    {
        Terminate();
    }

    private void Terminate()
    {
        _client?.Dispose();
    }
    
    private int ConnectToServer()
    {
        try
        {
            _client!.Connect(1000);
        }
        catch (Exception e)
        {
            return 1;
        }
        
        return 0;
    }

    public int ConnectToServer(string sessionId, string slavePath)
    {
        int statusCode = ConnectToServer();
        if (statusCode != 0)
        {
            return 1;
        }

        statusCode = SendCommand(new InitialiseCommand()
        {
            SlavePath = slavePath,
            BaseStackPath = sessionId
        });

        return statusCode;
    }

    private int SendCommand(ICommand command)
    {
        byte[] data = Communication.Serialize(command);

        try
        {
            _writer!.Write(data);
            _writer!.Flush();
        }
        catch (Exception e)
        {
            // TODO: Log error somewhere
            return 1;
        }

        return 0;
    }

    private int SendCommandAndAwaitResponse(ICommand command)
    {
        int statusCode = SendCommand(command);
        if (statusCode != 0)
        {
            return statusCode;
        }
        
        statusCode = GetResponse(out ResponseCommand? response);
        return statusCode != 0 ? statusCode : response!.StatusCode;
    }

    // TODO: Implement function to simplify handling the status codes
    public int Install(Package package, int stage, string? data = null)
    {
        if (stage == 0 && !_client!.IsConnected)
        {
            throw new Exception("A connection to the server should already be established but was not.");
        }

        int statusCode = 0;
        if (stage == 0)
        {
            throw new Exception("Stage 0 is not valid");
        }
        else if (stage > 1)
        {
            statusCode = ConnectToServer();
            if (statusCode != 0)
            {
                return statusCode;
            }
            
            // Remove the bash command that triggered this stage
            statusCode = SendCommandAndAwaitResponse(new PopStackCommand
            {
                Count = 1,
                DisconnectAfter = false
            });
            
            if (statusCode != 0)
            {
                return statusCode;
            }
        }

        // Send the install command
        return SendCommandAndAwaitResponse(new InstallCommand()
        {
            Package = package,
            Stage = stage,
            Data = data,
            DisconnectAfter = true
        });
    }

    public int PopStack(int count)
    {
        int statusCode = ConnectToServer();
        if (statusCode != 0)
        {
            return statusCode;
        }

        return SendCommandAndAwaitResponse(new PopStackCommand
        {
            Count = count,
            DisconnectAfter = true
        });
    }

    private int GetResponse(out ResponseCommand? response)
    {
        try
        {
            response = (ResponseCommand)Communication.Deserialize(Communication.GetData((BinaryReader)_reader!),
                out Type type);
        }
        catch (Exception e)
        {
            // TODO: Log the error somewhere
            response = null;
            return 1;
        }

        return 0;
    }
    
    public int DisconnectFromServer()
    {
        int statusCode = ConnectToServer();
        if (statusCode != 0)
        {
            return statusCode;
        }

        statusCode = SendCommand(new FinaliseCommand());
        return statusCode;
    }
}