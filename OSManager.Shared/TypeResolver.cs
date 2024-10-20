using OSManager.Shared.Commands;

namespace OSManager.Shared;

public static class TypeResolver
{
    private static readonly Dictionary<Type, byte> _lookup = new()
    {
        { typeof(InstallCommand), 1 },
        { typeof(ResponseCommand), 2 },
        { typeof(InitialiseCommand), 3 },
        { typeof(PopStackCommand), 4 },
        { typeof(DisconnectCommand), 5 }
    };
    private static readonly Dictionary<byte, Type> _reverseLookup;

    static TypeResolver()
    {
        _reverseLookup = _lookup.ToDictionary(x => x.Value, x => x.Key);
    }
        
    public static Type ToType(this byte metadata)
    {
        bool success = _reverseLookup.TryGetValue(metadata, out Type? type);
        if (!success)
        {
            throw new NotImplementedException($"No corresponding type exists for identifier '{metadata}'");
        }

        return type!;
    }

    public static byte ToByteIdentifier<T>(this T obj)
    {
        bool success = _lookup.TryGetValue(obj.GetType(), out byte output);
        if (!success)
        {
            throw new NotImplementedException($"No corresponding identifier exists for type {obj.GetType()}");
        }
        else if (output == 0)
        {
            throw new FormatException("0 is not a valid value for a byte identifier");
        }
        
        return output;
    }
}