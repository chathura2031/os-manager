namespace OSManager.Core.Packages;

public class Discord: Package
{
    public static readonly Discord Instance = new();
    public override string Name { get; protected set; } = "Discord";

    private Discord() { }

    protected override void Install(int verbosity)
    {
        base.Install(verbosity);
        
        // Download the debian package
        if (verbosity > 0)
        {
            Console.WriteLine("Downloading debian package...");
        }
        string filePath = Functions.DownloadFromUrl("https://discord.com/api/download?platform=linux&format=deb", $"{Name}.deb");
        
        // Install the debian package
        Functions.RunCommand("/usr/bin/sudo", $"apt install -y --fix-broken {filePath}");
        
        // Delete the debian package
        if (verbosity > 0)
        {
            Console.WriteLine("Deleting debian package...");
        }
        File.Delete(filePath);
    }

    protected override void Configure(int verbosity)
    {
        // No configuration required
        // base.Configure(verbosity);
    }
}