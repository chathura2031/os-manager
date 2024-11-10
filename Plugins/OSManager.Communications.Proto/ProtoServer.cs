using System.IO.Pipes;
using OSManager.Communications.Proto.Commands;
using OSManager.Plugins.Intercommunication;
using OSManager.Plugins.Intercommunication.EventArgs;

namespace OSManager.Communications.Proto;

public class ProtoServer : IIntercommServer
{
    private readonly NamedPipeServerStream _server;
    private readonly BinaryWriter _writer;
    private readonly BinaryReader _reader;

    public event EventHandler<InitialiseEventArgs>? OnInitialise;
    public event EventHandler<PopStackEventArgs>? OnStackPop;
    public event EventHandler? OnFinalise;
    public event EventHandler<InstallEventArgs>? OnInstall;

    public bool IsConnected => _server.IsConnected;

    public ProtoServer()
    {
        _server = new("PipesOfPiece");
        _writer = new(_server);
        _reader = new(_server);
    }

    ~ProtoServer()
    {
        Terminate();
    }

    private void Terminate()
    {
        _server.Disconnect();
    }

    private async Task WaitForClient()
    {
        await _server.WaitForConnectionAsync();
    }

    private ICommand AwaitCommand()
    {
        byte[] data = Communication.GetData(_reader);
        return (ICommand)Communication.Deserialize(data, out Type type);
    }

    private void SendResponse(int statusCode)
    {
        byte[] binary = Communication.Serialize(new ResponseCommand()
        {
            StatusCode = statusCode
        });
        
        _writer.Write(binary);
        _writer.Flush();
    }

    private void DisconnectFromClient()
    {
        _server.Disconnect();
    }

    public async Task StartServer()
    {
        while (true)
        {
            if (!_server.IsConnected)
            {
                await WaitForClient();
            }

            ICommand data = AwaitCommand();

            if (data.GetType() == typeof(InitialiseCommand))
            {
                var initialiseCommand = (InitialiseCommand)data;
                
                if (OnInitialise != null)
                {
                    InitialiseEventArgs e = new()
                    {
                        SlavePath = initialiseCommand.SlavePath,
                        SessionId = initialiseCommand.BaseStackPath
                    };
                    OnInitialise.Invoke(this, e);
                }
            }
            else if (data.GetType() == typeof(InstallCommand))
            {
                var installCommand = (InstallCommand)data;
                
                if (OnInstall != null)
                {
                    InstallEventArgs e = new()
                    {
                        Package = installCommand.Package,
                        Stage = installCommand.Stage,
                        Data = installCommand.Data
                    };

                    OnInstall.Invoke(this, e);
                    SendResponse(e.StatusCode);
                }
                
                if (installCommand.DisconnectAfter)
                {
                    DisconnectFromClient();
                }
            }
            else if (data.GetType() == typeof(PopStackCommand))
            {
                var popCommand = (PopStackCommand)data;
                if (OnStackPop != null)
                {
                    PopStackEventArgs e = new()
                    {
                        Count = popCommand.Count
                    };
                    
                    OnStackPop.Invoke(this, e);
                    SendResponse(e.StatusCode);
                }
                
                if (popCommand.DisconnectAfter)
                {
                    DisconnectFromClient();
                }
            }
            else if (data.GetType() == typeof(FinaliseCommand))
            {
                OnFinalise?.Invoke(this, System.EventArgs.Empty);
                DisconnectFromClient();
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}