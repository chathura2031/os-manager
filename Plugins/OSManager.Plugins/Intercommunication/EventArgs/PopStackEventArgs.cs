using OSManager.Plugins.Intercommunication.Enums;

namespace OSManager.Plugins.Intercommunication.EventArgs;

public class PopStackEventArgs : EventArgs
{
    public int Count;
    public StackType Stack;
}