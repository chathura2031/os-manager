namespace OSManager.CLI.Packages;

public static class PackageRepository
{
    // Keep track of all packages that are available
    private static HashSet<IPackage> _packages = [
        PackageDependencies.Instance,
        UpdateAndUpgrade.Instance,
        Discord.Instance,
        Chrome.Instance, 
        Vim.Instance 
    ];

    private static readonly Dictionary<string, IPackage> _packageLookup = new();

    static PackageRepository()
    {
        foreach (IPackage package in _packages)
        {
            _packageLookup[package.Name] = package;
        }
    }

    /// <summary>
    /// Get a package from the package name
    /// </summary>
    /// <param name="packageName">The name of the package</param>
    /// <returns>A reference to the package instance</returns>
    /// <exception cref="ArgumentException">If the package name is unknown</exception>
    public static IPackage GetPackage(string packageName)
    {
        if (_packageLookup.TryGetValue(packageName, out IPackage? value))
        {
            return value;
        }
        else
        {
            throw new ArgumentException($"Unknown package '{packageName}'");
        }
    }
}