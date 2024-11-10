using OSManager.Plugins.Intercommunication.Commands;
using OSManager.Plugins.Intercommunication.Enums;

namespace OSManager.Plugins.Intercommunication;

public interface IIntercommClient
{
    public int ConnectToServer(string sessionId, string slavePath);

    public int Install(Package package, int stage, string? data = null);
    
    public int PopStack(int count);

    public int DisconnectFromServer();
}