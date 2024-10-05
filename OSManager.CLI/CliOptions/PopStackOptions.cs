using CommandLine;

namespace OSManager.CLI.CliOptions;

// TODO
[Verb("popstack", HelpText = "TODO")]
public class PopStackOptions
{
    [Option('s', "stack", Required = true,
        HelpText = "the location to use for communications (ie. memory or disk)")]
    public string StackPath { get; set; }
    
    [Option('c', "count", Required = true,
        HelpText = "the number of nodes to pop from the stack")]
    public int Count { get; set; }
}