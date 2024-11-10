using OSManager.Communications.Proto.Extensions;
using OSManager.Plugins.Intercommunication.Commands;

namespace OSManager.Communications.Proto.Enums;

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