using CommandLine;
using OSManager.CLI;
using OSManager.CLI.CliOptions;
using OSManager.Communications.Proto;
using OSManager.Core;
using OSManager.Core.Interfaces;

IServer server = new ProtoServer("PipesOfPiece");
Stuff stuff = new(server);

// TODO: Figure out a way to avoid having the types in angle brackets and in the map result
return CommandLine.Parser.Default.ParseArguments<InitialiseOptions, ContinueOptions, PushStackOptions, PopStackOptions, FinaliseOptions>(args)
    .MapResult(
        (InitialiseOptions opts) => stuff.Initialise(opts),
        (ContinueOptions opts) => stuff.GotoStep(opts),
        (PushStackOptions opts) => stuff.PushStack(opts),
        (PopStackOptions opts) => stuff.PopStack(opts),
        (FinaliseOptions opts) => stuff.Finalise(opts),
        errs => 1);
