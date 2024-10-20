using ProtoBuf;

namespace OSManager.Shared.Commands;

[ProtoContract]
public class PopStackCommand
{
    [ProtoMember(1)] public int Count = 1;
}