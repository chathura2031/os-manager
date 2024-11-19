using PersistentTools.Stack;

namespace OSManager.Daemon;

public static class StackExtensions
{
    /// <summary>
    /// Push a bash command to the stack
    /// </summary>
    /// <param name="stack">A reference to a stack</param>
    /// <param name="command">The bash command to add to the stack</param>
    /// <param name="useSudo">True if the command should use sudo, False otherwise</param>
    public static void PushBashCommand(this FatStack stack, string command, bool useSudo = false)
    {
        string bashCommand = useSudo ? "sudo " : "";
        bashCommand += $"{command} && ./{Utilities.SlavePath} popstack --stack bash --count 1";
        stack.Push(bashCommand);
    }

    /// <summary>
    /// Push a bash command to check if a package exists
    /// </summary>
    /// <param name="stack">A reference to a stack</param>
    /// <param name="packageName">The name of the package to check for</param>
    public static void PushPackageExistsCommand(this FatStack stack, string packageName)
    {
        string command = $"(dpkg -l {packageName} &> {Utilities.ProgramStack.Path}.tmp; ./{Utilities.SlavePath} pushstack --file {Utilities.ProgramStack.Path}";
        PushBashCommand(stack, command, false);
    }

    /// <summary>
    /// Push a bash command to execute a specific stage of this project
    /// </summary>
    /// <param name="stack">A reference to a stack</param>
    /// <param name="stage">The installation stage to run</param>
    /// <param name="packageName">The package's name</param>
    /// <param name="dataPath">The path to any data required (optional)</param>
    /// <param name="adminAccess">True if the program needs admin access, False otherwise</param>
    public static void PushNextStage(this FatStack stack, int stage, string packageName, string? dataPath = null, bool adminAccess = false)
    {
        string content = adminAccess ? "sudo " : "";
        content += $"./{Utilities.SlavePath} continue --slave {Utilities.SlavePath} --stage {stage} --package {packageName}";
        
        if (dataPath != null)
        {
            content += $" --data {dataPath}";
        }
        
        stack.Push(content);
    }
}