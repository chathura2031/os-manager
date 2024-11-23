using System.ComponentModel.DataAnnotations;
using CommandLine;
using CommandLine.Text;
using Newtonsoft.Json;

namespace OSManager.CLI.CliOptions;

[Verb("initialise", HelpText = "Initialise the program")]
public class InitialiseOptions
{
    [Option('s', "stack", Required = true,
        HelpText = "the location to use for communications (ie. memory or disk)")]
    public string BaseStackPath { get; set; }
    
    [Option('a', "slave", Required = true,
        HelpText = "the path to the slave agent")]
    public string SlavePath { get; set; }
    
    [Option('w', "workingdir", Required = false,
        HelpText = "the path to the working directory")]
    public string? WorkingDirectory { get; set; }
}
