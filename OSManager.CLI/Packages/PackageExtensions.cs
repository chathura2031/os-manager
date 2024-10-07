namespace OSManager.CLI.Packages;

public static class PackageExtensions
{
    public static int InstallDependencies(this IPackage package)
    {
        int statusCode = PackageDependencies.Instance.Install(0, package.Name);
        return statusCode;
    }
}