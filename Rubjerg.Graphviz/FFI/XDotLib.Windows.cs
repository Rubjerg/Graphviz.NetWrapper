using System;
using System.Runtime.InteropServices;

namespace Rubjerg.Graphviz.FFI;

using static Constants;

/// <summary>
/// See https://graphviz.org/docs/outputs/canon/#xdot
/// </summary>
internal static class XDotLibWindows
{
    [DllImport(XDotLibNameWindows, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr parseXDot(IntPtr xdotString);

    [DllImport(XDotLibNameWindows, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern void freeXDot(IntPtr xdotptr);
}
