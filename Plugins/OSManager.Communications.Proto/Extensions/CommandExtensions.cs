using OSManager.Communications.Proto.Enums;

namespace OSManager.Communications.Proto.Extensions;

public static class CommandExtensions
{
    public static Type Type(this Command command)
    {
        return command.GetAttribute<TypeAttribute>().CommandType;
    }
}