using OSManager.Plugins.Intercommunication.Enums;

namespace OSManager.Plugins.Intercommunication.Extensions;

public static class CommandExtensions
{
    public static Type Type(this Command command)
    {
        return command.GetAttribute<TypeAttribute>().CommandType;
    }
}