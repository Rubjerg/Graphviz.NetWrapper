using System;
using System.Runtime.InteropServices;

public static class NativeLibraryNames
{
    public static string GvcLib => _gvcLib.Value;
    public static string CGraphLib => _cgraphLib.Value;
    public static string XDotLib => _xdotLib.Value;
    public static string GraphvizWrapperLib => _wrapperLib.Value;

    private static readonly Lazy<string> _gvcLib = new(() => Resolve("gvc.dll", "libgvc.so.6"));
    private static readonly Lazy<string> _cgraphLib = new(() => Resolve("cgraph.dll", "libcgraph.so.6"));
    private static readonly Lazy<string> _xdotLib = new(() => Resolve("xdot.dll", "libxdot.so.4"));
    private static readonly Lazy<string> _wrapperLib = new(() => Resolve("GraphvizWrapper.dll", "libGraphvizWrapper.so"));

    private static string Resolve(string windows, string linux, string? mac = null)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return windows;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return linux;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) return mac ?? throw new PlatformNotSupportedException("macOS not supported yet.");
        throw new PlatformNotSupportedException("Unknown platform");
    }
}
