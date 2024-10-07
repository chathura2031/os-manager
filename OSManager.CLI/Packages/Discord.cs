namespace OSManager.CLI.Packages;

public class Discord : IPackage
{
    public static readonly Discord Instance = new();
    
    public string Name { get; } = "discord";

    public string HumanReadableName { get; } = "Discord";

    public List<IPackage> Dependencies { get; } = [];
    
    public List<IPackage> OptionalExtras { get; } = [];

    private int DownloadPackage(int verbosity, out string filePath)
    {
        // Download the debian package
        if (verbosity > 0)
        {
            Console.WriteLine("Downloading debian package...");
        }

        int statusCode = Utilities.DownloadFromUrl("https://discord.com/api/download?platform=linux&format=deb",
            $"{Name}.deb", out filePath);

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

    public int Install(int stage, string data)
    {
        int statusCode = 0;
        switch (stage)
        {
            case 0:
                Utilities.BashStack.PushNextStage(stage + 1, Name);
                this.InstallDependencies();
                break;
            case 1:
                statusCode = DownloadPackage(2, out string filePath);
                if (statusCode != 0)
                {
                    Console.WriteLine("Failed to download debian file");
                    File.Delete(filePath);
                    return 1;
                }
                
                // TODO: Check status code of previous bash command -- will probably need to create a cache for the script to write the return code to
                // TODO: Convert the data field to a path to a file
                Utilities.RunInReverse([
                    () => Utilities.BashStack.PushBashCommand($"apt install -y --fix-broken {filePath}", true),
                    () => Utilities.BashStack.PushNextStage(stage + 1, Name, filePath),
                ]);
                break;
            case 2:
                string filePath = data;
                statusCode = DeletePackage(2, filePath);
                if (statusCode != 0)
                {
                    Console.WriteLine("Failed to delete debian file");
                    return 1;
                }
                break;
            default:
                throw new ArgumentException($"{HumanReadableName} does not have {stage} stages of installation.");
        }

        return statusCode;
    }
}