using System.Reflection;
using System.Runtime.InteropServices;
using CommandLine;
using OSManager.CLI;
using OSManager.CLI.CliOptions;
using OSManager.CLI.Packages;

if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
{
    throw new SystemException("This program is only supported on Linux.");
}

// Vim.Instance.InstallAndConfigure(1);
// TODO: Ensure the backup and encrypted backup directories exist
// TODO: Unencrypt backup directory before backing up then reencrypt
// Chrome.Instance.Backup(0);
// Discord.Instance.InstallAndConfigure(1);

// Parser.Default.ParseArguments<InitOptions, StageOptions>(args).MapResult(
//     (InitOptions options) => Init(options),
//     errs => 1
// );

return CommandLine.Parser.Default.ParseArguments<InitialiseOptions, ContinueOptions, FinaliseOptions, PopStackOptions>(args)
    .MapResult(
        (InitialiseOptions opts) => Initialise(opts),
        (ContinueOptions opts) => GotoStep(opts),
        (FinaliseOptions opts) => Finalise(opts),
        (PopStackOptions opts) => PopStack(opts),
        errs => 1);

int Initialise(InitialiseOptions options)
{
    AssemblyName assembly = Assembly.GetEntryAssembly()!.GetName();
    Console.WriteLine($"Version {assembly.Version}");

    // TODO: Adjust for the case when the location is on disk
    // TODO: Refactor so this doesn't have to be defined all the time
    StackManager.SlavePath = options.SlavePath;
    // Create a file to keep track of the stack size
    StackManager.Instance = new StackManager(options.StackPath);

    // TODO: Add ability for the user to select what to do here
    int selection = 0;
    switch (selection)
    {
        case 0:
            Discord.Instance.Install(0, null);
            break;
        default:
            throw new ArgumentException("Received an invalid selection");
    }
    
    return 1;
}

int GotoStep(ContinueOptions options)
{
    // TODO: Refactor so this doesn't have to be defined all the time
    StackManager.SlavePath = options.SlavePath;
    StackManager.Instance = new StackManager(options.StackPath, false);
    StackManager.Instance.Pop();
    
    switch (options.Package)
    {
        // TODO: Get from variable instead of hard-coding
        case "discord":
            Discord.Instance.Install(options.Stage, options.DataPath);
            break;
        default:
            throw new ArgumentException("Received an invalid package");
    }
    return 1;
}

int PopStack(PopStackOptions options)
{
    StackManager.Instance = new StackManager(options.StackPath, false);
    for (int i = 0; i < options.Count; i++)
    {
        StackManager.Instance.Pop();
    }
    
    return 1;
}

int Finalise(FinaliseOptions options)
{
    // TODO: Refactor so this doesn't have to be defined all the time
    StackManager.Instance = new StackManager(options.StackPath, false);

    if (StackManager.Instance.Count > 0)
    {
        throw new InvalidOperationException("Failed to terminate program, stack is not empty.");
    }
    
    File.Delete(StackManager.Instance.Path);
    
    return 1;
}