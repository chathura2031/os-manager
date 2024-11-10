using ProtoBuf;

namespace OSManager.Plugins.Intercommunication.Commands;

[ProtoContract]
public class ResponseCommand : ICommand
{
    [ProtoMember(1)]
    public string? Command { get; set; }
    
    [ProtoMember(2)]
    public int StatusCode { get; set; }
}