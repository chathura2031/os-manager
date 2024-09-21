namespace OSManager.Core;

public abstract class Package
{
    public virtual string Name { get; protected set; }
    public virtual Package[] Dependencies { get; protected set; }

    public virtual void InstallAndConfigure(int verbosity = 0)
    {
        Install(verbosity);
        
        Configure(verbosity);
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

    protected virtual void Configure(int verbosity)
    {
        if (verbosity > 0)
        {
            Console.WriteLine($"Configuring {Name}...");
        }
    }

    protected virtual void BackupConfiguration(int verbosity)
    {
        if (verbosity > 0)
        {
            Console.WriteLine($"Backing up configuration for {Name}...");
        }
    }
    
    // TODO: Add rollback function -- say something like "One or more errors were detected during installation, do you want to rollback or proceed to configuration?"
}