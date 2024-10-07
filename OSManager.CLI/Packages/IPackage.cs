namespace OSManager.CLI.Packages;

public interface IPackage
{
    string Name { get; }
    
    string PathSafeName { get; }
    
    List<IPackage> Dependencies { get; }
    
    List<IPackage> OptionalExtras { get; }

    public int Install(int stage, string data);
}