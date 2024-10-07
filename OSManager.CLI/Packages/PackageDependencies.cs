using System.Text.RegularExpressions;

namespace OSManager.CLI.Packages;

/// <summary>
/// This is a special type of pseudo-package which is responsible for installing dependencies for other packages.
/// </summary>
public class PackageDependencies : IPackage
{
    public static readonly PackageDependencies Instance = new();

    public string Name { get; } = "@internal:dependencies";

    public string HumanReadableName { get; } = "Dependencies";
    
    public List<IPackage> Dependencies { get; } = [];

    public List<IPackage> OptionalExtras { get; } = [];
    
    public int Install(int stage, string dependencyName)
    {
        IPackage package = PackageRepository.GetPackage(dependencyName);
        
        switch (stage)
        {
            case 0:
                // Check whether each dependency has been installed
                Utilities.BashStack.PushNextStage(stage + 1, Name, dependencyName);
                foreach (IPackage dependency in package.Dependencies)
                {
                    Utilities.BashStack.PushPackageExistsCommand(dependency.Name);
                }
                break;
            case 1:
                // Install each dependency that has not been installed
                foreach (IPackage dependency in package.Dependencies)
                {
                    string output = Utilities.ProgramStack.Pop().Trim().Split("\n")[^1];
                    string packageName = Regex.Split(output, @"\s+")[1];

                    if (packageName == "no")
                    {
                        Utilities.BashStack.PushNextStage(0, dependency.Name);
                    }
                    else if (packageName != dependency.Name)
                    {
                        throw new ArgumentException($"Received installation status for {packageName} but expected {dependency.Name}");
                    }
                }
                break;
            default:
                throw new ArgumentException($"Dependency installer does not have {stage} stages of installation.");
        }

        return 0;
    }
}