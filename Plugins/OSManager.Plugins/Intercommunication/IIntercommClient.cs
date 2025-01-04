using OSManager.Core.Enums;
using OSManager.Plugins.Intercommunication.Enums;

namespace OSManager.Plugins.Intercommunication;

public interface IIntercommClient
{
    public int ConnectToServer(string sessionId, string slavePath, string? workingDirectory = null);

    public int Install(Package package, int stage, string? data = null, bool popBeforeExecution = false);
    
    public int Configure(Package package, int stage);
    
    public int BackupConfig(Package package, int stage);
    
    public int PopStack(int count, StackType stack);
    
    public int PushStack(string[] content);

    public int DisconnectFromServer();
}