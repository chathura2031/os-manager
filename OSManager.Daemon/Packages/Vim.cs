using OSManager.Core.Enums;

namespace OSManager.Daemon.Packages;

public class Vim : BasePackage
{
    #region public static members
    public static readonly Vim Instance = new();
    #endregion

    #region public members
    public override Package Package { get; } = Package.Vim;
    public override List<IPackage> Dependencies { get; } = [];
    public override List<IPackage> OptionalExtras { get; } = [];
    public readonly string ConfigFilePath = Path.Join(DestinationConfigDirPath, ConfigFileName);
    #endregion
    
    #region protected members
    protected override List<Func<string, InstallStepReturnData>> InstallSteps { get; }
    protected override List<Func<int>> ConfigureSteps { get; }
    protected override List<Func<int>> BackupConfigurationSteps { get; }
    #endregion

    #region private members
    private const string ConfigFileName = "vimrc";
    private const string DestinationConfigDirPath = "/etc/vim";
    #endregion

    #region ctor
    private Vim()
    {
        InstallSteps = [AptInstall];
        ConfigureSteps = [CreateVimrcFolder, ConfigureVimrc];
        BackupConfigurationSteps = [BackupVimrc];
    }
    #endregion

    #region private methods
    private InstallStepReturnData AptInstall(string data)
    {
        return new InstallStepReturnData(
            0,
            [() => Utilities.BashStack.PushBashCommand("apt install -y vim", true)],
            null
        );
    }

    private int CreateVimrcFolder()
    {
        if (!Directory.Exists(DestinationConfigDirPath))
        {
            Utilities.BashStack.PushBashCommand($"mkdir -p {DestinationConfigDirPath}", true);
        }
        
        return 0;
    }

    private int ConfigureVimrc()
    {
        if (!Directory.Exists(DestinationConfigDirPath))
        {
            throw new Exception($"{DestinationConfigDirPath} does not exist");
        }
        
        string templateFilePath = Path.Join(BackupDirPath, ConfigFileName);

        // Nothing to do if the vimrc file doesn't exist
        if (!File.Exists(templateFilePath))
        {
            return 0;
        }
        
        IEnumerable<string> linesToAdd = File.ReadLines(templateFilePath);
        
        Utilities.ReplaceContentBlockInFile(
            ConfigFilePath,
            out string modifiedFilePath,
            linesToAdd,
            "\" Start custom vim entries",
            "\" End custom vim entries"
        );

        Utilities.BashStack.PushBashCommand($"mv -v {modifiedFilePath} {ConfigFilePath}", true);
        return 0;
    }

    private int BackupVimrc()
    {
        if (!File.Exists(ConfigFilePath))
        {
            return 0;
        }
        
        string destinationFilePath = Path.Join(BackupDirPath, ConfigFileName);
        
        Utilities.ReadContentBlockInFile(
            ConfigFilePath,
            out string contentFilePath,
            "\" Start custom vim entries",
            "\" End custom vim entries"
        );
        
        Directory.CreateDirectory(BackupDirPath);
        File.Move(contentFilePath, destinationFilePath, true);

        return 0;
    }
    #endregion
}