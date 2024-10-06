using CommandLine;

namespace OSManager.CLI.CliOptions;

// TODO
[Verb("pushstack", HelpText = "TODO")]
public class PushStackOptions
{
    [Option('s', "stack", Required = true,
        HelpText = "the location to use for communications (ie. memory or disk)")]
    public string BaseStackPath { get; set; }
}