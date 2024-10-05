namespace OSManager.CLI.Packages;

public class Discord : IPackage
{
    public static readonly Discord Instance = new();
    
    public string Name { get; } = "Discord";

    public string PathSafeName { get; } = "discord";

    public HashSet<IPackage> Dependencies { get; } = [];
    
    public HashSet<IPackage> OptionalExtras { get; } = [];
    
    private int DownloadPackage(int verbosity, out string filePath)
    {
        // Download the debian package
        if (verbosity > 0)
        {
            Console.WriteLine("Downloading debian package...");
        }

        int statusCode = Utilities.DownloadFromUrl("https://discord.com/api/download?platform=linux&format=deb",
            $"{PathSafeName}.deb", out filePath);

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

    public void Install(int stage, string? data)
    {
        switch (stage)
        {
            case 0:
                StackManager.Instance.Push($"./{Utilities.SlavePath} continue --stack {StackManager.Instance.Path} --slave {Utilities.SlavePath} --stage 1 --package {PathSafeName}");
                this.InstallDependencies();
                break;
            case 1:
                int statusCode = DownloadPackage(2, out string filePath);
                // TODO
                if (statusCode != 0)
                {
                    throw new Exception("Failed something.. idk TODO");
                }
                
                // TODO: Check status code of previous bash command -- will probably need to create a cache for the script to write the return code to
                // TODO: Convert the data field to a path to a file
                Utilities.RunInReverse([
                    () => StackManager.Instance.PushBashCommand($"sudo apt install -y --fix-broken {filePath}"),
                    () => StackManager.Instance.PushNextStage(2,PathSafeName, filePath),
                ]);
                break;
            case 2:
                DeletePackage(2, (string)data!);
                break;
            default:
                throw new ArgumentException($"{Name} does not have {stage} stages of installation.");
        }
    }
}