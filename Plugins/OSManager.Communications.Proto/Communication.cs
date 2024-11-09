using OSManager.Plugins.Intercommunication.Commands;
using OSManager.Plugins.Intercommunication.Enums;
using OSManager.Plugins.Intercommunication.Extensions;
using ProtoBuf;

namespace OSManager.Communications.Proto;

public static class Communication
{
    private static readonly Dictionary<Type, byte> Lookup = new();
    private static readonly Dictionary<byte, Type> ReverseLookup = new();

    static Communication()
    {
        // Can't use 0 as an identifier as it is used as a terminator for communication
        byte count = 1;
        foreach (Command command in Enum.GetValues(typeof(Command)))
        {
            Lookup.Add(command.Type(), count);
            ReverseLookup.Add(count, command.Type());
            count++;
        }
    }
    
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
        // TODO: Modify so that you send the number of bytes for the object followed by a 0 then the type identifier (single byte) followed by the data
        MemoryStream stream = new();
        Serializer.Serialize(stream, obj);

        var data = new byte[stream.Length + 2];
        data[0] = ToByteIdentifier(obj);
        data[^1] = 0;

        Array.Copy(stream.ToArray(), 0, data, 1, stream.Length);

        return data;
    }
    
    public static object Deserialize(byte[] data, out Type type)
    {
        type = data[0].ToType();
        byte[] objData = data[1..];
        MemoryStream stream = new(objData);
        
        // TODO: Figure out a non-manual method
        if (type == typeof(InstallCommand))
        {
            return Serializer.Deserialize<InstallCommand>(stream);
        }
        else if (type == typeof(ResponseCommand))
        {
            return Serializer.Deserialize<ResponseCommand>(stream);
        }
        else if (type == typeof(InitialiseCommand))
        {
            return Serializer.Deserialize<InitialiseCommand>(stream);
        }
        else if (type == typeof(PopStackCommand))
        {
            return Serializer.Deserialize<PopStackCommand>(stream);
        }
        else if (type == typeof(FinaliseCommand))
        {
            return Serializer.Deserialize<FinaliseCommand>(stream);
        }

        throw new NotImplementedException();
    }
    
    public static Type ToType(this byte metadata)
    {
        bool success = ReverseLookup.TryGetValue(metadata, out Type? type);
        if (!success)
        {
            throw new NotImplementedException($"No corresponding type exists for identifier '{metadata}'");
        }

        return type!;
    }

    private static byte ToByteIdentifier<T>(T obj)
    {
        Type test = obj!.GetType();
        bool success = Lookup.TryGetValue(test, out byte output);
        if (!success)
        {
            throw new NotImplementedException($"No corresponding identifier exists for type {obj.GetType()}");
        }

        if (output == 0)
        {
            throw new FormatException("0 is not a valid value for a byte identifier");
        }

        return output;
    }
}