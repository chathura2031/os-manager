using ProtoBuf;

namespace OSManager.Communications.Proto.Commands;

[ProtoContract]
public class PopStackCommand : ICommand
{
    [ProtoMember(1)]
    public int Count { get; set; }
    
    [ProtoMember(2)]
    public bool DisconnectAfter { get; set; }
}