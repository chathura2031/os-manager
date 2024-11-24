using OSManager.Core.Enums;
using OSManager.Core.Extensions;

namespace OSManager.Daemon.Packages;

public class LocalNugetRepo : IPackage
{
    public static readonly LocalNugetRepo Instance = new();

    public Package Package { get; } = Package.LocalNugetRepo;

    public List<IPackage> Dependencies { get; } = [];

    public List<IPackage> OptionalExtras { get; } = [];
    
    public int Install(int stage, string data)
    {
        int statusCode = 0;
        switch (stage)
        {
            case 1:
            {
                // TODO:
                // `dotnet` binary location: ~/.dotnet/dotnet
                //  - Create repository folder: `mkdir $HOME/.nuget/local-packages`
                //  - Replace the `~/.nuget/Nuget/Nuget.config` file
                //  - Download all nuget packages to $HOME/.nuget/local-packages
                break;
            }
            default:
                throw new ArgumentException($"{Package.PrettyName()} does not have {stage} stages of installation.");
        }

        return statusCode;
    }

    public int Configure(int stage)
    {
        return 0;
    }

    public int BackupConfig(int stage)
    {
        return 0;
    }
}