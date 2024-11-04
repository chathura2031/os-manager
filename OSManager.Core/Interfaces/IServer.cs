using OSManager.Core.Commands;

namespace OSManager.Core.Interfaces;

public interface IServer
{
    public int ConnectToServer();

    public int SendCommand(ICommand command);

    public int GetResponse(out ResponseCommand? response);
}