namespace OSManager.Plugins.Intercommunication.Commands;

// TODO: Move all references to this interface
public interface IPopStackCommand : ICommand
{
    public int Count { get; set; }
    public bool DisconnectAfter { get; set; }
}