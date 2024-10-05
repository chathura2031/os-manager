using System.Diagnostics;

namespace OSManager.Core;

public static class Functions
{
    public static string HomeDirectory { get; } = Environment.GetEnvironmentVariable("HOME") ?? "";
    
    public static string ConfigDirectory { get; } = Path.Join(HomeDirectory, ".config");

    public static string BackupDirectory { get; } = Path.Join(HomeDirectory, "Projects/os-management");
    
    public static string EncryptedBackupDirectory { get; } = Path.Join(HomeDirectory, "Projects/os-setup-secrets");
    
    /// <summary>
    /// Run a command
    /// </summary>
    /// <param name="fileName">The path to the command (e.g. /usr/bin/sudo)</param>
    /// <param name="arguments">The arguments to pass in</param>
    /// <returns>A status code</returns>
    public static int RunCommand(string fileName, string arguments)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo()
        {
            FileName = fileName,
            Arguments = arguments,
            WorkingDirectory = Directory.GetCurrentDirectory(),
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            RedirectStandardInput = true
        };
        
        Process process = new Process() { StartInfo = startInfo };
        process.Start();
        
        // Read output
        while (process.StandardOutput.Peek() > -1 || process.StandardError.Peek() > -1)
        {
            string? tmp;
            
            if (process.StandardError.Peek() > -1 && (tmp = process.StandardError.ReadLine()) != "")
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(tmp);
                Console.ResetColor();
            }
            
            if (process.StandardOutput.Peek() > -1 && (tmp = process.StandardOutput.ReadLine()) != "")
            {
                Console.WriteLine(tmp);
            }
        }
        
        process.WaitForExit();
        
        return process.ExitCode;
    }

    /// <summary>
    /// Run a command
    /// </summary>
    /// <param name="command">The command to run</param>
    /// <param name="logOnFail">True if a message should be logged when the command fails to execute, False otherwise</param>
    /// <returns>A status code</returns>
    public static int RunCommand(Command command, bool logOnFail = true)
    {
        int statusCode = RunCommand(command.FileName, command.Arguments);
        if (logOnFail && statusCode != 0)
        {
            command.LogFailedExecutionMessage();
        }

        return statusCode;
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
            stream.Result.CopyTo(fs);
        }

        return cancelOperation ? 1 : 0;
    }

    /// Copy a file to a new location
    /// </summary>
    /// <param name="sourcePath">The path to the file to copy</param>
    /// <param name="destinationPath">The path to the file at the destination</param>
    /// <param name="promptReplacement">True if a prompt should be shown if the file already exists, False to replace it without a prompt</param>
    /// <returns>A status code</returns>
    public static int CopyFile(string sourcePath, string destinationPath, bool promptReplacement = false)
    {
        if (!File.Exists(sourcePath))
        {
            Console.WriteLine($"{sourcePath} does not exist");
            return 1;
        }

        bool cancelOperation = false;
        if (promptReplacement && File.Exists(destinationPath))
        {
            cancelOperation = !Functions.ShowYesOrNoPrompt($"{destinationPath} already exists. Would you like to replace it?");
        }

        if (!cancelOperation)
        {
            if (File.Exists(destinationPath))
            {
                File.Delete(destinationPath);
            }
            
            try
            {
                // Create the parent destination directory if it doesn't exist
                string? parentDir = (string)Path.GetDirectoryName(destinationPath)!;
                Directory.CreateDirectory(parentDir);
                // Copy the file
                File.Copy(sourcePath, destinationPath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return 1;
            }
        }

        return cancelOperation ? 1 : 0;
    }
    
    /// <summary>
    /// Copy a directory to a new location and if specified, subdirectories
    /// </summary>
    /// <param name="sourcePath">The path to the directory to copy</param>
    /// <param name="destinationPath">The path to the directory at the destination</param>
    /// <param name="promptReplacement">True if a prompt should be shown if the file already exists, False to replace it without a prompt</param>
    /// <returns>A status code</returns>
    public static int CopyDirectory(string sourcePath, string destinationPath, bool promptReplacement = false)
    {
        // Get information about the source directory
        var dir = new DirectoryInfo(sourcePath);

        // Check if the source directory exists
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
            string targetFilePath = Path.Join(destinationPath, file.Name);
            CopyFile(file.FullName, targetFilePath, promptReplacement);
        }

        // TODO: Implement this iteratively
        // If recursive and copying subdirectories, recursively call this method
        foreach (DirectoryInfo subDir in dirs)
        {
            bool cancelOperation = false;
            string newDestinationDir = Path.Join(destinationPath, subDir.Name);
            
            if (promptReplacement && File.Exists(newDestinationDir))
            {
                cancelOperation = !Functions.ShowYesOrNoPrompt($"{newDestinationDir} already exists. Would you like to replace it?");
            }

            if (!cancelOperation)
            {
                CopyDirectory(subDir.FullName, newDestinationDir, promptReplacement);
            }
        }

        return 0;
    }
    
    /// <summary>
    /// Run a sequence of commands one after another if the previous one executed successfully
    /// </summary>
    /// <param name="commands">A list of commands to run</param>
    /// <returns>The status code after running the commands</returns>
    public static int RunCommands(Command[] commands)
    {
        int statusCode = 0;
        foreach (Command command in commands)
        {
            statusCode = RunCommand(command);
            if (statusCode != 0)
            {
                break;
            }
        }

        return statusCode;
    }

    /// <summary>
    /// Run a sequence of functions one after another if the previous one executed successfully
    /// </summary>
    /// <param name="functions">A list of functions to run</param>
    /// <returns>The status code after running the functions</returns>
    public static int RunFunctions(Function[] functions)
    {
        int statusCode = 0;
        foreach (Function function in functions)
        {
            statusCode = function.Func.Invoke();
            if (statusCode != 0)
            {
                function.LogFailedExecutionMessage();
                break;
            }
        }
        
        return statusCode;
    }

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
}