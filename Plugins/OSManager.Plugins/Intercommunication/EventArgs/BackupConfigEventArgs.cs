using OSManager.Core.Enums;

namespace OSManager.Plugins.Intercommunication.EventArgs;

public class BackupConfigEventArgs : EventArgs
{
    public Package Package;
    public int Stage;
}