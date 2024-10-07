namespace OSManager.CLI.Packages;

public class Chrome : IPackage
{
    public static readonly Chrome Instance = new();

    public string Name { get; } = "chrome";

    public string HumanReadableName { get; } = "Chrome";

    public List<IPackage> Dependencies { get; } = [];
    
    public List<IPackage> OptionalExtras { get; } = [];
    
    public int Install(int stage, string dependencyName)
    {
        throw new NotImplementedException();
    }
}