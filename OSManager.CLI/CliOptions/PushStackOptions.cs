using CommandLine;

namespace OSManager.CLI.CliOptions;

// TODO
[Verb("pushstack", HelpText = "TODO")]
public class PushStackOptions
{
    [Option('c', "content", 
        HelpText = "the content to push to the stack")]
    public string? Content { get; set; }
    
    [Option('f', "file", 
        HelpText = "the file path containing the content to push to the stack")]
    public string? FilePath { get; set; }
}