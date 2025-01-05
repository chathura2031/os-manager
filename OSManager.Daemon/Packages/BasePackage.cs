using OSManager.Core.Enums;
using OSManager.Core.Extensions;
using OSManager.Daemon.Extensions;

namespace OSManager.Daemon.Packages;

public abstract class BasePackage : IPackage
{
    #region public members
    public abstract Package Package { get; }
    public abstract List<IPackage> Dependencies { get; }
    public abstract List<IPackage> OptionalExtras { get; }
    #endregion

    #region protected members
    protected abstract List<Func<string, int>> InstallSteps { get; }
    protected abstract List<Func<int>> ConfigureSteps { get; }
    protected abstract List<Func<int>> BackupConfigurationSteps { get; }
    protected string BackupDirPath => Path.Join(Utilities.BackupDirectory, Package.Name());
    protected string EncryptedBackupDirPath => Path.Join(Utilities.EncryptedBackupDirectory, Package.Name());
    #endregion
    
    #region public methods
    public int Install(int stage, string data)
    {
        if (stage <= 0)
        {
            throw new Exception($"Invalid stage: {stage}");
        }
        else if (stage > InstallSteps.Count + 1)
        {
            throw new ArgumentException($"{Package.PrettyName()} does not have {stage} stages of installation.");
        }

        if (stage < InstallSteps.Count + 1)
        {
            Utilities.BashStack.PushInstallStage(stage + 1, Package.Name());
        }
        
        return stage == 1 ? this.InstallDependencies() : InstallSteps[stage - 2].Invoke(data);
    }

    public int Configure(int stage)
    {
        if (stage <= 0)
        {
            throw new Exception($"Invalid stage: {stage}");
        }
        else if (stage != 1 && stage > ConfigureSteps.Count)
        {
            throw new ArgumentException($"{Package.PrettyName()} does not have {stage} stages of configuration.");
        }
        
        if (stage < ConfigureSteps.Count)
        {
            Utilities.BashStack.PushConfigureStage(stage + 1, Package.Name());
        }
        
        if (stage > 1 || ConfigureSteps.Count > 0)
        {
            return ConfigureSteps[stage - 1].Invoke();
        }

        return 0;
    }

    public int BackupConfig(int stage)
    {
        if (stage <= 0)
        {
            throw new Exception($"Invalid stage: {stage}");
        }
        else if (stage != 1 && stage > BackupConfigurationSteps.Count)
        {
            throw new ArgumentException($"{Package.PrettyName()} does not have {stage} stages of configuration backup.");
        }
        
        if (stage < BackupConfigurationSteps.Count)
        {
            Utilities.BashStack.PushBackupConfigStage(stage + 1, Package.Name());
        }
        
        if (stage > 1 || BackupConfigurationSteps.Count > 0)
        {
            return BackupConfigurationSteps[stage - 1].Invoke();
        }

        return 0;
    }
    #endregion
}