using CommandLine;

namespace OSManager.CLI.CliOptions;

// TODO
[Verb("pushstack", HelpText = "TODO")]
public class PushStackOptions
{
    [Option('c', "content", Required = true,
        HelpText = "the content to push to the stack")]
    public string Content { get; set; }
}