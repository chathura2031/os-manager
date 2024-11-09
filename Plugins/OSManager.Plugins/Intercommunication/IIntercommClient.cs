using OSManager.Plugins.Intercommunication.Interfaces;

namespace OSManager.Plugins.Intercommunication;

public interface IIntercommClient
{
    public bool IsConnected { get; }
    
    public Task WaitForClient();
    
    public Task<ICommand> ReceiveCommand();

    public Task SendResponse(int statusCode);

    public Task DisconnectFromClient();
}