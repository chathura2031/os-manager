namespace OSManager.CLI;

public class StackManager
{
    public static StackManager Instance { get; set; }
    
    // TODO: Move this somewhere else central
    public static string SlavePath { get; set; }

    public string Path { get; set; }

    public int Count => int.Parse(File.ReadAllLines(Path)[0]);

    public StackManager(string path, bool newStack = true)
    {
        Path = path;
        // Create an empty with an empty command for the next execution
        if (!File.Exists(Path) || newStack)
        {
            File.WriteAllLines(Path, ["0", ""]);
        }
    }

    // private void ModifyStackCount(int count)
    // {
    //     string[] lines = File.ReadAllLines(Path);
    //     lines[0] = count.ToString();
    //     File.WriteAllLines(Path, lines);
    // }

    private string? Peek(out int count)
    {
        count = Count;
        if (count == 0)
        {
            return null;
        }
        
        return File.ReadAllLines(Path)[1];
    }

    private string? Peek()
    {
        return Peek(out _);
    }

    private void ResetHead()
    {
        File.WriteAllLines(Path, ["0", string.Empty]);
    }

    // TODO: Redo stack manager
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
}