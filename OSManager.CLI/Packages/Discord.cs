namespace OSManager.CLI.Packages;

public class Discord : IPackage
{
    public static readonly Discord Instance = new();
    
    public string Name { get; } = "Discord";

    public string MachineName { get; } = "discord";

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
            $"{MachineName}.deb", out filePath);

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
        // TODO: Consume the current execution??
        switch (stage)
        {
            case 0:
                StackManager.Instance.Push($"./{StackManager.SlavePath} continue --stack {StackManager.Instance.Path} --slave {StackManager.SlavePath} --stage 1 --package {MachineName}");
                this.InstallDependencies();
                break;
            case 1:
                int statusCode = DownloadPackage(2, out string filePath);
                // TODO
                if (statusCode != 0)
                {
                    throw new Exception("Failed something.. idk TODO");
                }
                
                // TODO: Add pop option?? otherwise gets stuck on stack
                // TODO: Write function to add to stack in reverse
                // TODO: Check status code of previous bash command -- will probably need to create a cache for the script to write the return code to
                // TODO: Convert the data field to a path to a file
                StackManager.Instance.Push($"./{StackManager.SlavePath} continue --stack {StackManager.Instance.Path} --slave {StackManager.SlavePath} --stage 2 --package {MachineName} --data {filePath}");
                StackManager.Instance.Push($"sudo apt install -y --fix-broken {filePath} && ./{StackManager.SlavePath} popstack --stack {StackManager.Instance.Path} --count 1");
                break;
            case 2:
                DeletePackage(2, (string)data!);
                break;
            default:
                throw new ArgumentException($"{Name} does not have {stage} stages of installation.");
        }
    }
}