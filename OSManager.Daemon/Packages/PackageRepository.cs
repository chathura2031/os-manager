using OSManager.Core.Enums;
using OSManager.Core.Extensions;

namespace OSManager.Daemon.Packages;

public static class PackageRepository
{
    // Keep track of all packages that are available
    private static readonly HashSet<IPackage> Packages = [
        PackageDependencies.Instance, 
        UpdateAndUpgrade.Instance,
        Discord.Instance,
        Vim.Instance,
        Chrome.Instance, 
    ];

    private static readonly Dictionary<Package, IPackage> PackageLookup = new();
    
    private static readonly Dictionary<string, IPackage> PackageNameLookup = new();

    static PackageRepository()
    {
        if (Packages.Count != Enum.GetNames(typeof(Package)).Length)
        {
            throw new MissingFieldException("The number of packages in the repository does not match the number of elements in the `Package` enum");
        }
        
        foreach (IPackage package in Packages)
        {
            PackageNameLookup[package.Package.Name()] = package;
            PackageLookup[package.Package] = package;
        }
    }

    /// <summary>
    /// Get a package from the package name
    /// </summary>
    /// <param name="package">An enum of the package</param>
    /// <returns>A reference to the package instance</returns>
    /// <exception cref="ArgumentException">If the package name is unknown</exception>
    public static IPackage GetPackage(Package package)
    {
        if (PackageLookup.TryGetValue(package, out IPackage? value))
        {
            return value;
        }
        else
        {
            throw new ArgumentException($"Unknown package '{package.ToString()}'");
        }
    }

    public static IPackage GetPackage(string packageName)
    {
        if (PackageNameLookup.TryGetValue(packageName, out IPackage? value))
        {
            return value;
        }
        else
        {
            throw new ArgumentException($"Unknown package '{packageName}'");
        }
    }
}