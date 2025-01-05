using OSManager.Core.Enums;
using OSManager.Core.Extensions;

namespace OSManager.Daemon.Packages;

public class Discord : BasePackage
{
    #region public static members
    public static readonly Discord Instance = new();
    #endregion

    #region public members
    public override Package Package { get; } = Package.Discord;
    public override List<IPackage> Dependencies { get; } = [];
    public override List<IPackage> OptionalExtras { get; } = [];
    #endregion
    
    #region protected members
    protected override List<Func<string, InstallStepReturnData>> InstallSteps { get; }
    protected override List<Func<int>> ConfigureSteps { get; }
    protected override List<Func<int>> BackupConfigurationSteps { get; }
    #endregion
    
    #region ctor
    private Discord()
    {
        InstallSteps = [DownloadAndInstallDebFile, DeleteDebFile];
        ConfigureSteps = [];
        BackupConfigurationSteps = [];
    }
    #endregion

    #region private methods
    private int DownloadDebFile(out string filePath)
    {
        int statusCode = Utilities.DownloadFromUrl("https://discord.com/api/download?platform=linux&format=deb",
            $"{Package.Name()}.deb", out filePath, $"/tmp/osman-{Guid.NewGuid()}");
        
        if (statusCode != 0)
        {
            DeleteDebFile(filePath);
        }
        
        return statusCode;
    }

    private InstallStepReturnData DownloadAndInstallDebFile(string data)
    {
        int statusCode = DownloadDebFile(out string filePath);
        if (statusCode != 0)
        {
            return new InstallStepReturnData(statusCode, [], null);
        }
        
        return new InstallStepReturnData(
            0,
            [() => Utilities.BashStack.PushBashCommand($"apt install -y --fix-broken {filePath}", true)],
            filePath
        );
    }

    private InstallStepReturnData DeleteDebFile(string filePath)
    {
        File.Delete(filePath);
        return new InstallStepReturnData(0, [], null);
    }
    #endregion
}