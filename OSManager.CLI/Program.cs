using CommandLine;
using OSManager.CLI;
using OSManager.CLI.CliOptions;
using OSManager.Communications.Proto;
using OSManager.Plugins.Intercommunication;

IIntercommClient client = new ProtoClient("PipesOfPiece");
Stuff stuff = new(client);

// TODO: Figure out a way to avoid having the types in angle brackets and in the map result
return CommandLine.Parser.Default.ParseArguments<InitialiseOptions, ContinueOptions, PopStackOptions, FinaliseOptions>(args)
    .MapResult(
        (InitialiseOptions opts) => stuff.Initialise(opts),
        (ContinueOptions opts) => stuff.GotoStep(opts),
        (PopStackOptions opts) => stuff.PopStack(opts),
        (FinaliseOptions opts) => stuff.Finalise(opts),
        errs => 1);
