using ProtoBuf;

namespace OSManager.Shared.Commands;

[ProtoContract]
public class InitialiseCommand
{
    [ProtoMember(1)] public string SlavePath;
    [ProtoMember(2)] public string BaseStackPath;
}