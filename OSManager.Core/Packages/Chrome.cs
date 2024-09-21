namespace OSManager.Core.Packages;

public class Chrome: Package
{
    public static readonly Chrome Instance = new();
    public override string Name { get; protected set; } = "Google Chrome";

    private Chrome() { }

    private int DownloadPackage(int verbosity, out string filePath)
    {
        // Download the debian package
        if (verbosity > 0)
        {
            Console.WriteLine("Downloading debian package...");
        }
        int statusCode = Functions.DownloadFromUrl("https://dl.google.com/linux/direct/google-chrome-stable_current_amd64.deb", $"{Name}.deb", out filePath);

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
            // TODO: Install chrome
            new(() => DeletePackage(verbosity, filePath), "Failed to delete debian file")
        ]);

        return statusCode;
    }

    protected override void Configure(int verbosity)
    {
        base.Configure(verbosity);

        string homeDir = "";
    }
}