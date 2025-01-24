using OSManager.Core.Enums;

namespace OSManager.Daemon.Packages;

public class Qalculate : BasePackage
{
    #region public static members
    public static readonly Qalculate Instance = new();
    #endregion
    
    #region public members
    public override Package Package { get; } = Package.Qalculate;
    public override List<IPackage> Dependencies { get; } = [];
    public override List<IPackage> OptionalExtras { get; } = [];
    #endregion
    
    #region protected members
    protected override List<Func<string, InstallStepReturnData>> InstallSteps { get; }
    protected override List<Func<int>> ConfigureSteps { get; }
    protected override List<Func<int>> BackupConfigurationSteps { get; }
    #endregion
    
    #region constructors
    private Qalculate()
    {
        InstallSteps = [AptInstall];
        ConfigureSteps = [];
        BackupConfigurationSteps = [];
    }
    #endregion
    
    #region private methods
    private InstallStepReturnData AptInstall(string data)
    {
        return new InstallStepReturnData(
            0,
            [
                () => Utilities.BashStack.PushBashCommand("apt install -y qalculate-gtk", true),
                () => Utilities.BashStack.PushBashCommand("apt remove -y gnome-calculator", true),
                () => Utilities.BashStack.PushBashCommand("apt autoremove", true),
            ],
            null
        );
    }
    #endregion
}