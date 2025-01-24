using OSManager.Core.Attributes;

namespace OSManager.Core.Enums;

// TODO: Move this to the plugins folder
public enum Package
{
    // ReSharper disable once InconsistentNaming
    [Name("@internal:dependencies"), PrettyName("Install Dependencies")]
    INTERNAL_PackageDependencies,
    
    // ReSharper disable once InconsistentNaming
    [Name("@internal:updateandupgrade"), PrettyName("Update and Upgrade")]
    INTERNAL_UpdateAndUpgrade,
    
    [Name("discord"), PrettyName("Discord")]
    Discord,
    
    [Name("vim"), PrettyName("Vim")]
    Vim,
    
    [Name("google-chrome"), PrettyName("Chrome")]
    Chrome,
    
    [Name("network-drives"), PrettyName("Network Drives")]
    NetworkDrives,
    
    [Name("local-nuget-repo"), PrettyName("Local Nuget Repo")]
    LocalNugetRepo,
    
    [Name("yakuake"), PrettyName("Yakuake")]
    Yakuake,
    
    [Name("qdbus"), PrettyName("QDBus")]
    Qdbus,
    
    [Name("docker"), PrettyName("Docker")]
    Docker,
    
    [Name("codium"), PrettyName("Codium")]
    Codium,
    
    [Name("rider"), PrettyName("Rider")]
    Rider,
    
    [Name("qalculate"), PrettyName("Qalculate")]
    Qalculate,
}