using Newtonsoft.Json.Linq;

namespace OSManager.Core.Packages;

public class Chrome: Package
{
    public static readonly Chrome Instance = new();
    public override string Name { get; } = "Google Chrome";

    public override string SafeName { get; } = "google-chrome";

    private Chrome() { }

    private int DownloadPackage(int verbosity, out string filePath)
    {
        // Download the debian package
        if (verbosity > 0)
        {
            Console.WriteLine("Downloading debian package...");
        }
        int statusCode = Functions.DownloadFromUrl("https://dl.google.com/linux/direct/google-chrome-stable_current_amd64.deb", $"{SafeName}.deb", out filePath);

        return statusCode;
    }

    private int DeletePackage(int verbosity, string filePath)
    {
        // Delete the debian package
        if (verbosity > 0)
        {
            Console.WriteLine("Deleting debian package...");
        }
        File.Delete(filePath);

        return 0;
    }
    
    protected override int Install(int verbosity)
    {
        string filePath = "";
        int statusCode = Functions.RunFunctions([
            new(() => base.Install(verbosity)),
            new(() => DownloadPackage(verbosity, out filePath), "Failed to download debian file"),
            new(() => Functions.RunCommand("/usr/bin/sudo", $"apt install -y --fix-broken {filePath}"), "Failed to install debian file"),
            new(() => DeletePackage(verbosity, filePath), "Failed to delete debian file")
        ]);

        return statusCode;
    }
    
    protected override int Configure(int verbosity)
    {
        int statusCode = Functions.RunFunctions([
            new(() => base.Configure(verbosity))
        ]);

        return statusCode;
    }

    protected override int BackupConfiguration(int verbosity)
    {
        base.BackupConfiguration(verbosity);

        // TODO: Convert to functions with return codes
        string sourcePath = Path.Join(Functions.ConfigDirectory, "google-chrome");
        // TODO: Repeat for all profiles
        string profileSourcePath = Path.Join(sourcePath, "Profile 1");

        string encryptedTargetPath = Path.Join(Functions.EncryptedBackupDirectory, "chrome");
        string encryptedProfileTargetPath = Path.Join(encryptedTargetPath, "Profile 1");
        
        // Backup profile metadata
        Functions.CopyFile(Path.Join(sourcePath, "Local State"), Path.Join(encryptedTargetPath, "Local State"));
        
        // Backup extension data
        Functions.CopyFile(Path.Join(profileSourcePath, "Preferences"), Path.Join(encryptedProfileTargetPath, "Preferences"));
        Functions.CopyDirectory(Path.Join(profileSourcePath, "Extensions"), Path.Join(encryptedProfileTargetPath, "Extensions"));
        Functions.CopyDirectory(Path.Join(profileSourcePath, "Local Extension Settings"), Path.Join(encryptedProfileTargetPath, "Local Extension Settings"));
        Functions.CopyDirectory(Path.Join(profileSourcePath, "Managed Extension Settings"), Path.Join(encryptedProfileTargetPath, "Managed Extension Settings"));

        // TODO: Read in and store the important parts of the preferences file
        // Functions.CopyDirectory(sourcePath, Path.Join(Functions.EncryptedBackupDirectory, "google-chrome"));
        // JObject o1 = JObject.Parse(File.ReadAllText(Path.Join(sourcePath, "Local State")));
        // JObject o2 = JObject.Parse(File.ReadAllText(Path.Join(profileSourcePath, "Preferences")));
        
        return 0;
    }
}