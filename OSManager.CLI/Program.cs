using CommandLine;
using OSManager.CLI;
using OSManager.CLI.CliOptions;
using OSManager.Communications.Proto;
using OSManager.Plugins.Intercommunication;

IIntercommClient client = new ProtoClient("PipesOfPiece");
Handler handler = new(client);

// TODO: Figure out a way to avoid having the types in angle brackets and in the map result
return CommandLine.Parser.Default.ParseArguments<InitialiseOptions, ContinueOptions, PopStackOptions, FinaliseOptions>(args)
    .MapResult(
        (InitialiseOptions opts) => handler.Initialise(opts),
        (ContinueOptions opts) => handler.GotoStep(opts),
        (PopStackOptions opts) => handler.PopStack(opts),
        (FinaliseOptions opts) => handler.Finalise(opts),
        errs => 1);
