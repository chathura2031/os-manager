using OSManager.Core.Commands;
using OSManager.Core.Extensions;

namespace OSManager.Core.Enums;

public enum Command
{
    [Type(typeof(InitialiseCommand))]
    InitialiseCommand,
    
    [Type(typeof(InstallCommand))]
    InstallCommand,
    
    [Type(typeof(PopStackCommand))]
    PopStackCommand,
    
    [Type(typeof(DisconnectCommand))]
    DisconnectCommand,
    
    [Type(typeof(ResponseCommand))]
    ResponseCommand,
    
    [Type(typeof(FinaliseCommand))]
    FinaliseCommand
}