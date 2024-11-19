using OSManager.Core.Extensions;
using OSManager.Daemon.Packages;

namespace OSManager.Daemon.Extensions;

public static class PackageExtensions
{
    public static int InstallDependencies(this IPackage package)
    {
        int statusCode = PackageDependencies.Instance.Install(1, package.Package.Name());
        return statusCode;
    }
}