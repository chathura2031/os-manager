using OSManager.Core.Enums;
using ProtoBuf;

namespace OSManager.Communications.Proto.Commands;

[ProtoContract]
public class ConfigureCommand : ICommand
{
    [ProtoMember(1)]
    public Package Package { get; set; }
    
    [ProtoMember(2)]
    public int Stage { get; set; }
    
    [ProtoMember(3)]
    public bool DisconnectAfter { get; set; }
}