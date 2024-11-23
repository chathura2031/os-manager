using OSManager.Core.Enums;
using OSManager.Core.Extensions;
using OSManager.Daemon.Extensions;

namespace OSManager.Daemon.Packages;

public class Chrome : IPackage
{
    public static readonly Chrome Instance = new();

    public Package Package { get; } = Package.Chrome;

    public List<IPackage> Dependencies { get; } = [];

    public List<IPackage> OptionalExtras { get; } = [];
    
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
                statusCode = DownloadPackage(2, out string filePath);
                if (statusCode != 0)
                {
                    Console.WriteLine("Failed to download debian file");
                    File.Delete(filePath);
                    return 1;
                }
                
                Utilities.RunInReverse([
                    () => Utilities.BashStack.PushBashCommand($"apt install -y --fix-broken {filePath}", true),
                    () => Utilities.BashStack.PushInstallStage(stage + 1, Package.Name(), filePath),
                ]);
                break;
            }
            case 3:
            {
                statusCode = DeletePackage(2, data);
                if (statusCode != 0)
                {
                    Console.WriteLine("Failed to delete debian file");
                    return 1;
                }
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
                Utilities.RunInReverse([
                    () => Utilities.BashStack.PushBashCommand($"{Utilities.EncryptorPath} -f true --workingdir {Utilities.EncryptedBackupDirectory} --action DecryptAll"),
                    () => Utilities.BashStack.PushBackupConfigStage(stage + 1, Package.Name())
                ]);
                break;
            }
            case 2:
            {
                string profileName = "Default";
                string origin = Path.Join(Utilities.ConfigDirectory, Package.Name());
                // TODO: Repeat for all profiles
                string profileOrigin = Path.Join(origin, profileName);

                string encryptedDestination = Path.Join(Utilities.EncryptedBackupDirectory, Package.Name());
                string encryptedProfileDestination = Path.Join(encryptedDestination, profileName);
                
                // Backup profile metadata
                Utilities.CopyFile(Path.Join(origin, "Local State"), Path.Join(encryptedDestination, "Local State"));
                
                // Backup extension data
                Utilities.CopyFile(Path.Join(profileOrigin, "Preferences"), Path.Join(encryptedProfileDestination, "Preferences"));
                Utilities.CopyFile(Path.Join(profileOrigin, "Bookmarks"), Path.Join(encryptedProfileDestination, "Bookmarks"));
                Utilities.CopyDirectory(Path.Join(profileOrigin, "Extensions"), Path.Join(encryptedProfileDestination, "Extensions"));
                Utilities.CopyDirectory(Path.Join(profileOrigin, "Local Extension Settings"), Path.Join(encryptedProfileDestination, "Local Extension Settings"));
                Utilities.CopyDirectory(Path.Join(profileOrigin, "Managed Extension Settings"), Path.Join(encryptedProfileDestination, "Managed Extension Settings"));

                // TODO: Read in and store the important parts of the preferences file
                // Functions.CopyDirectory(sourcePath, Path.Join(Functions.EncryptedBackupDirectory, "google-chrome"));
                // JObject o1 = JObject.Parse(File.ReadAllText(Path.Join(sourcePath, "Local State")));
                // JObject o2 = JObject.Parse(File.ReadAllText(Path.Join(profileSourcePath, "Preferences")));
                
                Utilities.BashStack.PushBackupConfigStage(stage + 1, Package.Name());
                break;
            }
            case 3:
            {
                Utilities.BashStack.PushBashCommand($"{Utilities.EncryptorPath} -f true --workingdir {Utilities.EncryptedBackupDirectory} --action EncryptAll");
                break;
            }
            default:
                throw new ArgumentException($"{Package.PrettyName()} does not have {stage} stages of configuration backup.");
        }
        
        return statusCode;
    }
    
    private int DownloadPackage(int verbosity, out string filePath)
    {
        // Download the debian package
        if (verbosity > 0)
        {
            Console.WriteLine("Downloading debian package...");
        }

        int statusCode = Utilities.DownloadFromUrl(
            "https://dl.google.com/linux/direct/google-chrome-stable_current_amd64.deb", $"{Package.Name()}.deb",
            out filePath, $"/tmp/osman-{Guid.NewGuid()}/");

        return statusCode;
    }
    
    private int DeletePackage(int verbosity, string filePath)
    {
        // Delete the debian package
        if (verbosity > 0)
        {
            Console.WriteLine("Deleting debian package...");
        }
        File.Delete(filePath);

        return 0;
    }
}