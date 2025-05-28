using System;
using System.Runtime.InteropServices;

namespace Rubjerg.Graphviz.FFI;

using static Constants;

internal static class GraphvizLibLinux
{
    [DllImport(CGraphLibNameLinux, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void agattr(IntPtr graph, int type, IntPtr name, IntPtr deflt);

    [DllImport(CGraphLibNameLinux, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr agedge(IntPtr graph, IntPtr tail, IntPtr head, IntPtr name, int create);

    [DllImport(CGraphLibNameLinux, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int agclose(IntPtr graph);
    [DllImport(CGraphLibNameLinux, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int agcontains(IntPtr graph, IntPtr obj);
    [DllImport(CGraphLibNameLinux, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int agcopyattr(IntPtr from, IntPtr to);
    [DllImport(CGraphLibNameLinux, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int agdegree(IntPtr graph, IntPtr node, int inset, int outset);
    [DllImport(CGraphLibNameLinux, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int agdelete(IntPtr graph, IntPtr item);
    [DllImport(CGraphLibNameLinux, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr agfstedge(IntPtr graph, IntPtr node);
    [DllImport(CGraphLibNameLinux, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr agfstin(IntPtr graph, IntPtr node);
    [DllImport(CGraphLibNameLinux, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr agfstnode(IntPtr graph);
    [DllImport(CGraphLibNameLinux, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr agfstout(IntPtr graph, IntPtr node);
    [DllImport(CGraphLibNameLinux, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr agfstsubg(IntPtr graph);
    [DllImport(CGraphLibNameLinux, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr agget(IntPtr obj, IntPtr name);
    [DllImport(CGraphLibNameLinux, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int agisdirected(IntPtr ptr);
    [DllImport(CGraphLibNameLinux, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int agisstrict(IntPtr ptr);
    [DllImport(CGraphLibNameLinux, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int agisundirected(IntPtr ptr);
    [DllImport(CGraphLibNameLinux, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr agnameof(IntPtr obj);

    [DllImport(CGraphLibNameLinux, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr agnode(IntPtr graph, IntPtr name, int create);
    [DllImport(CGraphLibNameLinux, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr agnxtattr(IntPtr obj, int kind, IntPtr attribute);
    [DllImport(CGraphLibNameLinux, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr agnxtedge(IntPtr graph, IntPtr edge, IntPtr node);
    [DllImport(CGraphLibNameLinux, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr agnxtin(IntPtr graph, IntPtr edge);
    [DllImport(CGraphLibNameLinux, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr agnxtnode(IntPtr graph, IntPtr node);
    [DllImport(CGraphLibNameLinux, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr agnxtout(IntPtr graph, IntPtr edge);
    [DllImport(CGraphLibNameLinux, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr agnxtsubg(IntPtr graph);

    [DllImport(CGraphLibNameLinux, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr agparent(IntPtr obj);
    [DllImport(CGraphLibNameLinux, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr agroot(IntPtr obj);
    [DllImport(CGraphLibNameLinux, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void agsafeset(IntPtr obj, IntPtr name, IntPtr val, IntPtr deflt);
    [DllImport(CGraphLibNameLinux, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void agset(IntPtr obj, IntPtr name, IntPtr value);
    [DllImport(CGraphLibNameLinux, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr agstrdup_html(IntPtr obj, IntPtr html);

    [DllImport(CGraphLibNameLinux, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr agsubedge(IntPtr graph, IntPtr edge, int create);

    [DllImport(CGraphLibNameLinux, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr agsubg(IntPtr graph, IntPtr name, int create);
    [DllImport(CGraphLibNameLinux, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr agsubnode(IntPtr graph, IntPtr node, int create);

    [DllImport(GvcLibNameLinux, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr gvContext();
    [DllImport(GvcLibNameLinux, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int gvFreeContext(IntPtr gvc);
    [DllImport(GvcLibNameLinux, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int gvFreeLayout(IntPtr gvc, IntPtr graph);
    [DllImport(GvcLibNameLinux, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int gvLayout(IntPtr gvc, IntPtr graph, IntPtr engine);
    [DllImport(GvcLibNameLinux, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int gvRender(IntPtr gvc, IntPtr graph, IntPtr format, IntPtr @out);
    [DllImport(GvcLibNameLinux, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int gvRenderFilename(IntPtr gvc, IntPtr graph, IntPtr format, IntPtr filename);
}