using OSManager.Communications.Proto.Commands;
using OSManager.Communications.Proto.Extensions;

namespace OSManager.Communications.Proto.Enums;

public enum Command
{
    [Type(typeof(InitialiseCommand))]
    InitialiseCommand,
    
    [Type(typeof(InstallCommand))]
    InstallCommand,
    
    [Type(typeof(ConfigureCommand))]
    ConfigureCommand,
    
    [Type(typeof(PopStackCommand))]
    PopStackCommand,
    
    [Type(typeof(PushStackCommand))]
    PushStackCommand,
    
    [Type(typeof(ResponseCommand))]
    ResponseCommand,
    
    [Type(typeof(FinaliseCommand))]
    FinaliseCommand
}