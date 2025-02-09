using OSManager.Core.Enums;
using OSManager.Core.Extensions;

namespace OSManager.Daemon.Packages;

public class CLion : BasePackage
{
    #region public static members
    public static readonly CLion Instance = new();
    #endregion
    
    #region public members
    public override Package Package { get; } = Package.CLion;
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
    private string InstallDirPath => "/usr/share/jetbrains/";
    private string DesktopFilePath => $"/usr/share/applications/jetbrains-{Package.Name()}.desktop";
    private string Version => "2024.3.2";
    private string DesktopFileContent => $"""
                                         [Desktop Entry]
                                         Version=1.0
                                         Type=Application
                                         Name=JetBrains CLion
                                         Icon=/usr/share/jetbrains/{Package.Name()}-{Version}/bin/clion.svg
                                         Exec=/usr/share/jetbrains/{Package.Name()}-{Version}/bin/clion.sh %f
                                         Comment=A cross-platform IDE for C and C++
                                         Categories=Development;IDE;
                                         Terminal=false
                                         StartupWMClass=jetbrains-clion
                                         StartupNotify=true
                                         """;
    #endregion
    
    #region constructors
    private CLion()
    {
        InstallSteps = [DownloadAndExtractPackage, InstallExtractedPackage, GenerateDesktopFile];
        ConfigureSteps = [ConfigureClionRc];
        BackupConfigurationSteps = [];
    }
    #endregion
    
    #region private methods
    private InstallStepReturnData DownloadAndExtractPackage(string data)
    {
        // TODO: Automatically retrieve the latest version
        string downloadDirectory = $"/tmp/osman-{Guid.NewGuid()}/";

        Console.WriteLine($"Downloading package...");
        int statusCode = Utilities.DownloadFromUrl(
            $"https://download.jetbrains.com/cpp/CLion-{Version}.tar.gz",
            $"{Package.Name()}.tar.gz",
            out string filePath,
            downloadDirectory
        );
        
        Console.WriteLine("Extracting package...");
        return new InstallStepReturnData(
            statusCode,
            [
                () => Utilities.BashStack.PushBashCommand($"tar -xvf {filePath} -C {downloadDirectory}"),
                () => Utilities.BashStack.PushBashCommand($"rm -v {filePath}"),
                () => Utilities.BashStack.PushBashCommandOutputToProgramStack($"find {downloadDirectory} -mindepth 1 -maxdepth 1 -type d")
            ],
            downloadDirectory
        );
    }

    private InstallStepReturnData InstallExtractedPackage(string downloadDirectory)
    {
        Console.WriteLine("Moving extracted files...");
        string extractedDirectoryPath = Utilities.ProgramStack.Pop()!.Trim();
        string destinationDirectoryPath = Path.Combine(InstallDirPath, $"{Package.Name()}-{Version}");

        return new InstallStepReturnData(
           0,
           [
               () => Utilities.BashStack.PushBashCommand($"mkdir -p {InstallDirPath}", true),
               () => Utilities.BashStack.PushBashCommand($"mv -v \"{extractedDirectoryPath}\" {destinationDirectoryPath}", true)
           ],
           null
           );
    }

    private InstallStepReturnData GenerateDesktopFile(string data)
    {
        Console.WriteLine("Creating desktop file...");
        string filePath = $"/tmp/osman-{Guid.NewGuid()}.desktop";
        File.WriteAllText(filePath, DesktopFileContent);
        return new InstallStepReturnData(0, [() => Utilities.BashStack.PushBashCommand($"mv -v {filePath} {DesktopFilePath}", true)], null);
    }
    
    private int ConfigureClionRc()
    {
        Utilities.BashStack.PushBashCommand($"ln -s {Vim.Instance.ConfigFilePath} {ConfigFilePath}");
        
        return 0;
    }
    #endregion
}