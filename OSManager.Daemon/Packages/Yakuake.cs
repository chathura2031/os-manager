using OSManager.Core.Enums;
using OSManager.Core.Extensions;
using OSManager.Daemon.Extensions;

namespace OSManager.Daemon.Packages;

public class Yakuake : IPackage
{
    public static readonly Yakuake Instance = new();

    public Package Package { get; } = Package.Yakuake;

    public List<IPackage> Dependencies { get; } = [];

    public List<IPackage> OptionalExtras { get; } = [];
    
    public int Install(int stage, string data)
    {
        int statusCode = 0;
        switch (stage)
        {
            case 1:
            {
                Utilities.BashStack.PushInstallStage(stage + 1, Package.Name());
                this.InstallDependencies();
                break;
            }
            case 2:
            {
                Utilities.BashStack.PushBashCommand("apt install yakuake", true);
                break;
            }
            default:
                throw new ArgumentException($"{Package.PrettyName()} does not have {stage} stages of installation.");
        }

        return statusCode;
    }

    public int Configure(int stage)
    {
        return 0;
    }

    public int BackupConfig(int stage)
    {
        return 0;
    }
}