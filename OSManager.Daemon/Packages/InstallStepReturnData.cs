namespace OSManager.Daemon.Packages;

public class InstallStepReturnData(int statusCode, Action[] bashCommands, string? outgoingData)
{
    public int StatusCode { get; set; } = statusCode;
    public Action[] BashCommands { get; set; } = bashCommands;
    public string? OutgoingData { get; set; } = outgoingData;
}