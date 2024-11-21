using OSManager.Core.Enums;
using OSManager.Core.Extensions;
using OSManager.Daemon.Extensions;

namespace OSManager.Daemon.Packages;

public class Discord : IPackage
{
    public static readonly Discord Instance = new();

    public Package Package { get; } = Package.Discord;
    
    public List<IPackage> Dependencies { get; } = [];
    
    public List<IPackage> OptionalExtras { get; } = [];

    private int DownloadPackage(int verbosity, out string filePath)
    {
        // Download the debian package
        if (verbosity > 0)
        {
            Console.WriteLine("Downloading debian package...");
        }

        int statusCode = Utilities.DownloadFromUrl("https://discord.com/api/download?platform=linux&format=deb",
            $"{Package.Name()}.deb", out filePath, $"/tmp/osman-{Guid.NewGuid()}");

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

    public int Install(int stage, string data)
    {
        int statusCode = 0;
        switch (stage)
        {
            case 1:
                Utilities.BashStack.PushNextStage(stage + 1, Package.Name());
                this.InstallDependencies();
                break;
            case 2:
                statusCode = DownloadPackage(2, out string filePath);
                if (statusCode != 0)
                {
                    Console.WriteLine("Failed to download debian file");
                    File.Delete(filePath);
                    return 1;
                }
                
                Utilities.RunInReverse([
                    () => Utilities.BashStack.PushBashCommand($"apt install -y --fix-broken {filePath}", true),
                    () => Utilities.BashStack.PushNextStage(stage + 1, Package.Name(), filePath),
                ]);
                break;
            case 3:
                statusCode = DeletePackage(2, data);
                if (statusCode != 0)
                {
                    Console.WriteLine("Failed to delete debian file");
                    return 1;
                }
                break;
            default:
                throw new ArgumentException($"{Package.PrettyName()} does not have {stage} stages of installation.");
        }

        return statusCode;
    }
}