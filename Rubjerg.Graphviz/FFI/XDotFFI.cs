using System;

namespace Rubjerg.Graphviz.FFI;

using static Marshaling;
using static Platform;

internal static class XDotFFI
{
    public static IntPtr ParseXDot(string xdotString)
    {
        return MarshalToUtf8(xdotString, s => IsWindows ? XDotLibWindows.parseXDot(s) : XDotLibLinux.parseXDot(s));
    }

    public static void FreeXDot(IntPtr xdotptr)
    {
        if (IsWindows)
            XDotLibWindows.freeXDot(xdotptr);
        else
            XDotLibLinux.freeXDot(xdotptr);
    }
}
