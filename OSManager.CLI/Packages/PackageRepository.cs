namespace OSManager.CLI.Packages;

public static class PackageRepository
{
    // Keep track of all packages that are available
    private static HashSet<IPackage> _packages = [
        PackageDependencies.Instance,
        Discord.Instance,
        Chrome.Instance, 
        Vim.Instance 
    ];

    private static readonly Dictionary<string, IPackage> _packageLookup = new();

    static PackageRepository()
    {
        foreach (IPackage package in _packages)
        {
            _packageLookup[package.PathSafeName] = package;
        }
    }

    /// <summary>
    /// Get a package from the package name
    /// </summary>
    /// <param name="pathSafeName">The name of the package</param>
    /// <returns>A reference to the package instance</returns>
    /// <exception cref="ArgumentException">If the package name is unknown</exception>
    public static IPackage GetPackage(string pathSafeName)
    {
        if (_packageLookup.TryGetValue(pathSafeName, out IPackage? value))
        {
            return value;
        }
        else
        {
            throw new ArgumentException($"Unknown package '{pathSafeName}'");
        }
    }
}