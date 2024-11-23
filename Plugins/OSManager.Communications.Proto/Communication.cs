using OSManager.Communications.Proto.Enums;
using OSManager.Communications.Proto.Extensions;
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
    
    /// <summary>
    /// Read the next object sent through the binary stream as a byte array
    /// </summary>
    /// <param name="binaryReader">The binary stream to read from</param>
    /// <returns>A byte array of the next object</returns>
    public static byte[] GetData(BinaryReader binaryReader)
    {
        // Allocate enough space for the metadata
        byte metadataSize = sizeof(long) + 1;
        List<byte> output = new(metadataSize);
        byte? val;
        
        // Read in the metadata
        for (int i = 0; i < metadataSize; i++)
        {
            output.Add(binaryReader.ReadByte());
        }

        // Read in the object
        long objSize = BitConverter.ToInt64(output.Take(metadataSize - 1).ToArray());
        for (int i = 0; i < objSize; i++)
        {
            output.Add(binaryReader.ReadByte());
        }

        return output.ToArray();
    }

    /// <summary>
    /// Serialize a given object to bytes. Once serialized, the first 8 bytes (size of long) will denote the size of the
    /// object, the byte after this will denote the type of the object and the last n bytes will be the object converted
    /// to bytes
    /// </summary>
    /// <param name="obj">The object to serialize</param>
    /// <typeparam name="T">The type of object to serialize</typeparam>
    /// <returns>A byte array of serialized data</returns>
    /// <exception cref="Exception">Thrown if the size of the first chunk is incorrect</exception>
    public static byte[] Serialize<T>(T obj)
    {
        MemoryStream stream = new();
        Serializer.Serialize(stream, obj);
        byte[] objSize = BitConverter.GetBytes(stream.Length);
        if (objSize.Length != sizeof(long))
        {
            throw new Exception("The number of bytes required to store the object size is different to the size of a long");
        }
        
        var data = new byte[objSize.Length + 1 + stream.Length];
        
        // Copy the object size
        Array.Copy(objSize, data, objSize.Length);
        // Add the byte identifier
        data[objSize.Length] = ToByteIdentifier(obj);
        // Copy the object
        Array.Copy(stream.ToArray(), 0, data, objSize.Length + 1, stream.Length);
        
        return data;
    }
    
    /// <summary>
    /// Deserialize a given byte array by reversing the serialization encoding
    /// </summary>
    /// <param name="data">The byte array to deserialize</param>
    /// <param name="type">The type of the returned object</param>
    /// <returns>The deserialized object</returns>
    public static object Deserialize(byte[] data, out Type type)
    {
        type = data[sizeof(long)].ToType();
        byte[] objData = data[(sizeof(long) + 1)..];
        MemoryStream stream = new(objData);

        return Serializer.Deserialize(type, stream);
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