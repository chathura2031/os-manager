namespace OSManager.Core;

public static class PreChecks
{
    private static DateTime _lastRun;
    private static readonly TimeSpan _runInterval = new(0, 0, 2);

    public static void Run(bool overrideRunCooldown = false)
    {
        // Enforce the run cooldown time
        if (!overrideRunCooldown && DateTime.Now - _lastRun < _runInterval)
        {
            Console.WriteLine("Skipping pre-checks as they were run recently.");
            return;
        }

        Functions.RunCommand("/usr/bin/sudo", "apt update");
        Functions.RunCommand("/usr/bin/sudo", "apt upgrade -y");
        Functions.RunCommand("/usr/bin/sudo", "apt autoremove -y");
        
        _lastRun = DateTime.Now;
    }
}