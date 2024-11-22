using System.Text.RegularExpressions;
using OSManager.Core.Enums;
using OSManager.Core.Extensions;

namespace OSManager.Daemon.Packages;

/// <summary>
/// This is a special type of pseudo-package which is responsible for installing dependencies for other packages.
/// </summary>
public class PackageDependencies : IPackage
{
    public static readonly PackageDependencies Instance = new();

    public Package Package { get; } = Package.INTERNAL_PackageDependencies;
    
    public List<IPackage> Dependencies { get; } = [];

    public List<IPackage> OptionalExtras { get; } = [];
    
    public int Install(int stage, string dependencyName)
    {
        IPackage package = PackageRepository.GetPackage(dependencyName);
        
        switch (stage)
        {
            case 1:
                // Update sources and upgrade packages
                Utilities.BashStack.PushInstallStage(stage + 1, Package.Name(), dependencyName);
                UpdateAndUpgrade.Instance.Install(1, "");
                break;
            case 2:
                // Check whether each dependency has been installed
                Utilities.BashStack.PushInstallStage(stage + 1, Package.Name(), dependencyName);
                foreach (IPackage dependency in package.Dependencies)
                {
                    Utilities.BashStack.PushPackageExistsCommand(dependency.Package.Name());
                }
                break;
            case 3:
                // Install each dependency that has not been installed
                foreach (IPackage dependency in package.Dependencies)
                {
                    throw new NotImplementedException();
                    string output = Utilities.ProgramStack.Pop().Trim().Split("\n")[^1];
                    string packageName = Regex.Split(output, @"\s+")[1];

                    if (packageName == "no")
                    {
                        
                        Utilities.BashStack.PushInstallStage(0, dependency.Package.Name());
                    }
                    else if (packageName != dependency.Package.Name())
                    {
                        throw new ArgumentException($"Received installation status for {packageName} but expected {dependency.Package.Name()}");
                    }
                }
                break;
            default:
                throw new ArgumentException($"Dependency installer does not have {stage} stages of installation.");
        }

        return 0;
    }

    public int Configure(int stage)
    {
        return 0;
    }
}