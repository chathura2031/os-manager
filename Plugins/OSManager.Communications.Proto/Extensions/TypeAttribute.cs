namespace OSManager.Communications.Proto.Extensions;

public class TypeAttribute : Attribute
{
    public Type CommandType { get; private set; }

    public TypeAttribute(Type commandType)
    {
        CommandType = commandType;
    }
}