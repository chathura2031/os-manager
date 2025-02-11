using OSManager.Core.Enums;

namespace OSManager.Daemon.Packages;

public class Element : BasePackage
{
    #region public static members
    public static readonly Element Instance = new();
    #endregion
    
    #region public members
    public override Package Package { get; } = Package.Element;
    public override List<IPackage> Dependencies { get; } = [AptTransportHttps.Instance];
    public override List<IPackage> OptionalExtras { get; } = [];
    #endregion
    
    #region protected members
    protected override List<Func<string, InstallStepReturnData>> InstallSteps { get; }
    protected override List<Func<int>> ConfigureSteps { get; }
    protected override List<Func<int>> BackupConfigurationSteps { get; }
    #endregion
    
    #region constructors
    private Element()
    {
        InstallSteps = [Something, Something2];
        ConfigureSteps = [];
        BackupConfigurationSteps = [];
    }
    #endregion
    
    #region private methods
    private InstallStepReturnData Something(string data)
    {
        // Add apt update to be run after this step
        UpdateAndUpgrade.Instance.ResetLastRun();
        RunPackagesPostStep.Add(Package.INTERNAL_UpdateAndUpgrade);
        
        return new InstallStepReturnData(
            0,
            [
                () => Utilities.BashStack.PushBashCommand("wget -O /usr/share/keyrings/element-io-archive-keyring.gpg https://packages.element.io/debian/element-io-archive-keyring.gpg", true),
                () => Utilities.BashStack.PushBashCommand("echo \"deb [signed-by=/usr/share/keyrings/element-io-archive-keyring.gpg] https://packages.element.io/debian/ default main\" | sudo tee /etc/apt/sources.list.d/element-io.list")
            ],
            null
        );
    }

    private InstallStepReturnData Something2(string data)
    {
        return new InstallStepReturnData(
            0,
            [
                () => Utilities.BashStack.PushBashCommand("apt install -y element-desktop", true)
            ],
            null
        );
    }
    #endregion
}