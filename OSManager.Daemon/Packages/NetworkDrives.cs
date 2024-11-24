using OSManager.Core.Enums;
using OSManager.Core.Extensions;

namespace OSManager.Daemon.Packages;

public class NetworkDrives : IPackage
{
    public static readonly NetworkDrives Instance = new();

    public Package Package { get; } = Package.NetworkDrives;

    public List<IPackage> Dependencies { get; } = [];

    public List<IPackage> OptionalExtras { get; } = [];
    
    public int Install(int stage, string data)
    {
        return 0;
    }

    public int Configure(int stage)
    {
        int statusCode = 0;
        switch (stage)
        {
            case 1:
            {
                Utilities.RunInReverse([
                    // TODO: Uncomment this
                    // () => Utilities.BashStack.PushBashCommand($"{Utilities.EncryptorPath} -f true --workingdir {Utilities.EncryptedBackupDirectory} --action DecryptAll"),
                    () => Utilities.BashStack.PushConfigureStage(stage + 1, Package.Name())
                ]);
                break;
            }
            case 2:
            {
                string encryptedOrigin = Path.Join(Utilities.EncryptedBackupDirectory, Package.Name());
                
                // TODO: Test this
                Utilities.CopyFile(Path.Join(encryptedOrigin, "smbcredentials"),
                    Path.Join(Utilities.HomeDirectory, ".smbcredentials"));
                
                string fstabFilename = "fstab";
                string fstabFilePath = Path.Join("/etc", fstabFilename);
                string tmpFstabFilePath = Path.Join("/tmp", $"osman-{Guid.NewGuid()}.tmp");
                
                HashSet<string> linesToAdd = [..File.ReadLines(Path.Join(encryptedOrigin, fstabFilename))];

                string[] content = File.ReadAllLines(fstabFilePath);
                foreach (string line in content)
                {
                    string trimmedLine = line.Trim();
                    if (linesToAdd.Contains(trimmedLine))
                    {
                        linesToAdd.Remove(trimmedLine);
                    }
                }

                string[] newContent = [..content, ..linesToAdd.ToArray()];
                File.WriteAllLines(tmpFstabFilePath, newContent);
                
                Utilities.RunInReverse([
                    () => Utilities.BashStack.PushBashCommand($"mv -v ${tmpFstabFilePath} {fstabFilePath}", true),
                    () => Utilities.BashStack.PushConfigureStage(stage + 1, Package.Name())
                ]);
                
                break;
            }
            case 3:
            {
                // TODO: Uncomment this
                // Utilities.BashStack.PushBashCommand($"{Utilities.EncryptorPath} -f true --workingdir {Utilities.EncryptedBackupDirectory} --action EncryptAll");
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
                break;
            }
            default:
                throw new ArgumentException($"{Package.PrettyName()} does not have {stage} stages of configuration backup.");
        }
        
        return statusCode;
    }
}