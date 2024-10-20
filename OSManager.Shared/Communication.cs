using OSManager.Shared.Commands;
using ProtoBuf;

namespace OSManager.Shared;

public static class Communication
{
    public static byte[] GetData(BinaryReader binaryReader)
    {
        List<byte> output = new();
        byte? val;
        
        while ((val = binaryReader.ReadByte()) != 0)
        {
            output.Add((byte)val);
        }

        return output.ToArray();
    }

    public static byte[] Serialize<T>(T obj)
    {
        MemoryStream stream = new();
        Serializer.Serialize(stream, obj);
        
        var data = new byte[stream.Length + 2];
        data[0] = obj.ToByteIdentifier();
        data[^1] = 0;
        
        Array.Copy(stream.ToArray(), 0, data, 1, stream.Length);
        
        return data;
    }

    public static object Deserialize(byte[] data, out Type type)
    {
        type = data[0].ToType();
        byte[] objData = data[1..];
        MemoryStream stream = new(objData);
        
        if (type == typeof(InstallCommand))
        {
            return Serializer.Deserialize<InstallCommand>(stream);
        }
        else if (type == typeof(ResponseCommand))
        {
            return Serializer.Deserialize<ResponseCommand>(stream);
        }

        throw new NotImplementedException();
    }
}