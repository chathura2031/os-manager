using ProtoBuf;

namespace OSManager.Shared.Commands;

[ProtoContract]
public class InstallCommand
{
    [ProtoMember(1)] public Packages Package;
    [ProtoMember(2)] public int Stage;
}