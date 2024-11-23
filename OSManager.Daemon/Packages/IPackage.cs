using OSManager.Core.Enums;

namespace OSManager.Daemon.Packages;

public interface IPackage
{
    Package Package { get; }
    
    List<IPackage> Dependencies { get; }
    
    List<IPackage> OptionalExtras { get; }

    public int Install(int stage, string data);

    public int Configure(int stage);
    
    public int BackupConfig(int stage);
}