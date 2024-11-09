using OSManager.Plugins.Intercommunication.Commands;

namespace OSManager.Plugins.Intercommunication;

public interface IIntercommServer
{
    public int ConnectToServer();

    public int SendCommand(ICommand command);

    public int GetResponse(out IResponseCommand? response);
}