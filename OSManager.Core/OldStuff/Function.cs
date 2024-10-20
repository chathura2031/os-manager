namespace OSManager.Core;

/// <summary>
/// Use to store data about a function to run
/// </summary>
/// <param name="function">The function to run</param>
/// <param name="failedExecutionMessage">A message to show if the function fails to execute</param>
public class Function(Func<int> function, string? failedExecutionMessage = null)
{
    public Func<int> Func { get; } = function;
    
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