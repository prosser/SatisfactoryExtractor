namespace Rosser.SatisfactoryExtractor;

using System;

#if !NET6_0_OR_GREATER
using System.Runtime.InteropServices;
#endif

internal static class OsHelper
{
#if NET6_0_OR_GREATER
    public static bool IsLinux { get; } = OperatingSystem.IsLinux();
    public static bool IsWindows { get; } = OperatingSystem.IsWindows();
#else
    public static bool IsLinux { get; } = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
    public static bool IsWindows { get; } = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
#endif
    public static string PlatformName { get; } = IsLinux ? "linux" : IsWindows ? "windows" : throw new NotSupportedException();
}
