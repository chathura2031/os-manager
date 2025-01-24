using OSManager.Core.Enums;
using OSManager.Core.Extensions;

namespace OSManager.Daemon.Packages;

public class NetworkDrives : BasePackage
{
    #region public static members
    public static readonly NetworkDrives Instance = new();
    #endregion

    #region public members
    public override Package Package { get; } = Package.NetworkDrives;
    public override List<IPackage> Dependencies { get; } = [];
    public override List<IPackage> OptionalExtras { get; } = [];
    #endregion
    
    #region protected members
    protected override List<Func<string, InstallStepReturnData>> InstallSteps { get; }
    protected override List<Func<int>> ConfigureSteps { get; }
    protected override List<Func<int>> BackupConfigurationSteps { get; }
    #endregion
    
    #region private members
    private string CredentialFileDirPath => Utilities.HomeDirectory;
    private string CredentialFilePath => Path.Join(CredentialFileDirPath, ".smbcredentials");
    private string CredentialTemplateFilePath => Path.Join(EncryptedBackupDirPath, "smbcredentials");
    private string FstabFilePath => Path.Join("/etc", "fstab");
    private string FstabTemplateFilePath => Path.Join(EncryptedBackupDirPath, "fstab");
    private string ScriptTemplateFilePath => Path.Join(EncryptedBackupDirPath, "connect-to-nas");
    private string MountScriptTemplateFilePath => Path.Join(EncryptedBackupDirPath, "mount-network-drives");
    private string VpnTemplateFilePath => Path.Join(EncryptedBackupDirPath, "VPNConfig.ovpn");
    private string ConnectionScriptFilePath => Path.Join(Utilities.UserScriptPath, "connect-to-nas");
    private string MountScriptFilePath => Path.Join(Utilities.UserScriptPath, "mount-network-drives");
    private string VpnFolderPath => "/opt/home-vpn";
    private string VpnFilePath => Path.Join(VpnFolderPath, "VPNConfig.ovpn");
    #endregion
    
    #region constructors
    private NetworkDrives()
    {
        InstallSteps = [];
        ConfigureSteps = [DecryptEncryptedBackupDir, CreateFolders, UpdateFiles, TransferScripts, EncryptEncryptedBackupDir];
        BackupConfigurationSteps = [DecryptEncryptedBackupDir, BackupFiles, EncryptEncryptedBackupDir];
    }
    #endregion

    #region public methods
    public int Configure(int stage)
    {
        int statusCode = 0;
        switch (stage)
        {
            case 2:
            {
                string encryptedOrigin = Path.Join(Utilities.EncryptedBackupDirectory, Package.Name());
                
                Utilities.CopyFile(Path.Join(EncryptedBackupDirPath, "smbcredentials"), Path.Join(Utilities.HomeDirectory, ".smbcredentials"));
                
                string fstabFilename = "fstab";
                string fstabFilePath = Path.Join("/etc", fstabFilename);

                IEnumerable<string> linesToAdd = File.ReadLines(Path.Join(EncryptedBackupDirPath, fstabFilename));

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
        }
        
        return statusCode;
    }
    #endregion

    #region private methods
    private int EncryptEncryptedBackupDir()
    {
        Utilities.BashStack.PushBashCommand($"{Utilities.EncryptorPath} -f true --workingdir {Utilities.EncryptedBackupDirectory} --action EncryptAll");
        return 0;
    }

    private int DecryptEncryptedBackupDir()
    {
        Utilities.BashStack.PushBashCommand($"{Utilities.EncryptorPath} -f true --workingdir {Utilities.EncryptedBackupDirectory} --action DecryptAll");
        return 0;
    }

    private int CreateFolders()
    {
        Directory.CreateDirectory(Path.Join(Utilities.HomeDirectory, "LDocuments"));
        Directory.CreateDirectory(Path.Join(Utilities.HomeDirectory, "LDownloads"));

        return 0;
    }

    private int UpdateFiles()
    {
        if (File.Exists(CredentialTemplateFilePath))
        {
            Directory.CreateDirectory(CredentialFileDirPath);
            Utilities.CopyFile(CredentialTemplateFilePath, CredentialFilePath);
        }

        if (File.Exists(FstabTemplateFilePath))
        {
            IEnumerable<string> linesToAdd = File.ReadLines(FstabTemplateFilePath);

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

                string mountPoint = line.Substring(startIdx, endIdx - startIdx);
                mountPoints.Add(mountPoint);
            }

            Utilities.ReplaceContentBlockInFile(
                FstabFilePath,
                out string modifiedFilePath,
                linesToAdd,
                "# Start custom entries",
                "# End custom entries"
            );

            Utilities.RunInReverse([
                () => Utilities.BashStack.PushBashCommand("mkdir -p " + string.Join(' ', mountPoints), true),
                () => Utilities.BashStack.PushBashCommand($"mv -v {modifiedFilePath} {FstabFilePath}", true),
            ]);
        }

        return 0;
    }

    private int TransferScripts()
    {
        Utilities.RunInReverse([
            () => Utilities.BashStack.PushBashCommand($"cp -v {ScriptTemplateFilePath} {ConnectionScriptFilePath}", true),
            () => Utilities.BashStack.PushBashCommand($"cp -v {MountScriptTemplateFilePath} {MountScriptFilePath}", true),
            () => Utilities.BashStack.PushBashCommand($"mkdir -p {VpnFolderPath}", true),
            () => Utilities.BashStack.PushBashCommand($"cp -v {VpnTemplateFilePath} {VpnFilePath}", true)
        ]);
        
        return 0;
    }
    
    private int BackupFiles()
    {
        Directory.CreateDirectory(EncryptedBackupDirPath);
        
        if (File.Exists(CredentialFilePath))
        {
            Utilities.CopyFile(CredentialFilePath, CredentialTemplateFilePath, true);
        }

        if (File.Exists(FstabFilePath))
        {
            Utilities.ReadContentBlockInFile(
                FstabFilePath,
                out string contentFilePath,
                "# Start custom entries",
                "# End custom entries"
            );

            File.Move(contentFilePath, FstabTemplateFilePath, true);
        }

        if (File.Exists(VpnFilePath))
        {
            Utilities.CopyFile(VpnFilePath, VpnTemplateFilePath, true);
        }

        if (File.Exists(ConnectionScriptFilePath))
        {
            Utilities.CopyFile(ConnectionScriptFilePath, ScriptTemplateFilePath, true);
        }
        
        if (File.Exists(MountScriptFilePath))
        {
            Utilities.CopyFile(MountScriptFilePath, MountScriptTemplateFilePath, true);
        }

        return 0;
    }
    #endregion
}