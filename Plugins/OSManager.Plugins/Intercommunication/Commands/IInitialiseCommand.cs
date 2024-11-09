namespace OSManager.Plugins.Intercommunication.Commands;

public interface IInitialiseCommand : ICommand
{
    public string SlavePath { get; set; }
    public string BaseStackPath { get; set; }
}