namespace OSManager.Daemon.Packages;

public interface IPackage
{
    string Name { get; }
    
    string HumanReadableName { get; }
    
    List<IPackage> Dependencies { get; }
    
    List<IPackage> OptionalExtras { get; }

    public int Install(int stage, string data);
}