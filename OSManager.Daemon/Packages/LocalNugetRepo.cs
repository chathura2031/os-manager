using System.Xml;
using OSManager.Core.Enums;
using OSManager.Core.Extensions;
using OSManager.Daemon.Extensions;

namespace OSManager.Daemon.Packages;

public class LocalNugetRepo : IPackage
{
    public static readonly LocalNugetRepo Instance = new();

    public Package Package { get; } = Package.LocalNugetRepo;

    public List<IPackage> Dependencies { get; } = [];

    public List<IPackage> OptionalExtras { get; } = [];
    
    public int Install(int stage, string data)
    {
        return 0;
    }

    public int Configure(int stage)
    {
        int statusCode = 0;
        switch (stage)
        {
            case 1:
            {
                Utilities.BashStack.PushConfigureStage(stage + 1, Package.Name());
                this.InstallDependencies();
                break;
            }
            case 2:
            {
                string nugetPath = Path.Join(Utilities.HomeDirectory, ".nuget");
                string localNugetPath = Path.Join(nugetPath, "local-packages");
                
                Directory.CreateDirectory(localNugetPath);

                // Create the nuget config file from a template if it doesn't exist
                string nugetConfigPath = Path.Join(nugetPath, "NuGet/NuGet.Config");
                if (!File.Exists(nugetConfigPath))
                {
                    File.Copy(Path.Join(Utilities.BackupDirectory, Package.Name(), "NuGet.Config"),
                        nugetConfigPath);
                }

                // Add the local nuget repository directory as a source
                XmlDocument doc = new();
                doc.Load(nugetConfigPath);

                XmlNode? sources = doc.DocumentElement!.SelectSingleNode("/configuration/packageSources");
                if (sources == null)
                {
                    throw new Exception("Invalid NuGet.Config file. Could not find sources.");
                }

                XmlNodeList stuff = ((XmlNode)sources).SelectNodes("add")!;
                bool entryAdded = false;
                foreach (XmlNode thing in stuff)
                {
                    if (thing.Attributes!["key"]!.Value != "LocalRepo")
                    {
                        continue;
                    }
                    
                    if (thing.Attributes["value"]!.Value != localNugetPath)
                    {
                        thing.Attributes["value"]!.Value = localNugetPath;
                    }

                    entryAdded = true;
                }

                if (!entryAdded)
                {
                    XmlElement node = doc.CreateElement("add");
                    node.SetAttribute("key", "LocalRepo");
                    node.SetAttribute("value", localNugetPath);
                    
                    sources.AppendChild(node);
                    entryAdded = true;
                }
                
                // Update the nuget config file
                doc.Save(nugetConfigPath);
                
                // Download the nuget packages
                Utilities.DownloadFromUrl(
                    "https://raw.githubusercontent.com/chathura2031/persistent-tools/master/PersistentTools.Stack/bin/Release/latest.nupkg",
                    $"PersistentTools.Stack.latest.nupkg",
                    out string filePath,
                    localNugetPath
                );
                
                break;
            }
            default:
                throw new ArgumentException($"{Package.PrettyName()} does not have {stage} stages of configuration.");
        }

        return statusCode;
    }

    public int BackupConfig(int stage)
    {
        // TODO
        return 0;
    }
}