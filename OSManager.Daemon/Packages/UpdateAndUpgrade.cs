using System.Globalization;
using OSManager.Core.Enums;
using OSManager.Core.Extensions;

namespace OSManager.Daemon.Packages;

public class UpdateAndUpgrade : IPackage
{
    public static readonly UpdateAndUpgrade Instance = new();

    public Package Package { get; } = Package.INTERNAL_UpdateAndUpgrade;
    
    public List<IPackage> Dependencies { get; } = [];
    
    public List<IPackage> OptionalExtras { get; } = [];
    
    private DateTime? _lastRun;

    public int Install(int stage, string data)
    {
        int statusCode = 0;
        switch (stage)
        {
            case 1:
                if (_lastRun == null || DateTime.Now - _lastRun >= new TimeSpan(0, 5, 0))
                {
                    Utilities.RunInReverse([
                        () => Utilities.BashStack.PushBashCommand("apt update", true),
                        () => Utilities.BashStack.PushBashCommand("apt upgrade -y", true),
                        () => Utilities.BashStack.PushBashCommand("apt autoremove -y", true),
                        () => Utilities.BashStack.PushInstallStage(stage + 1, Package.Name())
                    ]);
                }
                break;
            case 2:
                _lastRun = DateTime.Now;
                break;
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