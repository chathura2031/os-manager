namespace OSManager.Core;

/// <summary>
/// Use to store data about a command to run
/// </summary>
/// <param name="fileName">The path to the command (e.g. /usr/bin/sudo)</param>
/// <param name="arguments">The arguments to pass in</param>
/// <param name="failedExecutionMessage">A message to show if the command fails to execute</param>
public class Command(string fileName, string arguments, string? failedExecutionMessage = null)
{
    public string FileName { get; } = fileName;
    public string Arguments { get; } = arguments;
    public string? FailedExecutionMessage { get; } = failedExecutionMessage;

    /// <summary>
    /// Log a message to the console
    /// </summary>
    public void LogFailedExecutionMessage()
    {
        if (FailedExecutionMessage != null)
        {
            Console.WriteLine(FailedExecutionMessage);
        }
    }
}