using System.Globalization;

namespace OSManager.CLI.Packages;

public class UpdateAndUpgrade : IPackage
{
    public static readonly UpdateAndUpgrade Instance = new();
    
    public string Name { get; } = "@internal:updateandupgrade";
    
    public string HumanReadableName { get; } = "Update and Upgrade";
    public List<IPackage> Dependencies { get; } = [];
    
    public List<IPackage> OptionalExtras { get; } = [];
    
    // TODO: Move this to memory
    private static string DataFilePath { get; } = $"/tmp/osman.uau";

    public static DateTime? LastRun
    {
        get
        {
            if (!File.Exists(DataFilePath))
            {
                return null;
            }
            else
            {
                return DateTime.Parse(File.ReadAllLines(DataFilePath)[0]);
            }
        }
    }
    
    public int Install(int stage, string data)
    {
        int statusCode = 0;
        switch (stage)
        {
            case 0:
                DateTime? lastRun = LastRun;
                if (lastRun == null || DateTime.Now - lastRun >= new TimeSpan(0, 5, 0))
                {
                    Utilities.RunInReverse([
                        () => Utilities.BashStack.PushBashCommand("apt update", true),
                        () => Utilities.BashStack.PushBashCommand("apt upgrade", true),
                        () => Utilities.BashStack.PushBashCommand("apt autoremove", true),
                        () => Utilities.BashStack.PushNextStage(stage + 1, Name)
                    ]);
                }
                break;
            case 1:
                DateTime lastRun1 = DateTime.Now;
                File.WriteAllLines(DataFilePath, [lastRun1.ToString(CultureInfo.CurrentCulture)]);
                break;
            default:
                throw new ArgumentException($"{HumanReadableName} does not have {stage} stages of installation.");
        }
        return statusCode;
    }
}