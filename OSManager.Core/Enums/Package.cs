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
}