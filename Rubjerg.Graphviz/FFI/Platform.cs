using System.Runtime.InteropServices;

namespace Rubjerg.Graphviz.FFI;

public static class Platform
{
    public static bool IsWindows { get; } = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    public static bool IsLinux { get; } = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
}
