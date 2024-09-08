namespace OSManager.Core;

public abstract class Package
{
    public virtual string Name { get; protected set; }
    public virtual Package[] Dependencies { get; protected set; }

    public virtual void InstallAndConfigure(int verbosity = 0)
    {
        if (verbosity > 0)
        {
            Console.WriteLine($"Installing {Name}...");
        }

        Install(verbosity);
        
        if (verbosity > 0)
        {
            Console.WriteLine($"Configuring {Name}...");
        }
        Configure(verbosity);
    }

    protected virtual void Install(int verbosity)
    {
        if (verbosity > 0)
        {
            Console.WriteLine("Running pre-checks...");
        }
        PreChecks.Run();
    }

    protected virtual void Configure(int verbosity)
    {
    }
}