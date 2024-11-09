using OSManager.Plugins.Intercommunication.Commands;
using OSManager.Plugins.Intercommunication.Extensions;

namespace OSManager.Plugins.Intercommunication.Enums;

public enum Command
{
    [Type(typeof(InitialiseCommand))]
    InitialiseCommand,
    
    [Type(typeof(InstallCommand))]
    InstallCommand,
    
    [Type(typeof(PopStackCommand))]
    PopStackCommand,
    
    [Type(typeof(ResponseCommand))]
    ResponseCommand,
    
    [Type(typeof(FinaliseCommand))]
    FinaliseCommand
}