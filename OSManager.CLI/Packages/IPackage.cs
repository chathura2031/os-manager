namespace OSManager.CLI.Packages;

public interface IPackage
{
    string Name { get; }
    
    string PathSafeName { get; }
    
    HashSet<IPackage> Dependencies { get; }
    
    HashSet<IPackage> OptionalExtras { get; }

    public int Install(int stage, string data);
}