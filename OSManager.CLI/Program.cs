using System.Runtime.InteropServices;
using OSManager.Core.Packages;

if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
{
    throw new SystemException("This program is only supported on Linux.");
}

Discord.Instance.InstallAndConfigure(1);