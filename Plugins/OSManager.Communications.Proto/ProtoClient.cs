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

    // TODO: Implement function to simplify handling the status codes
    public int Install(Package package, int stage, string? data = null)
    {
        int statusCode;
        if (stage == 0 && !_client!.IsConnected)
        {
            throw new Exception("A connection to the server should already be established but was not.");
        }

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
            statusCode = SendCommand(new PopStackCommand { Count = 1, DisconnectAfter = false });
            if (statusCode != 0)
            {
                return statusCode;
            }
            
            statusCode = GetResponse(out ResponseCommand? response);
            if (statusCode != 0)
            {
                return statusCode;
            }
        }

        statusCode = SendCommand(new InstallCommand()
        {
            Package = package,
            Stage = stage,
            Data = data,
            DisconnectAfter = true
        });
        
        if (statusCode != 0)
        {
            return statusCode;
        }
        
        statusCode = GetResponse(out ResponseCommand? response1);
        
        return statusCode != 0 ? statusCode : response1!.StatusCode;
    }

    public int PopStack(int count)
    {
        int statusCode = ConnectToServer();
        if (statusCode != 0)
        {
            return statusCode;
        }

        statusCode = SendCommand(new PopStackCommand { Count = count, DisconnectAfter = true });
        if (statusCode != 0)
        {
            return statusCode;
        }
        
        statusCode = GetResponse(out ResponseCommand? response);
        
        return statusCode != 0 ? statusCode : response!.StatusCode;
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