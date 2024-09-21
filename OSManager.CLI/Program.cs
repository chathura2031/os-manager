using System.Reflection;
using System.Runtime.InteropServices;
using OSManager.Core.Packages;

AssemblyName assembly = Assembly.GetEntryAssembly().GetName();
Console.WriteLine($"Version {assembly.Version}");

if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
{
    throw new SystemException("This program is only supported on Linux.");
}

Vim.Instance.InstallAndConfigure(1);