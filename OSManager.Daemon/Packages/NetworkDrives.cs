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
                    () => Utilities.BashStack.PushBashCommand($"{Utilities.EncryptorPath} -f true --workingdir {Utilities.EncryptedBackupDirectory} --action DecryptAll"),
                    () => Utilities.BashStack.PushConfigureStage(stage + 1, Package.Name())
                ]);
                break;
            }
            case 2:
            {
                string encryptedOrigin = Path.Join(Utilities.EncryptedBackupDirectory, Package.Name());
                
                Utilities.CopyFile(Path.Join(encryptedOrigin, "smbcredentials"), Path.Join(Utilities.HomeDirectory, ".smbcredentials"));
                
                string fstabFilename = "fstab";
                string fstabFilePath = Path.Join("/etc", fstabFilename);

                IEnumerable<string> linesToAdd = File.ReadLines(Path.Join(encryptedOrigin, fstabFilename));

                // Get mount points from the fstab file
                List<string> mountPoints = [];
                foreach (string rawLine in linesToAdd)
                {
                    string line = rawLine.Trim();
                    if (line[0] == '#')
                    {
                        continue;
                    }

                    // Read until the first space (the end of the first parameter)
                    int i = 0;
                    while (line[i] != ' ')
                    {
                        i++;
                    }

                    // Read until the end of the spaces (the start of the second parameter)
                    while (line[i] == ' ')
                    {
                        i++;
                    }

                    // Read until the end of the second parameter
                    int startIdx = i;
                    while (line[i] != ' ')
                    {
                        i++;
                    }
                    int endIdx = i;
                    
                    string mountPoint = line.Substring(startIdx, endIdx-startIdx);
                    mountPoints.Add(mountPoint);
                }

                Utilities.ReplaceContentBlockInFile(
                    fstabFilePath,
                    out string modifiedFilePath,
                    linesToAdd,
                    "# Start custom entries",
                    "# End custom entries"
                );

                Utilities.RunInReverse([
                    () => Utilities.BashStack.PushBashCommand("mkdir -p " + string.Join(' ', mountPoints), true),
                    () => Utilities.BashStack.PushBashCommand($"mv -v {modifiedFilePath} {fstabFilePath}", true),
                    () => Utilities.BashStack.PushConfigureStage(stage + 1, Package.Name())
                ]);
                
                break;
            }
            case 3:
            {
                Utilities.BashStack.PushBashCommand($"{Utilities.EncryptorPath} -f true --workingdir {Utilities.EncryptedBackupDirectory} --action EncryptAll");
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
                string encryptedOrigin = Path.Join(Utilities.EncryptedBackupDirectory, Package.Name());
                
                Utilities.CopyFile(Path.Join(Utilities.HomeDirectory, ".smbcredentials"), Path.Join(encryptedOrigin, "smbcredentials"));
                
                string fstabFilename = "fstab";
                string fstabFilePath = Path.Join("/etc", fstabFilename);
                string tmpFstabFilePath = Path.Join("/tmp", $"osman-{Guid.NewGuid()}.tmp");
                
                // Create a new temporary file with just the custom fstab entries
                using (StreamReader reader = new StreamReader(fstabFilePath))
                using (StreamWriter writer = new StreamWriter(tmpFstabFilePath))
                {
                    bool startWrite = false;
                    string? lastLine = null;
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine()!;
                        if (!startWrite && line == "# Start custom entries")
                        {
                            startWrite = true;
                        }

                        if (startWrite)
                        {
                            writer.WriteLine(line);
                        }

                        if (startWrite && line == "# End custom entries")
                        {
                            startWrite = false;
                        }

                        lastLine = line;
                    }

                    if (lastLine == null || lastLine.Trim() != "")
                    {
                        writer.WriteLine();
                    }
                }

                File.Move(tmpFstabFilePath, Path.Join(encryptedOrigin, fstabFilename), true);
                
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
}