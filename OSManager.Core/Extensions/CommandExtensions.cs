namespace OSManager.Core.Extensions;

public static class CommandExtensions
{
    public static Type Type(this Enums.Command command)
    {
        return command.GetAttribute<TypeAttribute>().CommandType;
    }
}