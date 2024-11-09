using System.IO.Pipes;
using OSManager.Plugins.Intercommunication;
using OSManager.Plugins.Intercommunication.Commands;

namespace OSManager.Communications.Proto;

public class ProtoServer : IIntercommServer
{
    private readonly NamedPipeClientStream? _client;
    private readonly BinaryReader? _reader;
    private readonly BinaryWriter? _writer;

    public ProtoServer(string pipeName)
    {
        _client = new(pipeName);
        _writer = new(_client);
        _reader = new(_client);
    }

    ~ProtoServer()
    {
        Terminate();
    }

    private void Terminate()
    {
        _client?.Dispose();
    }
    
    public int ConnectToServer()
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

    public int SendCommand(ICommand command)
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

    public int GetResponse(out IResponseCommand? response)
    {
        try
        {
            response = (IResponseCommand)Communication.Deserialize(Communication.GetData((BinaryReader)_reader!),
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
}