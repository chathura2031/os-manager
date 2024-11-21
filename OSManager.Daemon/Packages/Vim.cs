using OSManager.Core.Enums;
using OSManager.Core.Extensions;
using OSManager.Daemon.Extensions;

namespace OSManager.Daemon.Packages;

public class Vim : IPackage
{
    public static readonly Vim Instance = new();

    public Package Package { get; } = Package.Vim;

    public List<IPackage> Dependencies { get; } = [];

    public List<IPackage> OptionalExtras { get; } = [];
    
    public int Install(int stage, string data)
    {
        int statusCode = 0;
        switch (stage)
        {
            case 1:
                Utilities.BashStack.PushNextStage(stage + 1, Package.Name());
                this.InstallDependencies();
                break;
            case 2:
                Utilities.BashStack.PushBashCommand("apt install vim", true);
                break;
            default:
                throw new ArgumentException($"{Package.PrettyName()} does not have {stage} stages of installation.");
        }

        return statusCode;
    }
}