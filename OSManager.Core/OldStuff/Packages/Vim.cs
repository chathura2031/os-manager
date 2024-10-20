namespace OSManager.Core.Packages;

public sealed class Vim: Package
{
    public static readonly Vim Instance = new();
    
    public override string Name { get; } = "Vim";

    public override string SafeName { get; } = "vim";

    private const string ConfigFileName = "vimrc";

    private const string DestinationDirPath = "/etc/vim";
    
    private readonly string _backupDirPath;

    private Vim()
    {
        _backupDirPath = Path.Join(Directory.GetCurrentDirectory(), SafeName);
    }

    private int CopyConfigFile(int verbosity)
    {
        string sourceFilePath = Path.Join(Directory.GetCurrentDirectory(), SafeName, ConfigFileName);
        string destinationFilePath = Path.Join(DestinationDirPath, ConfigFileName);
        
        if (!File.Exists(sourceFilePath))
        {
            if (verbosity > 0)
            {
                Console.WriteLine($"Could not find {sourceFilePath}");
            }
            return 1;
        }

        if (!Directory.Exists(DestinationDirPath))
        {
            if (verbosity > 1)
            {
                Console.WriteLine($"Creating {DestinationDirPath}...");
            }
            Directory.CreateDirectory(DestinationDirPath);
        }

        if (verbosity > 1)
        {
            Console.WriteLine($"Copying vimrc file to {destinationFilePath}...");
        }
        int statusCode = Functions.CopyFile(sourceFilePath, destinationFilePath, true);

        return statusCode;
    }

    private int BackupConfigFile(int verbosity)
    {
        string sourceFilePath = Path.Join(DestinationDirPath, ConfigFileName);
        string destinationFilePath = Path.Join(_backupDirPath, ConfigFileName);
        
        if (!File.Exists(sourceFilePath))
        {
            if (verbosity > 0)
            {
                Console.WriteLine($"Could not find {sourceFilePath}");
            }
            return 1;
        }
        
        if (!Directory.Exists(_backupDirPath))
        {
            if (verbosity > 1)
            {
                Console.WriteLine($"Creating {_backupDirPath}...");
            }
            Directory.CreateDirectory(_backupDirPath);
        }
        
        if (verbosity > 1)
        {
            Console.WriteLine($"Copying vimrc file to {destinationFilePath}...");
        }
        int statusCode = Functions.CopyFile(sourceFilePath, destinationFilePath, true);

        return statusCode;
    }
    
    protected override int Install(int verbosity)
    {
        int statusCode = Functions.RunFunctions([
            new(() => base.Install(verbosity)),
            new(() => Functions.RunCommand("/usr/bin/sudo", $"apt install -y vim"), "Failed to install package")
        ]);

        return statusCode;
    }
    
    protected override int Configure(int verbosity)
    {
        int statusCode = Functions.RunFunctions([
            new(() => base.Configure(verbosity)),
            new(() => CopyConfigFile(verbosity), "Failed to copy vimrc file")
        ]);

        return statusCode;
    }

    protected override int BackupConfiguration(int verbosity)
    {
        int statusCode = Functions.RunFunctions([
            new(() => base.BackupConfiguration(verbosity)),
            new(() => CopyConfigFile(verbosity))
        ]);

        return statusCode;
    }
}