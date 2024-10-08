namespace OSManager.Core.Packages;

public class Discord: Package
{
    public static readonly Discord Instance = new();
    public override string Name { get; } = "Discord";

    public override string SafeName { get; } = "discord";

    private Discord() { }

    private int DownloadPackage(int verbosity, out string filePath)
    {
        // Download the debian package
        if (verbosity > 0)
        {
            Console.WriteLine("Downloading debian package...");
        }

        int statusCode = Functions.DownloadFromUrl("https://discord.com/api/download?platform=linux&format=deb",
            $"{SafeName}.deb", out filePath);

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
        // No configuration required
        // base.Configure(verbosity);
        return 0;
    }
}