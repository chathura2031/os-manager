using OSManager.Plugins.Intercommunication.Enums;

namespace OSManager.Plugins.Intercommunication.Commands;

// TODO: Move all references to this interface
public interface IInstallCommand : ICommand
{
    public Package Package { get; set; }
    public int Stage { get; set; }
    public string? Data { get; set; }
    public bool DisconnectAfter { get; set; }
}