namespace OSManager.CLI;

public static class Utilities
{
    public static string SlavePath { get; set; } = null!;
    
    public static string BaseStackPath { get; private set; }
    
    // A stack that the master agent (the bash client) will read from
    public static IStack BashStack { get; private set; }
    
    // A stack that the slave agent (the program) will read from
    public static IStack ProgramStack { get; private set; }
    
    /// <summary>
    /// Prompt the user for a yes or no answer
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static bool ShowYesOrNoPrompt(string message)
    {
        while (true)
        {
            Console.Write($"{message} (Y/n) ");
            string? input = Console.ReadLine()?.Trim().ToLower();
            
            if (input is "y" or "n")
            {
                return input == "y";
            }

            Console.WriteLine("Invalid input. Please try again.\n");
        }
    }
    
    /// <summary>
    /// Download a file from a given URL
    /// </summary>
    /// <param name="url">The URL to download from</param>
    /// <param name="fileName">The desired file name</param>
    /// <param name="filePath">A variable to store the downloaded file path into</param>
    /// <param name="downloadDirectory">The directory to download the file to. Will download to current directory if not
    ///     provided.</param>
    /// <param name="promptReplacement">True if a prompt should be shown if the file already exists, False to replace it without a prompt</param>
    /// <returns>A status code</returns>
    public static int DownloadFromUrl(string url, string fileName, out string filePath, string? downloadDirectory = null, bool promptReplacement = false)
    {
        downloadDirectory ??= Directory.GetCurrentDirectory();
        filePath = Path.Join(downloadDirectory, fileName);
        bool cancelOperation = false;
        
        if (promptReplacement && File.Exists(filePath))
        {
            cancelOperation = !ShowYesOrNoPrompt($"{filePath} already exists. Would you like to replace it?");
        }

        if (!cancelOperation)
        {
            File.Delete(filePath);
            
            using HttpClient client = new();
            using Task<Stream> stream = client.GetStreamAsync(url);
            using FileStream fs = new(filePath, FileMode.CreateNew);
            try
            {
                stream.Result.CopyTo(fs);
            }
            catch (Exception)
            {
                Console.WriteLine($"Failed to download file from {url}");
                return 1;
            }
        }

        return cancelOperation ? 1 : 0;
    }

    /// <summary>
    /// Run a chain of functions in the reverse order that they are passed in
    /// </summary>
    /// <param name="actions">A list of functions to run</param>
    public static void RunInReverse(Action[] actions)
    {
        for (int i = actions.Length - 1; i >= 0; i--)
        {
            actions[i]();
        }
    }

    public static void GetOrCreateStacks(string baseStackPath, bool newStack = false)
    {
        BaseStackPath = baseStackPath;
        Utilities.BashStack = new FatStack($"{BaseStackPath}.bash", newStack);
        Utilities.ProgramStack = new ThinStack($"{BaseStackPath}.cli", newStack);
    }
}