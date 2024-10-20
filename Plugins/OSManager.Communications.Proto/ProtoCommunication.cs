using System.IO.Pipes;
using OSManager.Core;

namespace OSManager.Communications.Proto;

public class ProtoCommunication(string pipeName) : ICommunication
{
    private readonly NamedPipeClientStream? _client = new(pipeName);

    ~ProtoCommunication()
    {
        Terminate();
    }

    private void Terminate()
    {
        _client?.Dispose();
    }
    
    public int ConnectToServer(out BinaryReader? reader, out BinaryWriter? writer)
    {
        try
        {
            _client!.Connect(1000);
        }
        catch (Exception e)
        {
            reader = null;
            writer = null;
            return 1;
        }
        
        writer = new(_client);
        reader = new(_client);
        
        return 0;
    }
}