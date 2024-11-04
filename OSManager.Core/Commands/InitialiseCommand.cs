using ProtoBuf;

namespace OSManager.Core.Commands;

[ProtoContract]
public class InitialiseCommand : ICommand
{
    [ProtoMember(1)] public string SlavePath;
    [ProtoMember(2)] public string BaseStackPath;
}