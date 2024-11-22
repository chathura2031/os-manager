using OSManager.Core.Enums;

namespace OSManager.Plugins.Intercommunication.EventArgs;

public class ConfigureEventArgs : EventArgs
{
    public Package Package;
    public int Stage;
}