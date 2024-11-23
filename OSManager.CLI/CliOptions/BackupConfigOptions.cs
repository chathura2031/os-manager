using CommandLine;

namespace OSManager.CLI.CliOptions;

[Verb("backupconfig", HelpText = "TODO")]
public class BackupConfigOptions
{
    [Option('n', "stage", Required = true,
        HelpText = "the stage of backup configuration to continue from")]
    public int Stage { get; set; }
    
    [Option('p', "package", Required = true,
        HelpText = "the name of the package to backup configuration for")]
    public string Package { get; set; }
}