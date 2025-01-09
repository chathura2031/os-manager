using OSManager.Core.Enums;

namespace OSManager.Daemon.Packages;

public class Codium : BasePackage
{
    #region public static members
    public static readonly Codium Instance = new();
    #endregion
    
    #region public members
    public override Package Package { get; } = Package.Codium;
    public override List<IPackage> Dependencies { get; } = [];
    public override List<IPackage> OptionalExtras { get; } = [];
    #endregion
    
    #region protected members
    protected override List<Func<string, InstallStepReturnData>> InstallSteps { get; }
    protected override List<Func<int>> ConfigureSteps { get; }
    protected override List<Func<int>> BackupConfigurationSteps { get; }
    #endregion
    
    #region constructors
    private Codium()
    {
        InstallSteps = [SetupAndInstall];
        ConfigureSteps = [InstallExtensions];
        BackupConfigurationSteps = [];
    }
    #endregion
    
    #region private methods
    private InstallStepReturnData SetupAndInstall(string data)
    {
        Action[] bashCommands = [
            () => Utilities.BashStack.PushBashCommand("wget -qO - https://gitlab.com/paulcarroty/vscodium-deb-rpm-repo/raw/master/pub.gpg | gpg --dearmor | sudo dd of=/usr/share/keyrings/vscodium-archive-keyring.gpg"),
            () => Utilities.BashStack.PushBashCommand("echo 'deb [ signed-by=/usr/share/keyrings/vscodium-archive-keyring.gpg ] https://download.vscodium.com/debs vscodium main' | sudo tee /etc/apt/sources.list.d/vscodium.list"),
            () => Utilities.BashStack.PushBashCommand("apt update", true),
            () => Utilities.BashStack.PushBashCommand("apt install codium", true),
        ];
        
        return new InstallStepReturnData(
            0,
            bashCommands,
            null
        );
    }

    private int InstallExtensions()
    {
        Utilities.BashStack.PushBashCommand("codium --install-extension vscodevim.vim");
        return 0;
    }
    #endregion
}