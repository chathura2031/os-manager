using OSManager.Core.Enums;
using ProtoBuf;

namespace OSManager.Communications.Proto.Commands;

[ProtoContract]
public class InstallCommand : ICommand
{
    [ProtoMember(1)]
    public Package Package { get; set; }
    
    [ProtoMember(2)]
    public int Stage { get; set; }
    
    [ProtoMember(3)]
    public string? Data { get; set; }
    
    [ProtoMember(4)]
    public bool DisconnectAfter { get; set; }
}