using CommandLine;

namespace OSManager.CLI.CliOptions;

// TODO
[Verb("install", HelpText = "TODO")]
public class InstallOptions
{
    [Option('n', "stage", Required = true,
        HelpText = "the stage of installation to continue from")]
    public int Stage { get; set; }
    
    [Option('p', "package", Required = true,
        HelpText = "the name of the package to install")]
    public string Package { get; set; }
    
    [Option('d', "data", HelpText = "the path to use for data")]
    public string DataPath { get; set; }
}
