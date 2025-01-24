using OSManager.Core.Enums;

namespace OSManager.Daemon.Packages;

public class Rider : BasePackage
{
    #region public static members
    public static readonly Rider Instance = new();
    #endregion
    
    #region public members
    public override Package Package { get; } = Package.Rider;
    public override List<IPackage> Dependencies { get; } = [Vim.Instance];
    public override List<IPackage> OptionalExtras { get; } = [];
    #endregion
    
    #region protected members
    protected override List<Func<string, InstallStepReturnData>> InstallSteps { get; }
    protected override List<Func<int>> ConfigureSteps { get; }
    protected override List<Func<int>> BackupConfigurationSteps { get; }
    #endregion
    
    #region private members
    private const string ConfigFileName = ".ideavimrc";
    private string ConfigFilePath => Path.Join(Utilities.HomeDirectory, ConfigFileName);
    #endregion
    
    #region constructors
    private Rider()
    {
        InstallSteps = [];
        ConfigureSteps = [ConfigureRiderRc];
        BackupConfigurationSteps = [];
    }
    #endregion
    
    #region private methods
    private int ConfigureRiderRc()
    {
        Utilities.BashStack.PushBashCommand($"ln -s {Vim.Instance.ConfigFilePath} {ConfigFilePath}");
        
        return 0;
    }
    #endregion
}