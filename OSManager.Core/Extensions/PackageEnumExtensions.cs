using OSManager.Core.Attributes;

namespace OSManager.Core.Extensions;

public static class PackageEnumExtensions
{
    public static string Name(this Enums.Package package)
    {
        return package.GetAttribute<NameAttribute>().Name;
    }
    
    public static string PrettyName(this Enums.Package package)
    {
        return package.GetAttribute<PrettyNameAttribute>().PrettyName;
    }
}