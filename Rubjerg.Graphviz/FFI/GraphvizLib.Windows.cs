using System;
using System.Runtime.InteropServices;

namespace Rubjerg.Graphviz.FFI;

using static Constants;

internal static class GraphvizLibWindows
{
    [DllImport(CGraphLibNameWindows, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void agattr(IntPtr graph, int type, IntPtr name, IntPtr deflt);

    [DllImport(CGraphLibNameWindows, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr agedge(IntPtr graph, IntPtr tail, IntPtr head, IntPtr name, int create);

    [DllImport(CGraphLibNameWindows, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int agclose(IntPtr graph);
    [DllImport(CGraphLibNameWindows, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int agcontains(IntPtr graph, IntPtr obj);
    [DllImport(CGraphLibNameWindows, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int agcopyattr(IntPtr from, IntPtr to);
    [DllImport(CGraphLibNameWindows, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int agdegree(IntPtr graph, IntPtr node, int inset, int outset);
    [DllImport(CGraphLibNameWindows, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int agdelete(IntPtr graph, IntPtr item);
    [DllImport(CGraphLibNameWindows, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr agfstedge(IntPtr graph, IntPtr node);
    [DllImport(CGraphLibNameWindows, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr agfstin(IntPtr graph, IntPtr node);
    [DllImport(CGraphLibNameWindows, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr agfstnode(IntPtr graph);
    [DllImport(CGraphLibNameWindows, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr agfstout(IntPtr graph, IntPtr node);
    [DllImport(CGraphLibNameWindows, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr agfstsubg(IntPtr graph);
    [DllImport(CGraphLibNameWindows, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr agget(IntPtr obj, IntPtr name);
    [DllImport(CGraphLibNameWindows, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int agisdirected(IntPtr ptr);
    [DllImport(CGraphLibNameWindows, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int agisstrict(IntPtr ptr);
    [DllImport(CGraphLibNameWindows, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int agisundirected(IntPtr ptr);
    [DllImport(CGraphLibNameWindows, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr agnameof(IntPtr obj);

    [DllImport(CGraphLibNameWindows, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr agnode(IntPtr graph, IntPtr name, int create);
    [DllImport(CGraphLibNameWindows, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr agnxtattr(IntPtr obj, int kind, IntPtr attribute);
    [DllImport(CGraphLibNameWindows, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr agnxtedge(IntPtr graph, IntPtr edge, IntPtr node);
    [DllImport(CGraphLibNameWindows, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr agnxtin(IntPtr graph, IntPtr edge);
    [DllImport(CGraphLibNameWindows, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr agnxtnode(IntPtr graph, IntPtr node);
    [DllImport(CGraphLibNameWindows, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr agnxtout(IntPtr graph, IntPtr edge);
    [DllImport(CGraphLibNameWindows, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr agnxtsubg(IntPtr graph);

    [DllImport(CGraphLibNameWindows, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr agparent(IntPtr obj);
    [DllImport(CGraphLibNameWindows, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr agroot(IntPtr obj);
    [DllImport(CGraphLibNameWindows, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void agsafeset(IntPtr obj, IntPtr name, IntPtr val, IntPtr deflt);
    [DllImport(CGraphLibNameWindows, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void agset(IntPtr obj, IntPtr name, IntPtr value);
    [DllImport(CGraphLibNameWindows, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr agstrdup_html(IntPtr obj, IntPtr html);

    [DllImport(CGraphLibNameWindows, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr agsubedge(IntPtr graph, IntPtr edge, int create);

    [DllImport(CGraphLibNameWindows, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr agsubg(IntPtr graph, IntPtr name, int create);
    [DllImport(CGraphLibNameWindows, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr agsubnode(IntPtr graph, IntPtr node, int create);

    [DllImport(GvcLibNameWindows, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr gvContext();
    [DllImport(GvcLibNameWindows, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int gvFreeContext(IntPtr gvc);
    [DllImport(GvcLibNameWindows, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int gvFreeLayout(IntPtr gvc, IntPtr graph);
    [DllImport(GvcLibNameWindows, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int gvLayout(IntPtr gvc, IntPtr graph, IntPtr engine);
    [DllImport(GvcLibNameWindows, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int gvRender(IntPtr gvc, IntPtr graph, IntPtr format, IntPtr @out);
    [DllImport(GvcLibNameWindows, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int gvRenderFilename(IntPtr gvc, IntPtr graph, IntPtr format, IntPtr filename);
}