namespace OSManager.CLI.Packages;

public class Vim : IPackage
{
    public static readonly Vim Instance = new();
    
    public string Name { get; } = "vim";

    public string HumanReadableName { get; } = "Vim";

    public List<IPackage> Dependencies { get; } = [];

    public List<IPackage> OptionalExtras { get; } = [];
    
    public int Install(int stage, string dependencyName)
    {
        throw new NotImplementedException();
    }
}