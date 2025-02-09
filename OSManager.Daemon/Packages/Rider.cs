using OSManager.Core.Enums;
using OSManager.Core.Extensions;

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
    private string InstallDirPath => "/usr/share/jetbrains/";
    private string DesktopFilePath => $"/usr/share/applications/jetbrains-{Package.Name()}.desktop";
    private string Version => "2024.3.5";
    private string DesktopFileContent => $"""
                                         [Desktop Entry]
                                         Version=1.0
                                         Type=Application
                                         Name=JetBrains Rider
                                         Icon=/usr/share/jetbrains/{Package.Name()}-{Version}/bin/rider.svg
                                         Exec=/usr/share/jetbrains/{Package.Name()}-{Version}/bin/rider %f
                                         Comment=A cross-platform IDE for .NET
                                         Categories=Development;IDE;
                                         Terminal=false
                                         StartupWMClass=jetbrains-rider
                                         StartupNotify=true
                                         """;
    #endregion
    
    #region constructors
    private Rider()
    {
        InstallSteps = [DownloadAndExtractPackage, InstallExtractedPackage, GenerateDesktopFile];
        ConfigureSteps = [ConfigureRiderRc];
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
            $"https://download.jetbrains.com/rider/JetBrains.Rider-{Version}.tar.gz",
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
    
    private int ConfigureRiderRc()
    {
        Utilities.BashStack.PushBashCommand($"ln -s {Vim.Instance.ConfigFilePath} {ConfigFilePath}");
        
        return 0;
    }
    #endregion
}