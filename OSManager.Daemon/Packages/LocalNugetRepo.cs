using System.Xml;
using OSManager.Core.Enums;

namespace OSManager.Daemon.Packages;

public class LocalNugetRepo : BasePackage
{
    #region public static members
    public static readonly LocalNugetRepo Instance = new();
    #endregion

    #region public members
    public override Package Package { get; } = Package.LocalNugetRepo;
    public override List<IPackage> Dependencies { get; } = [];
    public override List<IPackage> OptionalExtras { get; } = [];
    #endregion
    
    #region protected members
    protected override List<Func<string, InstallStepReturnData>> InstallSteps { get; }
    protected override List<Func<int>> ConfigureSteps { get; }
    protected override List<Func<int>> BackupConfigurationSteps { get; }
    #endregion
    
    #region private members
    private string NugetDirPath => Path.Join(Utilities.HomeDirectory, ".nuget");
    private string LocalNugetRepoPath => Path.Join(NugetDirPath, "local-packages");
    private string NugetConfigDirPath => Path.Join(NugetDirPath, "NuGet");
    private string NugetConfigFilePath => Path.Join(NugetConfigDirPath, "NuGet.Config");
    private string NugetTemplateConfigFilePath => Path.Join(BackupDirPath, "NuGet.Config");
    #endregion

    #region ctor
    private LocalNugetRepo()
    {
        InstallSteps = [];
        ConfigureSteps = [CreateLocalNugetRepo];
        BackupConfigurationSteps = [BackupNugetConfig];
    }
    #endregion
    
    #region private methods
    private void UpdateNugetConfigFile()
    {
        if (!File.Exists(NugetTemplateConfigFilePath))
        {
            return;
        }
        
        Directory.CreateDirectory(LocalNugetRepoPath);
        Directory.CreateDirectory(NugetConfigDirPath);

        // Create the nuget config file from a template if it doesn't exist
        if (!File.Exists(NugetConfigFilePath))
        {
            File.Copy(NugetTemplateConfigFilePath, NugetConfigFilePath);
        }

        // Add the local nuget repository directory as a source
        XmlDocument doc = new();
        doc.Load(NugetConfigFilePath);

        XmlNode? sources = doc.DocumentElement!.SelectSingleNode("/configuration/packageSources");
        if (sources == null)
        {
            throw new Exception("Invalid NuGet.Config file. Could not find sources.");
        }

        XmlNodeList sourceNodes = ((XmlNode)sources).SelectNodes("add")!;
        bool entryAdded = false;
        foreach (XmlNode sourceNode in sourceNodes)
        {
            if (sourceNode.Attributes!["key"]!.Value != "LocalRepo")
            {
                continue;
            }
            
            if (sourceNode.Attributes["value"]!.Value != LocalNugetRepoPath)
            {
                sourceNode.Attributes["value"]!.Value = LocalNugetRepoPath;
            }

            entryAdded = true;
        }

        if (!entryAdded)
        {
            XmlElement node = doc.CreateElement("add");
            node.SetAttribute("key", "LocalRepo");
            node.SetAttribute("value", LocalNugetRepoPath);
            
            sources.AppendChild(node);
            entryAdded = true;
        }
        
        // Update the nuget config file
        doc.Save(NugetConfigFilePath);
    }

    private void DownloadNugetPackages()
    {
        // Download the nuget packages
        Utilities.DownloadFromUrl(
            "https://raw.githubusercontent.com/chathura2031/persistent-tools/master/PersistentTools.Stack/bin/Release/latest.nupkg",
            $"PersistentTools.Stack.latest.nupkg",
            out _,
            LocalNugetRepoPath
        );
    }
    
    private int CreateLocalNugetRepo()
    {
        UpdateNugetConfigFile();
        DownloadNugetPackages();

        return 0;
    }

    private int BackupNugetConfig()
    {
        if (!File.Exists(NugetConfigFilePath))
        {
            return 0;
        }
        
        Directory.CreateDirectory(BackupDirPath);
        
        XmlDocument doc = new();
        doc.Load(NugetConfigFilePath);

        XmlNode? sources = doc.DocumentElement!.SelectSingleNode("/configuration/packageSources");
        if (sources == null)
        {
            throw new Exception("Invalid NuGet.Config file. Could not find sources.");
        }

        XmlNodeList sourceNodes = ((XmlNode)sources).SelectNodes("add")!;
        foreach (XmlNode sourceNode in sourceNodes)
        {
            if (sourceNode.Attributes!["key"]!.Value == "LocalRepo")
            {
                sources.RemoveChild(sourceNode);
                break;
            }
        }

        doc.Save(NugetTemplateConfigFilePath);
        
        return 0;
    }
    #endregion
}