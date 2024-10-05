namespace OSManager.CLI.Packages;

public interface IPackage
{
    string Name { get; }
    
    string PathSafeName { get; }
    
    HashSet<IPackage> Dependencies { get; }
    
    HashSet<IPackage> OptionalExtras { get; }

    public void Install(int stage, string data);
}

public static class PackageExtensions
{
    public static void InstallDependencies(this IPackage package)
    {
        // TODO: Check if the package is installed and if not, install the dependency
    }
}