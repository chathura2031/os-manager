using ProtoBuf;

namespace OSManager.Core.Commands;

[ProtoContract]
public class ResponseCommand : ICommand
{
    [ProtoMember(1)] public string? Command;
    [ProtoMember(2)] public int StatusCode;
}