namespace OSManager.CLI;

public interface IStack
{
    public string Path { get; }

    public int Count { get; }

    /// <summary>
    /// Clear the whole stack
    /// </summary>
    public void Clear();

    /// <summary>
    /// View the content at the top of the stack without popping it
    /// </summary>
    /// <returns>The content at the top of the stack</returns>
    public string? Peek();

    /// <summary>
    /// Pop the item at the top of the stack
    /// </summary>
    /// <returns>The content that was at the top of the stack</returns>
    public string? Pop();

    /// <summary>
    /// Push content to the stack
    /// </summary>
    /// <param name="content">The content to add to the stack</param>
    public void Push(string content);
}