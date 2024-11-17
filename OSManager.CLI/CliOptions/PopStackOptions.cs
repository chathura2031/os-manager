using CommandLine;

namespace OSManager.CLI.CliOptions;

// TODO
[Verb("popstack", HelpText = "TODO")]
public class PopStackOptions
{
    [Option('c', "count", Required = true,
        HelpText = "the number of nodes to pop from the stack")]
    public int Count { get; set; }

    [Option('s', "stack", Required = true,
        HelpText = "the stack to pop (either `bash` or `program`)")]
    public string Stack { get; set; }
}