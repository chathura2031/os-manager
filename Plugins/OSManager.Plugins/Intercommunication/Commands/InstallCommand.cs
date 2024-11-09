using OSManager.Plugins.Intercommunication.Enums;
using ProtoBuf;

namespace OSManager.Plugins.Intercommunication.Commands;

[ProtoContract]
public class InstallCommand : IInstallCommand
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