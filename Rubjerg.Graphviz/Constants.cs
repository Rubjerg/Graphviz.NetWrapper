using System;
using System.Runtime.InteropServices;

namespace Rubjerg.Graphviz;

internal static class Constants
{
#if _WINDOWS
    public const string GvcLib = "gvc.dll";
    public const string CGraphLib = "cgraph.dll";
    public const string XDotLib = "xdot.dll";
    public const string GraphvizWrapperLib = "GraphvizWrapper.dll";
#else
    public const string GvcLib = "libgvc.so.6";
    public const string CGraphLib = "libcgraph.so.6";
    public const string XDotLib = "libxdot.so.4";
    public const string GraphvizWrapperLib = "libGraphvizWrapper.so";
#endif
}
