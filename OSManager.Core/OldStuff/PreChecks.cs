namespace OSManager.Core;

public static class PreChecks
{
    private static DateTime _lastRun;
    private static readonly TimeSpan _runInterval = new(0, 0, 2);

    public static int Run(bool overrideRunCooldown = false)
    {
        // Enforce the run cooldown time
        if (!overrideRunCooldown && DateTime.Now - _lastRun < _runInterval)
        {
            Console.WriteLine("Skipping pre-checks as they were run recently.");
            return 0;
        }

        int statusCode = Functions.RunCommands([
            new("/usr/bin/sudo", "apt update", "Failed to update package list"),
            new("/usr/bin/sudo", "apt upgrade -y", "Failed to upgrade packages"),
            new("/usr/bin/sudo", "apt autoremove -y", "Failed to automatically remove packages")
        ]);
        
        _lastRun = DateTime.Now;

        return statusCode;
    }
}