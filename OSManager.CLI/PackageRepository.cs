using OSManager.CLI.Packages;

namespace OSManager.CLI;

public static class PackageRepository
{
    private static HashSet<IPackage> _packages = [
        Discord.Instance
    ];

    private static readonly Dictionary<string, IPackage> _packageLookup = new();

    static PackageRepository()
    {
        foreach (IPackage package in _packages)
        {
            _packageLookup[package.PathSafeName] = package;
        }
    }

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