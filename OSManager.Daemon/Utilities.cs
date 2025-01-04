using System.Text.Json.Nodes;
using PersistentTools.Stack;

namespace OSManager.Daemon;

public static class Utilities
{
    private static string? _workingDirectory;

    public static string HomeDirectory { get; } = Environment.GetEnvironmentVariable("HOME") ?? throw new NotImplementedException();
    
    public static string ConfigDirectory { get; } = Path.Join(HomeDirectory, ".config");
    
    public static string WorkingDirectory
    {
        get => _workingDirectory ?? Environment.CurrentDirectory;
        set => _workingDirectory = value;
    } 
    
    public static string BackupDirectory => Path.Join(WorkingDirectory, "os-config");
    
    public static string EncryptedBackupDirectory => Path.Join(WorkingDirectory, "os-config-secrets");
    
    public static string EncryptorPath => Path.Join(EncryptedBackupDirectory, "file-encryptor", "Encryption.FileEncryptor");
    
    public static string BaseStackPath { get; private set; }
    
    // A stack that the master agent (the bash client) will read from
    public static FatStack BashStack { get; private set; }
    
    // A stack that the slave agent (the program) will read from
    public static ThinStack ProgramStack { get; private set; }
    
    public static string SlavePath { get; set; } = null!;
    
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
        if (downloadDirectory != null)
        {
            Directory.CreateDirectory(downloadDirectory);
        }
        
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
    
    /// <summary>
    /// Instantiate the bash and program stacks creating them if they don't exist
    /// </summary>
    /// <param name="baseStackPath">The base stack path</param>
    /// <param name="newStack">True if a new stack should be created, False if one should only be created if it doesn't exist</param>
    public static void GetOrCreateStacks(string baseStackPath, bool newStack = false)
    {
        BaseStackPath = baseStackPath;
        Utilities.BashStack = new FatStack($"{BaseStackPath}.bash", newStack);
        Utilities.ProgramStack = new ThinStack($"{BaseStackPath}.cli", newStack);
    }
    
    /// <summary>
    /// Delete the bash and program stacks
    /// </summary>
    public static int DeleteStacks()
    {
        int statusCode = 0;
        if (Utilities.BashStack.Count > 0)
        {
#if debug
            throw new InvalidOperationException("Failed to terminate program, requested sudden deletion of non-empty bash stack.");
#else
            Console.WriteLine("WARNING: Requested sudden deletion of non-empty bash stack.");
            statusCode = 1;
            Utilities.BashStack.Clear();
#endif
        }
        
        File.Delete(Utilities.BashStack.Path);
        
        if (Utilities.ProgramStack.Count > 0)
        {
#if debug
            throw new InvalidOperationException("Failed to terminate program, requested sudden deletion of non-empty program stack.");
#else
            Console.WriteLine("WARNING: Requested sudden deletion of non-empty program stack.");
            statusCode = 1;
            Utilities.ProgramStack.Clear();
#endif
        }
        File.Delete(Utilities.ProgramStack.Path);

        return statusCode;
    }
    
    /// <summary>
    /// Copy a file to a new location
    /// </summary>
    /// <param name="sourcePath">The path to the file to copy</param>
    /// <param name="destinationPath">The path to the file at the destination</param>
    /// <param name="replaceFile">True if the file should be replaced if it already exists, False otherwise</param>
    /// <returns>A status code</returns>
    public static int CopyFile(string sourcePath, string destinationPath, bool replaceFile = true)
    {
        if (!File.Exists(sourcePath))
        {
            Console.WriteLine($"{sourcePath} does not exist");
            return 1;
        }

        if (File.Exists(destinationPath))
        {
            if (!replaceFile)
            {
                return 1;
            }
            
            File.Delete(destinationPath);
        }
        
        try
        {
            // Create the parent destination directory if it doesn't exist
            string? parentDir = Path.GetDirectoryName(destinationPath)!;
            Directory.CreateDirectory(parentDir);
            // Copy the file
            File.Copy(sourcePath, destinationPath);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            return 1;
        }

        return 0;
    }

    /// <summary>
    /// Copy a directory to a new location and if specified, subdirectories
    /// </summary>
    /// <param name="sourcePath">The path to the directory to copy</param>
    /// <param name="destinationPath">The path to the directory at the destination</param>
    /// <param name="replaceFiles">True if the files should be replaced if it already exists, False otherwise</param>
    /// <param name="extExclusions">File extensions to not copy</param>
    /// <param name="fileNameExclusions">File names to not copy (must include file extension)</param>
    /// <returns>A status code</returns>
    public static int CopyDirectory(string sourcePath, string destinationPath, bool replaceFiles = false,
        HashSet<string>? extExclusions = null, HashSet<string>? fileNameExclusions = null)
    {
        extExclusions ??= [];
        fileNameExclusions ??= [];
        
        // Get information about the source directory
        var dir = new DirectoryInfo(sourcePath);

        // Check if the source directory existetermine which extension folders to copy
        // string[] extensions = es
        if (!dir.Exists)
        {
            Console.WriteLine($"{sourcePath} does not exist");
            return 1;
        }

        // Cache directories before we start copying
        DirectoryInfo[] dirs = dir.GetDirectories();

        // Create the destination directory
        Directory.CreateDirectory(destinationPath);

        // Get the files in the source directory and copy to the destination directory
        foreach (FileInfo file in dir.GetFiles())
        {
            if (extExclusions.Contains(file.Extension) || fileNameExclusions.Contains(file.Name))
            {
                continue;
            }
            
            string targetFilePath = Path.Join(destinationPath, file.Name);
            CopyFile(file.FullName, targetFilePath, replaceFiles);
        }

        // TODO: Implement this iteratively
        // TODO: Test this
        // If recursive and copying subdirectories, recursively call this method
        foreach (DirectoryInfo subDir in dirs)
        {
            string newDestinationDir = Path.Join(destinationPath, subDir.Name);
            if (!File.Exists(newDestinationDir) || replaceFiles)
            {
                CopyDirectory(subDir.FullName, newDestinationDir, replaceFiles, extExclusions, fileNameExclusions);
            }
        }

        return 0;
    }

    /// <summary>
    /// Replace the value for a given key in the original with the updated value
    /// </summary>
    /// <param name="original">The original JSON to replace</param>
    /// <param name="updated">The updated JSON to get values from</param>
    /// <param name="key">The key to replace</param>
    public static void UpdateJson(ref JsonObject original, JsonObject updated, string key)
    {
        if (!original.ContainsKey(key) && !updated.ContainsKey(key))
        {
            return;
        }

        original.Remove(key);
        
        if (original.ContainsKey(key) && !updated.ContainsKey(key))
        {
            return;
        }

        original[key] = updated[key]!.DeepClone();
    }

    /// <summary>
    /// Replace the value for a given set of keys in the original with the updated value
    /// </summary>
    /// <param name="original">The original JSON to replace</param>
    /// <param name="updated">The updated JSON to get values from</param>
    /// <param name="keys">The keys to replace. These can be nested keys
    ///     eg. to replace keys ["a", "b" -> ["c", "d"]], you can pass
    ///     in ["a", new object[] {"b", new object[] {"c", "d"}}]</param>
    /// <exception cref="NotImplementedException"></exception>
    public static void UpdateJson(ref JsonObject original, JsonObject updated, object[] keys)
    {
        Stack<Tuple<JsonObject, JsonObject, object>> nodes = new();
        foreach (object key in keys)
        {
            nodes.Push(new(original, updated, key));
        }

        while (nodes.Count > 0)
        {
            var (jsonA, jsonB, obj) = nodes.Pop();

            if (obj is string s)
            {
                UpdateJson(ref jsonA, jsonB, s);
            }
            else if (obj is object[] o)
            {
                string key = (string)o[0];
                foreach (object subObj in (object[])o[1])
                {
                    bool keyExistsInA = jsonA.TryGetPropertyValue(key, out var jsonAChildNode);
                    bool keyExistsInB = jsonB.TryGetPropertyValue(key, out var jsonBChildNode);
                    
                    if (!keyExistsInB)
                    {
                        continue;
                    }
                    
                    if (!keyExistsInA && keyExistsInB)
                    {
                        UpdateJson(ref jsonA, jsonB, key);
                    }
                    else
                    {
                        nodes.Push(new(jsonA[key]!.AsObject(), jsonB[key]!.AsObject(), subObj));
                    }
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }

    /// <summary>
    /// Replace the content between 2 block start and end comments with new lines as specified and generate a new file.
    /// If the content doesn't exist, it will be added to the end of the file.
    /// </summary>
    /// <param name="filePath">The path to the file with the content to replace</param>
    /// <param name="modifiedFilePath">The path to the new file with the content replaced</param>
    /// <param name="linesToAdd">The lines to add. The first and last line must match the block start and end comments</param>
    /// <param name="blockStartComment">The comment line which indicates the start of the block</param>
    /// <param name="blockEndComment">The comment line which indicates the end of the block</param>
    /// <exception cref="Exception">Thrown if the lines to add are invalid</exception>
    public static void ReplaceContentBlockInFile(string filePath, out string modifiedFilePath,
        IEnumerable<string> linesToAdd, string blockStartComment, string blockEndComment)
    {
        if (linesToAdd.Count() < 2)
        {
            throw new Exception("There must be at least 2 lines of content to add -- lines of comments indicating the start and end of the block");
        }
        
        string blockFirstLine = linesToAdd.First().Trim();
        string blockLastLine = linesToAdd.Last().Trim();
        if (blockFirstLine.ToLower() != blockStartComment.ToLower())
        {
            throw new Exception($"The first line must be '{blockStartComment}' (case insensitive)");
        }
        else if (blockLastLine.ToLower() != blockEndComment.ToLower())
        {
            throw new Exception($"The last line must be '{blockEndComment}' (case insensitive)");
        }

        void AddCustomEntries(StreamWriter writer, bool addSpaceAbove)
        {
            if (addSpaceAbove)
            {
                writer.WriteLine();
            }
            
            foreach (string lineToAdd in linesToAdd)
            {
                writer.WriteLine(lineToAdd);
            }
        }

        modifiedFilePath = Path.Join("/tmp", $"osman-{Guid.NewGuid()}.tmp");
        // Create a temporary file with the contents of the fstab file but adding in (or replacing) the custom entries
        using (StreamReader reader = new StreamReader(filePath))
        using (StreamWriter writer = new StreamWriter(modifiedFilePath))
        {
            bool customEntriesAdded = false;
            bool startReplacement = false;
            bool addSpaceAbove = false;
            string? lastLine = null;
            
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine()!;
                if (!startReplacement && line.Trim().ToLower() == blockStartComment.ToLower())
                {
                    startReplacement = true;
                    // Trigger a new line before the replacement content
                    if (lastLine.Trim() != "")
                    {
                        addSpaceAbove = true;
                    }
                }

                if (!startReplacement)
                {
                    writer.WriteLine(line);
                }
                else if (startReplacement && line.Trim().ToLower() == blockEndComment.ToLower())
                {
                    AddCustomEntries(writer, addSpaceAbove);
                    customEntriesAdded = true;
                    startReplacement = false;
                    addSpaceAbove = false;
                }

                lastLine = line;
            }

            // If the custom entries comments do not exist, add the fstab entries to the end of the file
            if (!customEntriesAdded)
            {
                AddCustomEntries(writer, lastLine != null && lastLine.Trim() != "");
                writer.WriteLine();
            }
            // Ensure the file ends with a blank line
            else if (lastLine == null || lastLine.Trim() != "")
            {
                writer.WriteLine();
            }
        }
    }
}