using ProtoBuf;

namespace OSManager.Communications.Proto.Commands;

[ProtoContract]
public class PushStackCommand : ICommand
{
    [ProtoMember(1)]
    public string[] Content { get; set; }
    
    [ProtoMember(2)]
    public bool DisconnectAfter { get; set; }
}