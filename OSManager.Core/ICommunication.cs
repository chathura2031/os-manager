namespace OSManager.Core;

public interface ICommunication
{
    public int ConnectToServer(out BinaryReader? reader, out BinaryWriter? writer);
}