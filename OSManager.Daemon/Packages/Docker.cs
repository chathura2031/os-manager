using OSManager.Core.Enums;
using OSManager.Core.Extensions;

namespace OSManager.Daemon.Packages;

public class Docker : BasePackage
{
    #region public static members
    public static readonly Docker Instance = new();
    #endregion
    
    #region public members

    public override Package Package { get; } = Package.Docker;
    public override List<IPackage> Dependencies { get; } = [];
    public override List<IPackage> OptionalExtras { get; } = [];
    #endregion
    
    #region protected members
    protected override List<Func<string, InstallStepReturnData>> InstallSteps { get; }
    protected override List<Func<int>> ConfigureSteps { get; }
    protected override List<Func<int>> BackupConfigurationSteps { get; }
    #endregion

    #region ctor
    private Docker()
    {
        // Instructions from: https://docs.docker.com/engine/install/debian/#install-from-a-package
        InstallSteps = [
            DownloadAndInstallDeb("containerd.io_1.7.24-1_amd64.deb", "containerd.deb"), DeleteFile,
            DownloadAndInstallDeb("docker-ce-cli_27.4.1-1~debian.12~bookworm_amd64.deb", "docker-ce-cli.deb"), DeleteFile,
            DownloadAndInstallDeb("docker-ce_27.4.1-1~debian.12~bookworm_amd64.deb", "docker-ce.deb"), DeleteFile,
            DownloadAndInstallDeb("docker-buildx-plugin_0.19.3-1~debian.12~bookworm_amd64.deb", "docker-buildx-plugin.deb"), DeleteFile,
            DownloadAndInstallDeb("docker-compose-plugin_2.32.1-1~debian.12~bookworm_amd64.deb", "docker-compose-plugin.deb"), DeleteFile
        ];
        ConfigureSteps = [];
        BackupConfigurationSteps = [];
    }
    #endregion

    #region private methods
    /// <summary>
    /// Get a function to download and install a debian file from the https://download.docker.com/linux/debian/dists/bookworm/pool/stable/amd64/
    /// repository.
    /// </summary>
    /// <param name="onlineFileName">The name of the debian file from the online repository</param>
    /// <param name="downloadFileName">The corresponding name for the downloaded file</param>
    /// <returns></returns>
    private Func<string, InstallStepReturnData> DownloadAndInstallDeb(string onlineFileName, string downloadFileName)
    {
        InstallStepReturnData DownloadAndInstallFunc(string data)
        {
            int statusCode = Utilities.DownloadFromUrl(
                $"https://download.docker.com/linux/debian/dists/bookworm/pool/stable/amd64/{onlineFileName}",
                $"osman-{Package.Name()}-{Guid.NewGuid()}-{downloadFileName}",
                out string filePath,
                $"/tmp"
            );

            return new InstallStepReturnData(
                statusCode,
                [() => Utilities.BashStack.PushBashCommand($"sudo dpkg -i {filePath}", true)],
                filePath
            );
        }

        return DownloadAndInstallFunc;
    }

    private InstallStepReturnData DeleteFile(string filePath)
    {
        File.Delete(filePath);
        return new InstallStepReturnData(0, [], null);
    }
    #endregion
}