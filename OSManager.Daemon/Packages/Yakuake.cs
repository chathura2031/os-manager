using OSManager.Core.Enums;
using OSManager.Core.Extensions;
using OSManager.Daemon.Extensions;

namespace OSManager.Daemon.Packages;

public class Yakuake : IPackage
{
    public static readonly Yakuake Instance = new();

    public Package Package { get; } = Package.Yakuake;

    public List<IPackage> Dependencies { get; } = [Qdbus.Instance];

    public List<IPackage> OptionalExtras { get; } = [];
    
    // The name of the config file in the package's directory
    private const string ConfigFileName = "yakuakerc";

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
                Utilities.BashStack.PushBashCommand("apt install -y yakuake", true);
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
                string backupDir = Path.Join(Utilities.BackupDirectory, Package.Name());
                string originFilePath = Path.Join(backupDir, ConfigFileName);
                string destinationDir = Path.Join(Utilities.HomeDirectory, ".config");
                string destinationFilePath = Path.Join(destinationDir, ConfigFileName);
                
                Directory.CreateDirectory(destinationDir);
                File.Copy(originFilePath, destinationFilePath, true);
                
                Utilities.RunInReverse([
                    () => Utilities.BashStack.PushBashCommand($"cp -v {Path.Join(backupDir, "open-yakuake-here")} /usr/local/bin", true),
                    () => Utilities.BashStack.PushBashCommand("gsettings set org.cinnamon.desktop.default-applications.terminal exec 'open-yakuake-here'")
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
                string originFilePath = Path.Join(Utilities.HomeDirectory, ".config", ConfigFileName);
                string backupDir = Path.Join(Utilities.BackupDirectory, Package.Name());
                string destinationFilePath = Path.Join(backupDir, ConfigFileName);
                
                Directory.CreateDirectory(backupDir);
                File.Copy(originFilePath, destinationFilePath, true);
                File.Copy("/usr/local/bin/open-yakuake-here", Path.Join(backupDir, "open-yakuake-here"), true);
                
                break;
            }
            default:
                throw new ArgumentException($"{Package.PrettyName()} does not have {stage} stages of configuration backup.");
        }
        
        return statusCode;
    }
}