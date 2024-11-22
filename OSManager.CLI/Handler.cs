using System.Reflection;
using OSManager.CLI.CliOptions;
using OSManager.Core.Enums;
using OSManager.Core.Extensions;
using OSManager.Plugins.Intercommunication;
using OSManager.Plugins.Intercommunication.Enums;

namespace OSManager.CLI;

public class Handler(IIntercommClient client)
{
    public Package GetPackageToInstall()
    {
        List<Package> packages = new();
        // Get all non-internal packages
        foreach (Package package in Enum.GetValues(typeof(Package)))
        {
            string enumName = package.ToString();
            if (enumName.Length >= 9 && enumName.Substring(0, 9) == "INTERNAL_")
            {
                continue;
            }
            
            packages.Add(package);
        }

        Console.WriteLine("##################################################");
        for (int i = 0; i < packages.Count; i++)
        {
            Console.WriteLine($"{i+1}. {packages[i].PrettyName()}");
        }

        while (true)
        {
            Console.Write($"Please enter a value between 1 and {packages.Count} to select a package to install: ");
            bool success = int.TryParse(Console.ReadLine(), out int selection);

            if (!success || selection < 1 || selection > packages.Count)
            {
                Console.WriteLine("Invalid selection. Try again.");
                continue;
            }

            return packages[selection-1];
        }
    }
    
    public int Initialise(InitialiseOptions options)
    {
        AssemblyName assembly = Assembly.GetEntryAssembly()!.GetName();
        Console.WriteLine($"Version {assembly.Version}");

        // TODO: Handle status code
        int statusCode = client.ConnectToServer(options.BaseStackPath, options.SlavePath);
        if (statusCode != 0)
        {
            Console.WriteLine("Could not connect to daemon");
            return 1;
        }

        Package packageToInstall = GetPackageToInstall();
        return client.Install(packageToInstall, 1);
    }
    
    public int Install(InstallOptions options)
    {
        foreach (Package package in Enum.GetValues(typeof(Package)))
        {
            if (options.Package == package.Name())
            {
                return client.Install(package, options.Stage, options.DataPath);
            }
        }
        
        throw new NotImplementedException();
    }

    public int PopStack(PopStackOptions options)
    {
        StackType stack = options.Stack switch
        {
            "bash" => StackType.BashStack,
            "program" => StackType.ProgramStack,
            _ => throw new NotImplementedException()
        };

        return client.PopStack(1, stack);
    }

    public int PushStack(PushStackOptions options)
    {
        string[] content;
        if (options.FilePath != null)
        {
            content = File.ReadAllLines(options.FilePath);
        }
        else if (options.Content != null)
        {
            content = [options.Content];
        }
        else
        {
            throw new Exception("Either content or a file path must be provided");
        }
        
        return client.PushStack(content);
    }

    public int Finalise(FinaliseOptions options)
    {
        return client.DisconnectFromServer();
    }
}