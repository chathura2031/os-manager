﻿using System.Reflection;
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
    Utilities.SlavePath = options.SlavePath;
    Utilities.GetOrCreateStacks(options.BaseStackPath, true);

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
    
    return 0;
}

int GotoStep(ContinueOptions options)
{
    // TODO: Refactor so this doesn't have to be defined all the time
    Utilities.SlavePath = options.SlavePath;
    Utilities.GetOrCreateStacks(options.BaseStackPath);
    Utilities.BashStack.Pop();

    PackageRepository.GetPackage(options.Package).Install(options.Stage, options.DataPath);
    return 0;
}

int PopStack(PopStackOptions options)
{
    Utilities.GetOrCreateStacks(options.BaseStackPath);
    for (int i = 0; i < options.Count; i++)
    {
        Utilities.BashStack.Pop();
    }
    
    return 0;
}

int Finalise(FinaliseOptions options)
{
    // TODO: Refactor so this doesn't have to be defined all the time
    Utilities.GetOrCreateStacks(options.BaseStackPath);
    int statusCode = 0;

    if (Utilities.BashStack.Count > 0)
    {
#if debug
        throw new InvalidOperationException("Failed to terminate program, stack is not empty.");
#else
        Console.WriteLine("WARNING: Sudden termination of program, deleting stack.");
        statusCode = 1;
        Utilities.BashStack.Clear();
#endif
    }
    
    File.Delete(Utilities.BashStack.Path);
    return statusCode;
}