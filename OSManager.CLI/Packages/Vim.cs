namespace OSManager.CLI.Packages;

public class Vim : IPackage
{
    public static readonly Vim Instance = new();
    
    public string Name { get; } = "Vim";

    public string PathSafeName { get; } = "vim";

    public List<IPackage> Dependencies { get; } = [];

    public List<IPackage> OptionalExtras { get; } = [];
    
    public int Install(int stage, string data)
    {
        throw new NotImplementedException();
    }
}