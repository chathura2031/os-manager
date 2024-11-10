using OSManager.Plugins.Intercommunication.Enums;

namespace OSManager.Plugins.Intercommunication.EventArgs;

public class InstallEventArgs : EventArgs
{
    public Package Package;
    public int Stage;
    public string? Data;
}