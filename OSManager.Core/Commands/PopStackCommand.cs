using ProtoBuf;

namespace OSManager.Core.Commands;

[ProtoContract]
public class PopStackCommand : ICommand
{
    [ProtoMember(1)] public int Count;
    [ProtoMember(2)] public bool DisconnectAfter;
}