using ProtoBuf;

namespace OSManager.Plugins.Intercommunication.Commands;

[ProtoContract]
public class InitialiseCommand : IInitialiseCommand
{
    [ProtoMember(1)]
    public string SlavePath { get; set; }
    
    [ProtoMember(2)]
    public string BaseStackPath { get; set; }
}