namespace OSManager.CLI;

/// <summary>
/// A stack where the head file only has the count
/// </summary>
public class ThinStack : IStack
{
    public string Path { get; }

    public int Count => int.Parse(File.ReadAllLines(Path)[0]);
    
    public ThinStack(string path, bool newStack = false)
    {
        Path = path;
        // Create an empty stack
        if (!File.Exists(Path) || newStack)
        {
            ModifyCount(0);
        }
    }

    /// <summary>
    /// Modify the stack count
    /// </summary>
    /// <param name="count">The new count</param>
    private void ModifyCount(int count)
    {
        File.WriteAllLines(Path, [count.ToString()]);
    }
    
    /// <summary>
    /// Clear the whole stack
    /// </summary>
    public void Clear()
    {
        int count = Count;
        for (int i = 0; i < count; i++)
        {
            File.Delete($"{Path}.{i}");
        }
        ModifyCount(0);
    }

    /// <summary>
    /// View the content at the top of the stack without popping it
    /// </summary>
    /// <param name="count">A variable to store the current stack count</param>
    /// <param name="nodePath">A variable to store the path to the node at the top</param>
    /// <returns>The content at the top of the stack</returns>
    private string? Peek(out int count, out string? nodePath)
    {
        count = Count;
        if (count == 0)
        {
            nodePath = null;
            return null;
        }

        nodePath = $"{Path}.{count - 1}";
        return File.ReadAllText(nodePath);
    }

    /// <summary>
    /// View the content at the top of the stack without popping it
    /// </summary>
    /// <returns>The content at the top of the stack</returns>
    public string? Peek()
    {
        return Peek(out _, out _);
    }
    
    /// <summary>
    /// Pop the item at the top of the stack
    /// </summary>
    /// <returns>The content that was at the top of the stack</returns>
    public string? Pop()
    {
        string? content = Peek(out int count, out string? nodePath);
        if (count == 0)
        {
            return null;
        }
        
        count--;
        File.Delete(nodePath!);
        ModifyCount(count);
        
        return content;
    }
    
    /// <summary>
    /// Push content to the stack
    /// </summary>
    /// <param name="content">The content to add to the stack</param>
    public void Push(string content)
    {
        int count = Count;
        File.WriteAllLines($"{Path}.{count}", [content]);

        count++;
        ModifyCount(count);
    }
}