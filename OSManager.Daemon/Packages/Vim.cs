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
                Utilities.BashStack.PushBashCommand("apt install vim", true);
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
                string origin = Path.Join(Utilities.WorkingDirectory, Package.Name(), ConfigFileName);
                string destinationDir = DestinationConfigDirPath;
                string destination = Path.Join(destinationDir, "vimrc");
                Utilities.RunInReverse([
                    () => Utilities.BashStack.PushBashCommand($"mkdir -p {destinationDir}", true),
                    () => Utilities.BashStack.PushBashCommand($"cp -v {origin} {destination}", true)
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
                string origin = Path.Join(DestinationConfigDirPath, "vimrc");
                string destinationDir = Path.Join(Utilities.WorkingDirectory, Package.Name());
                string destination = Path.Join(destinationDir, ConfigFileName);
                Utilities.RunInReverse([
                    () => Utilities.BashStack.PushBashCommand($"mkdir -p {destinationDir}"),
                    () => Utilities.BashStack.PushBashCommand($"cp -v {origin} {destination}")
                ]);
                break;
            }
            default:
                throw new ArgumentException($"{Package.PrettyName()} does not have {stage} stages of configuration backup.");
        }
        
        return statusCode;
    }
}