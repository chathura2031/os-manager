using System.IO.Pipes;
using OSManager.Plugins.Intercommunication;
using OSManager.Plugins.Intercommunication.Commands;

namespace OSManager.Communications.Proto;

public class ProtoClient : IIntercommClient
{
    private readonly NamedPipeServerStream _server;
    private readonly BinaryWriter _writer;
    private readonly BinaryReader _reader;

    public bool IsConnected => _server.IsConnected;

    public ProtoClient()
    {
        _server = new("PipesOfPiece");
        _writer = new(_server);
        _reader = new(_server);
    }

    ~ProtoClient()
    {
        Terminate();
    }

    private void Terminate()
    {
        _server.Disconnect();
    }

    public async Task WaitForClient()
    {
        await _server.WaitForConnectionAsync();
    }

    public Task<ICommand> ReceiveCommand()
    {
        byte[] data = Communication.GetData(_reader);
        return Task.FromResult((ICommand)Communication.Deserialize(data, out Type type));
    }

    public Task SendResponse(int statusCode)
    {
        byte[] binary = Communication.Serialize(new ResponseCommand()
        {
            StatusCode = statusCode
        });
        
        _writer.Write(binary);
        _writer.Flush();
        
        return Task.CompletedTask;
    }

    public Task DisconnectFromClient()
    {
        _server.Disconnect();
        
        return Task.CompletedTask;
    }
}