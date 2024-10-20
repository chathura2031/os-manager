using CommandLine;

namespace OSManager.CLI.CliOptions;

// TODO
[Verb("continue", HelpText = "TODO")]
public class ContinueOptions
{
    [Option('a', "slave", Required = true,
        HelpText = "the path to the slave agent")]
    public string SlavePath { get; set; }
    
    [Option('n', "stage", Required = true,
        HelpText = "the stage of installation to continue from")]
    public int Stage { get; set; }
    
    [Option('p', "package", Required = true,
        HelpText = "the name of the package to install")]
    public string Package { get; set; }
    
    [Option('d', "data", HelpText = "the path to use for data")]
    public string DataPath { get; set; }
}
