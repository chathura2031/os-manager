using OSManager.Core.Enums;
using OSManager.Plugins.Intercommunication.Enums;

namespace OSManager.Plugins.Intercommunication;

public interface IIntercommClient
{
    public int ConnectToServer(string sessionId, string slavePath);

    public int Install(Package package, int stage, string? data = null);
    
    public int PopStack(int count, StackType stack);
    
    public int PushStack(string[] content);

    public int DisconnectFromServer();
}