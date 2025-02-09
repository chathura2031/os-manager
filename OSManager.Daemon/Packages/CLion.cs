using OSManager.Core.Enums;

namespace OSManager.Daemon.Packages;

public class CLion : BasePackage
{
    #region public static members
    public static readonly CLion Instance = new();
    #endregion
    
    #region public members

    public override Package Package { get; } = Package.CLion;
    public override List<IPackage> Dependencies { get; } = [];
    public override List<IPackage> OptionalExtras { get; } = [];
    #endregion
    
    #region protected members
    protected override List<Func<string, InstallStepReturnData>> InstallSteps { get; }
    protected override List<Func<int>> ConfigureSteps { get; }
    protected override List<Func<int>> BackupConfigurationSteps { get; }
    #endregion
    
    #region constructors
    private CLion()
    {
        InstallSteps = [];
        ConfigureSteps = [];
        BackupConfigurationSteps = [];
    }
    #endregion
    
    #region private methods
    // private InstallStepReturnData 
    #endregion
}