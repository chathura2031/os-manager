using CommandLine;

namespace OSManager.CLI.CliOptions;

// TODO
[Verb("configure", HelpText = "TODO")]
public class ConfigureOptions
{
    [Option('n', "stage", Required = true,
        HelpText = "the stage of configuration to continue from")]
    public int Stage { get; set; }
    
    [Option('p', "package", Required = true,
        HelpText = "the name of the package to configure")]
    public string Package { get; set; }
}