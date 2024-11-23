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
        string command = $"(dpkg -l {packageName} &> {Utilities.ProgramStack.Path}.tmp; ./{Utilities.SlavePath} pushstack --file {Utilities.ProgramStack.Path}.tmp";
        PushBashCommand(stack, command, false);
    }

    /// <summary>
    /// Push a bash command to install a specific package
    /// </summary>
    /// <param name="stack">A reference to a stack</param>
    /// <param name="stage">The installation stage to run</param>
    /// <param name="packageName">The package's name</param>
    /// <param name="dataPath">The path to any data required (optional)</param>
    public static void PushInstallStage(this FatStack stack, int stage, string packageName, string? dataPath = null)
    {
        string content = $"./{Utilities.SlavePath} install --stage {stage} --package {packageName}";
        
        if (dataPath != null)
        {
            content += $" --data {dataPath}";
        }
        
        stack.Push(content);
    }

    /// <summary>
    /// Push a bash command to configure a specific package
    /// </summary>
    /// <param name="stack">A reference to a stack</param>
    /// <param name="stage">The configuration stage to run</param>
    /// <param name="packageName">The package's name</param>
    public static void PushConfigureStage(this FatStack stack, int stage, string packageName)
    {
        string content = $"./{Utilities.SlavePath} configure --stage {stage} --package {packageName}";
        
        stack.Push(content);
    }
    
    /// <summary>
    /// Push a bash command to back the configuration of a specific package
    /// </summary>
    /// <param name="stack">A reference to a stack</param>
    /// <param name="stage">The backup configuration stage to run</param>
    /// <param name="packageName">The package's name</param>
    public static void PushBackupConfigStage(this FatStack stack, int stage, string packageName)
    {
        string content = $"./{Utilities.SlavePath} backupconfig --stage {stage} --package {packageName}";
        
        stack.Push(content);
    }
}