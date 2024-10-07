namespace OSManager.CLI.Packages;

public class Vim : IPackage
{
    public static readonly Vim Instance = new();
    
    public string Name { get; } = "vim";

    public string HumanReadableName { get; } = "Vim";

    public List<IPackage> Dependencies { get; } = [];

    public List<IPackage> OptionalExtras { get; } = [];
    
    public int Install(int stage, string data)
    {
        int statusCode = 0;
        switch (stage)
        {
            case 0:
                Utilities.BashStack.PushNextStage(stage + 1, Name);
                this.InstallDependencies();
                break;
            case 1:
                Utilities.BashStack.PushBashCommand("apt install vim", true);
                break;
            default:
                throw new ArgumentException($"{HumanReadableName} does not have {stage} stages of installation.");
        }
        
        return statusCode;
    }
}