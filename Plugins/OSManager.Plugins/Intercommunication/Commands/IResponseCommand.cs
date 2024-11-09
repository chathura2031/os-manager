namespace OSManager.Plugins.Intercommunication.Commands;

// TODO: Move all references to this interface
public interface IResponseCommand
{
    public string? Command { get; set; }
    public int StatusCode { get; set; }
}