using ProtoBuf;

namespace OSManager.Shared.Commands;

[ProtoContract]
public class ResponseCommand
{
    [ProtoMember(1)] public string? Command;
    [ProtoMember(2)] public int StatusCode;
}