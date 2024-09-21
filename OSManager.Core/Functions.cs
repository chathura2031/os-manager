using System.Diagnostics;

namespace OSManager.Core;

public static class Functions
{
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
    
    public static string DownloadFromUrl(string url, string fileName, string? downloadDirectory = null)
    {
        using HttpClient client = new();
        using Task<Stream> stream = client.GetStreamAsync(url);
        downloadDirectory ??= Directory.GetCurrentDirectory();
        string filePath = Path.Join(downloadDirectory, fileName);
        using FileStream fs = new(filePath, FileMode.CreateNew);
        stream.Result.CopyTo(fs);

        return filePath;
    }
}