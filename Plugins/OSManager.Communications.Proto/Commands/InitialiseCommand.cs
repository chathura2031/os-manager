using ProtoBuf;

namespace OSManager.Communications.Proto.Commands;

[ProtoContract]
public class InitialiseCommand : ICommand
{
    [ProtoMember(1)]
    public string SlavePath { get; set; }
    
    [ProtoMember(2)]
    public string BaseStackPath { get; set; }
    
    [ProtoMember(3)]
    public string? WorkingDirectoryPath { get; set; }
    
    [ProtoMember(4)]
    public bool DisconnectAfter { get; set; }
}