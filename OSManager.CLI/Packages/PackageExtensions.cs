namespace OSManager.CLI.Packages;

public static class PackageExtensions
{
    public static int InstallDependencies(this IPackage package)
    {
        // TODO: Check if the package is installed and if not, install the dependency
        // TODO: Figure out a way to read this -- might need to add a new option??
        Utilities.BashStack.PushPackageExistsCommand(Discord.Instance.PathSafeName);
        return 0;
    }
}