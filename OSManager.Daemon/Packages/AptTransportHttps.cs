using OSManager.Core.Enums;

namespace OSManager.Daemon.Packages;

public class AptTransportHttps : BasePackage
{
    #region public static members
    public static readonly AptTransportHttps Instance = new();
    #endregion
    
    #region public members
    public override Package Package { get; } = Package.AptTransportHttps;
    public override List<IPackage> Dependencies { get; } = [];
    public override List<IPackage> OptionalExtras { get; } = [];
    #endregion
    
    #region protected members
    protected override List<Func<string, InstallStepReturnData>> InstallSteps { get; }
    protected override List<Func<int>> ConfigureSteps { get; }
    protected override List<Func<int>> BackupConfigurationSteps { get; }
    #endregion
    
    #region constructors
    private AptTransportHttps()
    {
        InstallSteps = [InstallFromApt];
        ConfigureSteps = [];
        BackupConfigurationSteps = [];
    }
    #endregion
    
    #region private methods
    private InstallStepReturnData InstallFromApt(string data)
    {
        return new InstallStepReturnData(
            0,
            [
                () => Utilities.BashStack.PushBashCommand("apt install -y apt-transport-https", true)
            ],
            null
        );
    }
    #endregion
}