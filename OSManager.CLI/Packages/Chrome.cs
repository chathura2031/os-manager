namespace OSManager.CLI.Packages;

public class Chrome : IPackage
{
    public static readonly Chrome Instance = new();

    public string Name { get; } = "Chrome";

    public string PathSafeName { get; } = "chrome";

    public List<IPackage> Dependencies { get; } = [];
    
    public List<IPackage> OptionalExtras { get; } = [];
    
    public int Install(int stage, string data)
    {
        throw new NotImplementedException();
    }
}