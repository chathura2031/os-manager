using ProtoBuf;

namespace OSManager.Core.Commands;

[ProtoContract]
public class InstallCommand : ICommand
{
    [ProtoMember(1)] public Enums.Package Package;
    [ProtoMember(2)] public int Stage;
    [ProtoMember(3)] public string? Data;
    [ProtoMember(4)] public bool DisconnectAfter;
}