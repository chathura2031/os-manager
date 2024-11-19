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
    
    private DateTime? LastRun;

    public int Install(int stage, string data)
    {
        int statusCode = 0;
        switch (stage)
        {
            case 1:
                if (LastRun == null || DateTime.Now - LastRun >= new TimeSpan(0, 5, 0))
                {
                    Utilities.RunInReverse([
                        () => Utilities.BashStack.PushBashCommand("apt update", true),
                        () => Utilities.BashStack.PushBashCommand("apt upgrade", true),
                        () => Utilities.BashStack.PushBashCommand("apt autoremove", true),
                        () => Utilities.BashStack.PushNextStage(stage + 1, Package.Name())
                    ]);
                }
                break;
            case 2:
                LastRun = DateTime.Now;
                break;
            default:
                throw new ArgumentException($"{Package.PrettyName()} does not have {stage} stages of installation.");
        }
        return statusCode;
    }
}