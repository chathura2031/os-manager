using System.IO.Pipes;
using OSManager.Communications.Proto.Commands;
using OSManager.Core.Enums;
using OSManager.Plugins.Intercommunication;
using OSManager.Plugins.Intercommunication.Enums;

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
        if (_client!.IsConnected)
        {
            return 0;
        }
        
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
            BaseStackPath = sessionId,
            DisconnectAfter = false
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
        int statusCode = ConnectToServer();
        if (statusCode != 0)
        {
            return statusCode;
        }
            
        switch (stage)
        {
            case 0:
                throw new Exception("Stage 0 is not valid");
            case > 1:
            {
                // Remove the bash command that triggered this stage
                statusCode = SendCommandAndAwaitResponse(new PopStackCommand
                {
                    Count = 1,
                    Stack = StackType.BashStack,
                    DisconnectAfter = false
                });
            
                if (statusCode != 0)
                {
                    return statusCode;
                }

                break;
            }
        }

        // Send the install command
        return SendCommandAndAwaitResponse(new InstallCommand
        {
            Package = package,
            Stage = stage,
            Data = data,
            DisconnectAfter = true
        });
    }

    public int Configure(Package package, int stage)
    {
        int statusCode = ConnectToServer();
        if (statusCode != 0)
        {
            return statusCode;
        }

        switch (stage)
        {
            case 0:
                throw new Exception("Stage 0 is not valid");
            case > 1:
            {
                // Remove the bash command that triggered this stage
                statusCode = SendCommandAndAwaitResponse(new PopStackCommand
                {
                    Count = 1,
                    Stack = StackType.BashStack,
                    DisconnectAfter = false
                });
        
                if (statusCode != 0)
                {
                    return statusCode;
                }

                break;
            }
        }
        
        // Send the configure command
        return SendCommandAndAwaitResponse(new ConfigureCommand
        {
            Package = package,
            Stage = stage,
            DisconnectAfter = true
        });
    }

    public int BackupConfig(Package package, int stage)
    {
        int statusCode = ConnectToServer();
        if (statusCode != 0)
        {
            return statusCode;
        }

        switch (stage)
        {
            case 0:
                throw new Exception("Stage 0 is not valid");
            case > 1:
            {
                // Remove the bash command that triggered this stage
                statusCode = SendCommandAndAwaitResponse(new PopStackCommand
                {
                    Count = 1,
                    Stack = StackType.BashStack,
                    DisconnectAfter = false
                });
        
                if (statusCode != 0)
                {
                    return statusCode;
                }

                break;
            }
        }
        
        // Send the configure command
        return SendCommandAndAwaitResponse(new BackupConfigCommand
        {
            Package = package,
            Stage = stage,
            DisconnectAfter = true
        });
    }

    public int PopStack(int count, StackType stack)
    {
        int statusCode = ConnectToServer();
        if (statusCode != 0)
        {
            return statusCode;
        }

        return SendCommandAndAwaitResponse(new PopStackCommand
        {
            Count = count,
            Stack = stack,
            DisconnectAfter = true
        });
    }

    public int PushStack(string[] content)
    {
        int statusCode = ConnectToServer();
        if (statusCode != 0)
        {
            return statusCode;
        }

        return SendCommandAndAwaitResponse(new PushStackCommand()
        {
            Content = content
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