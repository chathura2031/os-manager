namespace OSManager.CLI;

public static class StackExtensions
{
    /// <summary>
    /// Push a bash command to the stack
    /// </summary>
    /// <param name="stack">A reference to a stack</param>
    /// <param name="command">The bash command to add to the stack</param>
    /// <param name="useSudo">True if the command should use sudo, False otherwise</param>
    public static void PushBashCommand(this IStack stack, string command, bool useSudo = false)
    {
        string bashCommand = useSudo ? "sudo " : "";
        bashCommand += $"{command} && ./{Utilities.SlavePath} popstack --stack {Utilities.BaseStackPath} --count 1";
        stack.Push(bashCommand);
    }

    /// <summary>
    /// Push a bash command to execute a specific stage of this project
    /// </summary>
    /// <param name="stack">A reference to a stack</param>
    /// <param name="stage">The installation stage to run</param>
    /// <param name="pathSafeName">The package's path safe name</param>
    /// <param name="dataPath">The path to any data required (optional)</param>
    /// <param name="adminAccess">True if the program needs admin access, False otherwise</param>
    public static void PushNextStage(this IStack stack, int stage, string pathSafeName, string? dataPath = null, bool adminAccess = false)
    {
        string content = adminAccess ? "sudo " : "";
        content += $"./{Utilities.SlavePath} continue --stack {Utilities.BaseStackPath} --slave {Utilities.SlavePath} --stage {stage} --package {pathSafeName}";
        
        if (dataPath != null)
        {
            content += $" --data {dataPath}";
        }
        
        stack.Push(content);
    }
}