using System.Text.RegularExpressions;

namespace OSManager.CLI.Packages;

/// <summary>
/// This is a special type of pseudo-package which is responsible for installing dependencies for other packages.
/// </summary>
public class PackageDependencies : IPackage
{
    public static readonly PackageDependencies Instance = new();

    public string Name { get; } = "Dependencies";
    public string PathSafeName { get; } = "@internal:dependencies";

    public List<IPackage> Dependencies { get; } = [];

    public List<IPackage> OptionalExtras { get; } = [];
    
    public int Install(int stage, string pathSafeName)
    {
        IPackage package = PackageRepository.GetPackage(pathSafeName);
        
        switch (stage)
        {
            case 0:
                // Check whether each dependency has been installed
                Utilities.BashStack.PushNextStage(stage + 1, PathSafeName, pathSafeName);
                foreach (IPackage dependency in package.Dependencies)
                {
                    Utilities.BashStack.PushPackageExistsCommand(dependency.PathSafeName);
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
                        Utilities.BashStack.PushNextStage(0, dependency.PathSafeName);
                    }
                    else if (packageName != dependency.PathSafeName)
                    {
                        throw new ArgumentException($"Received installation status for {packageName} but expected {dependency.PathSafeName}");
                    }
                }
                break;
            default:
                throw new ArgumentException($"Dependency installer does not have {stage} stages of installation.");
        }

        return 0;
    }
}