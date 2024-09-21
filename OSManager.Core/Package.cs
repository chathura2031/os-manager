namespace OSManager.Core;

public abstract class Package
{
    public virtual string Name { get; }
    public virtual string SafeName { get; }
    // public virtual Package[] Dependencies { get; }

    public virtual int InstallAndConfigure(int verbosity = 0)
    {
        int statusCode = Functions.RunFunctions([
            new(() => Install(verbosity)),
            new (() => Configure(verbosity))
        ]);

        return statusCode;
    }

    protected virtual int Install(int verbosity)
    {
        if (verbosity > 0)
        {
            Console.WriteLine("Running pre-checks...");
        }

        int statusCode = PreChecks.Run();
        if (statusCode > 0)
        {
            Console.WriteLine("Failed to run pre-checks.");
            return statusCode;
        }
        
        if (verbosity > 0)
        {
            Console.WriteLine($"Installing {Name}...");
        }

        return 0;
    }

    protected virtual int Configure(int verbosity)
    {
        if (verbosity > 0)
        {
            Console.WriteLine($"Configuring {Name}...");
        }

        return 0;
    }

    protected virtual int BackupConfiguration(int verbosity)
    {
        if (verbosity > 0)
        {
            Console.WriteLine($"Backing up configuration for {Name}...");
        }

        return 0;
    }
    
    // TODO: Add rollback function -- say something like "One or more errors were detected during installation, do you want to rollback or proceed to configuration?"
}