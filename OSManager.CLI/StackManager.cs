namespace OSManager.CLI;

public class StackManager
{
    public static StackManager Instance { get; set; }
    
    public string Path { get; set; }

    public int Count => int.Parse(File.ReadAllLines(Path)[0]);

    public StackManager(string path, bool newStack = true)
    {
        Path = path;
        // Create an empty stack
        if (!File.Exists(Path) || newStack)
        {
            File.WriteAllLines(Path, ["0", ""]);
        }
    }

    /// <summary>
    /// View the content at the top of the stack without popping it
    /// </summary>
    /// <param name="count">A variable to store the current stack count</param>
    /// <returns>The content at the top of the stack</returns>
    private string? Peek(out int count)
    {
        count = Count;
        if (count == 0)
        {
            return null;
        }
        
        return File.ReadAllLines(Path)[1];
    }

    /// <summary>
    /// View the content at the top of the stack without popping it
    /// </summary>
    /// <returns>The content at the top of the stack</returns>
    public string? Peek()
    {
        return Peek(out _);
    }

    /// <summary>
    /// Reset the stack head
    /// </summary>
    private void ResetHead()
    {
        File.WriteAllLines(Path, ["0", string.Empty]);
    }

    /// <summary>
    /// Pop the item at the top of the stack
    /// </summary>
    /// <returns>The content that was at the top of the stack</returns>
    public string? Pop()
    {
        string? content = Peek(out int count);
        if (count == 0)
        {
            return null;
        }
        else if (count == 1)
        {
            ResetHead();
            return content;
        }
        
        count--;
        string nextStackPath = $"{Path}.{count-1}";
        string newContent = File.ReadAllLines(nextStackPath)[0];
        File.Delete(nextStackPath);
        File.WriteAllLines(Path, [count.ToString(), newContent]);
        
        return content;
    }

    /// <summary>
    /// Clear the whole stack
    /// </summary>
    public void Clear()
    {
        int count = Count;
        for (int i = 0; i < count - 1; i++)
        {
            File.Delete($"{Path}.{i}");
        }
        ResetHead();
    }

    /// <summary>
    /// Push content to the stack
    /// </summary>
    /// <param name="content">The content to add to the stack</param>
    public void Push(string content)
    {
        string[] lines = File.ReadAllLines(Path);
        int count = int.Parse(lines[0]);

        if (count > 0)
        {
            string filePath = $"{Path}.{count-1}";
            File.WriteAllLines(filePath, [lines[1]]);
        }
        count++;
        
        lines = [count.ToString(), content];
        File.WriteAllLines(Path, lines);
    }

    /// <summary>
    /// Push a bash command to the stack
    /// </summary>
    /// <param name="command">The bash command to add to the stack</param>
    public void PushBashCommand(string command)
    {
        Push($"{command} && ./{Utilities.SlavePath} popstack --stack {Path} --count 1");
    }

    /// <summary>
    /// Push a bash command to execute a specific stage of this project
    /// </summary>
    /// <param name="stage">The installation stage to run</param>
    /// <param name="pathSafeName">The package's path safe name</param>
    /// <param name="dataPath">The path to any data required (optional)</param>
    public void PushNextStage(int stage, string pathSafeName, string? dataPath = null)
    {
        string content = $"./{Utilities.SlavePath} continue --stack {Path} --slave {Utilities.SlavePath} --stage {stage} --package {pathSafeName}";
        if (dataPath != null)
        {
            content += $" --data {dataPath}";
        }
        Push(content);
    }
}