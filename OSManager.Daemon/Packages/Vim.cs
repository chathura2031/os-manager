using OSManager.Core.Enums;
using OSManager.Core.Extensions;
using OSManager.Daemon.Extensions;

namespace OSManager.Daemon.Packages;

public class Vim : IPackage
{
    public static readonly Vim Instance = new();

    public Package Package { get; } = Package.Vim;

    public List<IPackage> Dependencies { get; } = [];

    public List<IPackage> OptionalExtras { get; } = [];

    // The name of the config file in the package's directory
    private const string ConfigFileName = "vimrc";

    private const string DestinationConfigDirPath = "/etc/vim";
    
    public int Install(int stage, string data)
    {
        int statusCode = 0;
        switch (stage)
        {
            case 1:
            {
                Utilities.BashStack.PushInstallStage(stage + 1, Package.Name());
                this.InstallDependencies();
                break;
            }
            case 2:
            {
                Utilities.BashStack.PushBashCommand("apt install -y vim", true);
                break;
            }
            default:
                throw new ArgumentException($"{Package.PrettyName()} does not have {stage} stages of installation.");
        }

        return statusCode;
    }

    public int Configure(int stage)
    {
        int statusCode = 0;
        switch (stage)
        {
            case 1:
            {
                string templatePath = Path.Join(Utilities.BackupDirectory, Package.Name(), ConfigFileName);
                string destinationDir = DestinationConfigDirPath;
                string destinationFilePath = Path.Join(destinationDir, "vimrc");
                
                IEnumerable<string> linesToAdd = File.ReadLines(templatePath);
                
                Utilities.ReplaceContentBlockInFile(
                    destinationFilePath,
                    out string modifiedFilePath,
                    linesToAdd,
                    "\" Start custom vim entries",
                    "\" End custom vim entries"
                );
                
                Utilities.RunInReverse([
                    () => Utilities.BashStack.PushBashCommand($"mkdir -p {destinationDir}", true),
                    () => Utilities.BashStack.PushBashCommand($"mv -v {modifiedFilePath} {destinationFilePath }", true)
                ]);
                break;
            }
            default:
                throw new ArgumentException($"{Package.PrettyName()} does not have {stage} stages of configuration.");
        }
        
        return statusCode;
    }

    public int BackupConfig(int stage)
    {
        int statusCode = 0;
        switch (stage)
        {
            case 1:
            {
                string originFilePath = Path.Join(DestinationConfigDirPath, "vimrc");
                string destinationDir = Path.Join(Utilities.BackupDirectory, Package.Name());
                string destinationFilePath = Path.Join(destinationDir, ConfigFileName);
                
                Utilities.ReadContentBlockInFile(
                    originFilePath,
                    out string contentFilePath,
                    "\" Start custom vim entries",
                    "\" End custom vim entries"
                );
                
                Directory.CreateDirectory(destinationDir);
                File.Move(contentFilePath, destinationFilePath, true);
                break;
            }
            default:
                throw new ArgumentException($"{Package.PrettyName()} does not have {stage} stages of configuration backup.");
        }
        
        return statusCode;
    }
}