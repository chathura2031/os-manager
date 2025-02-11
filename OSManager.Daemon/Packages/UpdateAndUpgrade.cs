using System.Globalization;
using OSManager.Core.Enums;
using OSManager.Core.Extensions;

namespace OSManager.Daemon.Packages;

public class UpdateAndUpgrade : BasePackage
{
    #region public static members
    public static readonly UpdateAndUpgrade Instance = new();
    #endregion
    
    #region public members
    public override Package Package { get; } = Package.INTERNAL_UpdateAndUpgrade;
    public override List<IPackage> Dependencies { get; } = [];
    public override List<IPackage> OptionalExtras { get; } = [];
    #endregion
    
    #region protected members
    protected override List<Func<string, InstallStepReturnData>> InstallSteps { get; }
    protected override List<Func<int>> ConfigureSteps { get; }
    protected override List<Func<int>> BackupConfigurationSteps { get; }
    #endregion
    
    #region private members
    private DateTime? _lastRun = null;
    #endregion
    
    #region constructors
    private UpdateAndUpgrade()
    {
        InstallSteps = [AptUpdateAndUpgrade, SaveLastRunTime];
        ConfigureSteps = [];
        BackupConfigurationSteps = [];
    }
    #endregion
    
    #region public methods
    public void ResetLastRun() => _lastRun = null;
    #endregion
    
    #region private methods
    private InstallStepReturnData AptUpdateAndUpgrade(string data)
    {
        Action[] commands = [
             () => Utilities.BashStack.PushBashCommand("apt update", true),
             () => Utilities.BashStack.PushBashCommand("apt upgrade -y", true),
             () => Utilities.BashStack.PushBashCommand("apt autoremove -y", true)
        ];

         return new InstallStepReturnData(
             0,
             _lastRun == null || DateTime.Now - _lastRun >= new TimeSpan(0, 5, 0) ? commands : [],
             null
         );
    }

    private InstallStepReturnData SaveLastRunTime(string data)
    {
         _lastRun = DateTime.Now;

         return new InstallStepReturnData(0, [], null);
    }
    #endregion
}