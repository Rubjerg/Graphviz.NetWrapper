using System;
using System.Runtime.InteropServices;

namespace Rubjerg.Graphviz;

using static Marshaling;

/// <summary>
/// Graphviz is thread unsafe, so we wrap all function calls inside a lock to make sure we don't run into
/// issues caused by multiple threads accessing the graphviz datastructures (like the GC executing a destructor).
/// </summary>
internal static class ForeignFunctionInterface
{
    private static readonly object _mutex = new object();

    public static IntPtr GvContext()
    {
        lock (_mutex)
        {
            return gvContext();
        }
    }
    public static int GvFreeContext(IntPtr gvc)
    {
        lock (_mutex)
        {
            return gvFreeContext(gvc);
        }
    }
    public static int GvLayout(IntPtr gvc, IntPtr graph, string engine)
    {
        lock (_mutex)
        {
            return MarshalToUtf8(engine, enginePtr => gvLayout(gvc, graph, enginePtr));
        }
    }
    public static int GvFreeLayout(IntPtr gvc, IntPtr graph)
    {
        lock (_mutex)
        {
            return gvFreeLayout(gvc, graph);
        }
    }
    public static int GvRender(IntPtr gvc, IntPtr graph, string? format, IntPtr @out)
    {
        lock (_mutex)
        {
            return MarshalToUtf8(format, formatPtr => gvRender(gvc, graph, formatPtr, @out));
        }
    }
    public static int GvRenderFilename(IntPtr gvc, IntPtr graph, string? format, string? filename)
    {
        lock (_mutex)
        {
            return
                MarshalToUtf8(format, formatPtr =>
                MarshalToUtf8(filename, filenamePtr =>
                gvRenderFilename(gvc, graph, formatPtr, filenamePtr)));
        }
    }
    public static IntPtr Agnode(IntPtr graph, string? name, int create)
    {
        lock (_mutex)
        {
            return MarshalToUtf8(name, namePtr => agnode(graph, namePtr, create));
        }
    }
    public static int Agdegree(IntPtr graph, IntPtr node, int inset, int outset)
    {
        lock (_mutex)
        {
            return agdegree(graph, node, inset, outset);
        }
    }
    public static IntPtr Agfstout(IntPtr graph, IntPtr node)
    {
        lock (_mutex)
        {
            return agfstout(graph, node);
        }
    }
    public static IntPtr Agnxtout(IntPtr graph, IntPtr edge)
    {
        lock (_mutex)
        {
            return agnxtout(graph, edge);
        }
    }
    public static IntPtr Agfstin(IntPtr graph, IntPtr node)
    {
        lock (_mutex)
        {
            return agfstin(graph, node);
        }
    }
    public static IntPtr Agnxtin(IntPtr graph, IntPtr edge)
    {
        lock (_mutex)
        {
            return agnxtin(graph, edge);
        }
    }
    public static IntPtr Agfstedge(IntPtr graph, IntPtr node)
    {
        lock (_mutex)
        {
            return agfstedge(graph, node);
        }
    }
    public static IntPtr Agnxtedge(IntPtr graph, IntPtr edge, IntPtr node)
    {
        lock (_mutex)
        {
            return agnxtedge(graph, edge, node);
        }
    }
    public static void Agattr(IntPtr graph, int type, string name, string deflt)
    {
        lock (_mutex)
        {
            MarshalToUtf8(deflt, defltPtr =>
            MarshalToUtf8(name, namePtr =>
            agattr(graph, type, namePtr, defltPtr)));
        }
    }
    public static void AgattrHtml(IntPtr graph, int type, string name, string deflt)
    {
        lock (_mutex)
        {
            MarshalToUtf8(name, namePtr =>
            MarshalToUtf8(deflt, defltPtr =>
            {
                var htmlPtr = agstrdup_html(agroot(graph), defltPtr);
                agattr(graph, type, namePtr, htmlPtr);
            }));
        }
    }

    public static void Agset(IntPtr obj, string name, string value)
    {
        lock (_mutex)
        {
            MarshalToUtf8(name, namePtr =>
            MarshalToUtf8(value, valuePtr =>
            agset(obj, namePtr, valuePtr)));
        }
    }

    public static void AgsetHtml(IntPtr obj, string name, string value)
    {
        lock (_mutex)
        {
            MarshalToUtf8(name, namePtr =>
            MarshalToUtf8(value, valuePtr =>
            {
                var htmlPtr = agstrdup_html(agroot(obj), valuePtr);
                agset(obj, namePtr, htmlPtr);
            }));
        }
    }

    public static void Agsafeset(IntPtr obj, string name, string? val, string? deflt)
    {
        lock (_mutex)
        {
            MarshalToUtf8(name, namePtr =>
            MarshalToUtf8(val, valPtr =>
            MarshalToUtf8(deflt, defltPtr =>
            agsafeset(obj, namePtr, valPtr, defltPtr))));
        }
    }
    public static void AgsafesetHtml(IntPtr obj, string name, string? val, string? deflt)
    {
        lock (_mutex)
        {
            MarshalToUtf8(name, namePtr =>
            MarshalToUtf8(val, valPtr =>
            MarshalToUtf8(deflt, defltPtr =>
            {
                var htmlPtr = agstrdup_html(agroot(obj), defltPtr);
                agsafeset(obj, namePtr, valPtr, htmlPtr);
            })));
        }
    }
    public static IntPtr Agroot(IntPtr obj)
    {
        lock (_mutex)
        {
            return agroot(obj);
        }
    }
    public static IntPtr Agnxtattr(IntPtr obj, int kind, IntPtr attribute)
    {
        lock (_mutex)
        {
            return agnxtattr(obj, kind, attribute);
        }
    }
    public static int Agcopyattr(IntPtr from, IntPtr to)
    {
        lock (_mutex)
        {
            return agcopyattr(from, to);
        }
    }
    public static bool Ageqedge(IntPtr edge1, IntPtr edge2)
    {
        lock (_mutex)
        {
            return rj_ageqedge(edge1, edge2);
        }
    }
    public static IntPtr Agtail(IntPtr node)
    {
        lock (_mutex)
        {
            return rj_agtail(node);
        }
    }
    public static IntPtr Aghead(IntPtr node)
    {
        lock (_mutex)
        {
            return rj_aghead(node);
        }
    }
    public static IntPtr Agedge(IntPtr graph, IntPtr tail, IntPtr head, string? name, int create)
    {
        lock (_mutex)
        {
            return MarshalToUtf8(name, namePtr => agedge(graph, tail, head, namePtr, create));
        }
    }
    public static IntPtr Agmkin(IntPtr edge)
    {
        lock (_mutex)
        {
            return rj_agmkin(edge);
        }
    }
    public static IntPtr Agmkout(IntPtr edge)
    {
        lock (_mutex)
        {
            return rj_agmkout(edge);
        }
    }
    public static IntPtr Agparent(IntPtr obj)
    {
        lock (_mutex)
        {
            return agparent(obj);
        }
    }
    public static int Agclose(IntPtr graph)
    {
        lock (_mutex)
        {
            return agclose(graph);
        }
    }
    public static int Agdelete(IntPtr graph, IntPtr item)
    {
        lock (_mutex)
        {
            return agdelete(graph, item);
        }
    }
    public static IntPtr Agfstnode(IntPtr graph)
    {
        lock (_mutex)
        {
            return agfstnode(graph);
        }
    }
    public static IntPtr Agnxtnode(IntPtr graph, IntPtr node)
    {
        lock (_mutex)
        {
            return agnxtnode(graph, node);
        }
    }
    public static int Agcontains(IntPtr graph, IntPtr obj)
    {
        lock (_mutex)
        {
            return agcontains(graph, obj);
        }
    }
    public static IntPtr Agsubg(IntPtr graph, string? name, int create)
    {
        lock (_mutex)
        {
            return MarshalToUtf8(name, namePtr => agsubg(graph, namePtr, create));
        }
    }
    public static IntPtr Agfstsubg(IntPtr graph)
    {
        lock (_mutex)
        {
            return agfstsubg(graph);
        }
    }
    public static IntPtr Agnxtsubg(IntPtr graph)
    {
        lock (_mutex)
        {
            return agnxtsubg(graph);
        }
    }
    public static int Agisstrict(IntPtr ptr)
    {
        lock (_mutex)
        {
            return agisstrict(ptr);
        }
    }
    public static int Agisdirected(IntPtr ptr)
    {
        lock (_mutex)
        {
            return agisdirected(ptr);
        }
    }
    public static int Agisundirected(IntPtr ptr)
    {
        lock (_mutex)
        {
            return agisundirected(ptr);
        }
    }
    public static IntPtr Agsubedge(IntPtr graph, IntPtr edge, int create)
    {
        lock (_mutex)
        {
            return agsubedge(graph, edge, create);
        }
    }
    public static IntPtr Agsubnode(IntPtr graph, IntPtr node, int create)
    {
        lock (_mutex)
        {
            return agsubnode(graph, node, create);
        }
    }
    public static IntPtr EdgeLabel(IntPtr node)
    {
        lock (_mutex)
        {
            return edge_label(node);
        }
    }
    public static string? Rjagmemwrite(IntPtr graph)
    {
        lock (_mutex)
        {
            var strPtr = rj_agmemwrite(graph);
            return MarshalFromUtf8(strPtr, true);
        }
    }
    public static IntPtr GraphLabel(IntPtr node)
    {
        lock (_mutex)
        {
            return graph_label(node);
        }
    }
    public static string? Agget(IntPtr obj, string name)
    {
        lock (_mutex)
        {
            return MarshalToUtf8(name, namePtr => MarshalFromUtf8(agget(obj, namePtr), false));
        }
    }
    public static string? Rjagnameof(IntPtr obj)
    {
        lock (_mutex)
        {
            return MarshalFromUtf8(agnameof(obj), false);
        }
    }
    public static void CloneAttributeDeclarations(IntPtr graphfrom, IntPtr graphto)
    {
        lock (_mutex)
        {
            clone_attribute_declarations(graphfrom, graphto);
        }
    }
    public static string? ImsymKey(IntPtr sym)
    {
        lock (_mutex)
        {
            return MarshalFromUtf8(rj_sym_key(sym), false);
        }
    }
    public static double LabelX(IntPtr label)
    {
        lock (_mutex)
        {
            return label_x(label);
        }
    }
    public static double LabelY(IntPtr label)
    {
        lock (_mutex)
        {
            return label_y(label);
        }
    }
    public static double LabelWidth(IntPtr label)
    {
        lock (_mutex)
        {
            return label_width(label);
        }
    }
    public static double LabelHeight(IntPtr label)
    {
        lock (_mutex)
        {
            return label_height(label);
        }
    }
    public static string? LabelText(IntPtr label)
    {
        lock (_mutex)
        {
            return MarshalFromUtf8(label_text(label), false);
        }
    }
    public static double LabelFontsize(IntPtr label)
    {
        lock (_mutex)
        {
            return label_fontsize(label);
        }
    }
    public static string? LabelFontname(IntPtr label)
    {
        lock (_mutex)
        {
            return MarshalFromUtf8(label_fontname(label), false);
        }
    }
    public static double NodeX(IntPtr node)
    {
        lock (_mutex)
        {
            return node_x(node);
        }
    }
    public static double NodeY(IntPtr node)
    {
        lock (_mutex)
        {
            return node_y(node);
        }
    }
    public static double NodeWidth(IntPtr node)
    {
        lock (_mutex)
        {
            return node_width(node);
        }
    }
    public static double NodeHeight(IntPtr node)
    {
        lock (_mutex)
        {
            return node_height(node);
        }
    }
    public static IntPtr NodeLabel(IntPtr node)
    {
        lock (_mutex)
        {
            return node_label(node);
        }
    }
    public static void ConvertToUndirected(IntPtr graph)
    {
        lock (_mutex)
        {
            convert_to_undirected(graph);
        }
    }
    public static IntPtr Rjagmemread(string input)
    {
        lock (_mutex)
        {
            return MarshalToUtf8(input, rj_agmemread);
        }
    }
    public static IntPtr Rjagopen(string? name, int graphtype)
    {
        lock (_mutex)
        {
            return MarshalToUtf8(name, namePtr => rj_agopen(namePtr, graphtype));
        }
    }

    [DllImport("gvc.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr gvContext();
    [DllImport("gvc.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern int gvFreeContext(IntPtr gvc);
    [DllImport("gvc.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern int gvLayout(IntPtr gvc, IntPtr graph, IntPtr engine);
    [DllImport("gvc.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern int gvFreeLayout(IntPtr gvc, IntPtr graph);
    [DllImport("gvc.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern int gvRender(IntPtr gvc, IntPtr graph, IntPtr format, IntPtr @out);
    [DllImport("gvc.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern int gvRenderFilename(IntPtr gvc, IntPtr graph, IntPtr format, IntPtr filename);

    [DllImport("cgraph.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr agnode(IntPtr graph, IntPtr name, int create);
    [DllImport("cgraph.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern int agdegree(IntPtr graph, IntPtr node, int inset, int outset);
    [DllImport("cgraph.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr agfstout(IntPtr graph, IntPtr node);
    [DllImport("cgraph.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr agnxtout(IntPtr graph, IntPtr edge);
    [DllImport("cgraph.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr agfstin(IntPtr graph, IntPtr node);
    [DllImport("cgraph.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr agnxtin(IntPtr graph, IntPtr edge);
    [DllImport("cgraph.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr agfstedge(IntPtr graph, IntPtr node);
    [DllImport("cgraph.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr agnxtedge(IntPtr graph, IntPtr edge, IntPtr node);

    [DllImport("cgraph.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern void agattr(IntPtr graph, int type, IntPtr name, IntPtr deflt);
    [DllImport("cgraph.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern void agset(IntPtr obj, IntPtr name, IntPtr value);
    [DllImport("cgraph.dll", SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr agstrdup_html(IntPtr obj, IntPtr html);
    [DllImport("cgraph.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern void agsafeset(IntPtr obj, IntPtr name, IntPtr val, IntPtr deflt);
    [DllImport("cgraph.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr agroot(IntPtr obj);
    [DllImport("cgraph.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr agnxtattr(IntPtr obj, int kind, IntPtr attribute);
    [DllImport("cgraph.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern int agcopyattr(IntPtr from, IntPtr to);

    [DllImport("GraphvizWrapper.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.U1)]
    private static extern bool rj_ageqedge(IntPtr edge1, IntPtr edge2);
    [DllImport("GraphvizWrapper.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr rj_agtail(IntPtr node);
    [DllImport("GraphvizWrapper.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr rj_aghead(IntPtr node);
    [DllImport("cgraph.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr agedge(IntPtr graph, IntPtr tail, IntPtr head, IntPtr name, int create);
    [DllImport("GraphvizWrapper.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr rj_agmkin(IntPtr edge);
    [DllImport("GraphvizWrapper.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr rj_agmkout(IntPtr edge);

    [DllImport("cgraph.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr agparent(IntPtr obj);

    [DllImport("cgraph.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern int agclose(IntPtr graph);
    [DllImport("cgraph.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern int agdelete(IntPtr graph, IntPtr item);
    [DllImport("cgraph.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr agfstnode(IntPtr graph);
    [DllImport("cgraph.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr agnxtnode(IntPtr graph, IntPtr node);
    [DllImport("cgraph.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern int agcontains(IntPtr graph, IntPtr obj);

    [DllImport("cgraph.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr agsubg(IntPtr graph, IntPtr name, int create);
    [DllImport("cgraph.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr agfstsubg(IntPtr graph);
    [DllImport("cgraph.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr agnxtsubg(IntPtr graph);
    [DllImport("cgraph.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern int agisstrict(IntPtr ptr);
    [DllImport("cgraph.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern int agisdirected(IntPtr ptr);
    [DllImport("cgraph.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern int agisundirected(IntPtr ptr);

    [DllImport("cgraph.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr agsubedge(IntPtr graph, IntPtr edge, int create);
    [DllImport("cgraph.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr agsubnode(IntPtr graph, IntPtr node, int create);

    [DllImport("GraphvizWrapper.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr edge_label(IntPtr node);
    [DllImport("GraphvizWrapper.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr rj_agmemwrite(IntPtr graph);
    [DllImport("GraphvizWrapper.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr graph_label(IntPtr node);

    [DllImport("cgraph.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr agget(IntPtr obj, IntPtr name);
    [DllImport("cgraph.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr agnameof(IntPtr obj);
    [DllImport("GraphvizWrapper.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern void clone_attribute_declarations(IntPtr graphfrom, IntPtr graphto);
    [DllImport("GraphvizWrapper.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr rj_sym_key(IntPtr sym);

    [DllImport("GraphvizWrapper.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern double label_x(IntPtr label);
    [DllImport("GraphvizWrapper.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern double label_y(IntPtr label);
    [DllImport("GraphvizWrapper.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern double label_width(IntPtr label);
    [DllImport("GraphvizWrapper.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern double label_height(IntPtr label);
    [DllImport("GraphvizWrapper.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr label_text(IntPtr label);
    [DllImport("GraphvizWrapper.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern double label_fontsize(IntPtr label);
    [DllImport("GraphvizWrapper.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr label_fontname(IntPtr label);

    [DllImport("GraphvizWrapper.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern double node_x(IntPtr node);
    [DllImport("GraphvizWrapper.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern double node_y(IntPtr node);
    [DllImport("GraphvizWrapper.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern double node_width(IntPtr node);
    [DllImport("GraphvizWrapper.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern double node_height(IntPtr node);
    [DllImport("GraphvizWrapper.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr node_label(IntPtr node);

    [DllImport("GraphvizWrapper.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern void convert_to_undirected(IntPtr graph);
    [DllImport("GraphvizWrapper.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr rj_agmemread(IntPtr input);
    [DllImport("GraphvizWrapper.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr rj_agopen(IntPtr name, int graphtype);


    /// <summary>
    /// A GraphvizContext is used to store various layout
    /// information that is independent of a particular graph and
    /// its attributes.  It holds the data associated with plugins,
    /// parsed - command lines, script engines, and anything else
    /// with a scope potentially larger than one graph, up to the
    /// scope of the application. In addition, it maintains lists of
    /// the available layout algorithms and renderers; it also
    /// records the most recent layout algorithm applied to a graph.
    /// It can be used to specify multiple renderings of a given
    /// graph layout into different associated files.It is also used
    /// to store various global information used during rendering.
    /// There should be just one GVC created for the entire
    /// duration of an application. A single GVC value can be used
    /// with multiple graphs, though with only one graph at a
    /// time. In addition, if gvLayout() was invoked for a graph and
    /// GVC, then gvFreeLayout() should be called before using
    /// gvLayout() again, even on the same graph.
    /// </summary>
    public static IntPtr GVC { get; private set; }
    static ForeignFunctionInterface()
    {
        // We initialize the gvc here before interacting with graphviz
        // https://gitlab.com/graphviz/graphviz/-/issues/2434
        GVC = GvContext();
    }

    #region debugging and testing

    // .NET uses UnmanagedType.Bool by default for P/Invoke, but our C++ code uses UnmanagedType.U1
    [DllImport("GraphvizWrapper.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.U1)]
    public static extern bool echobool([MarshalAs(UnmanagedType.U1)] bool arg);
    [DllImport("GraphvizWrapper.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.U1)]
    public static extern bool return_true();
    [DllImport("GraphvizWrapper.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.U1)]
    public static extern bool return_false();
    [DllImport("GraphvizWrapper.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern int echoint(int arg);
    [DllImport("GraphvizWrapper.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern int return1();
    [DllImport("GraphvizWrapper.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern int return_1();

    public enum TestEnum
    {
        Val1, Val2, Val3, Val4, Val5
    }
    [DllImport("GraphvizWrapper.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern TestEnum return_enum1();
    [DllImport("GraphvizWrapper.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern TestEnum return_enum2();
    [DllImport("GraphvizWrapper.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern TestEnum return_enum5();
    [DllImport("GraphvizWrapper.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern TestEnum echo_enum(TestEnum e);

    [DllImport("GraphvizWrapper.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr echo_string(IntPtr str);
    [DllImport("GraphvizWrapper.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr return_empty_string();
    [DllImport("GraphvizWrapper.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr return_hello();
    [DllImport("GraphvizWrapper.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr return_copyright();

    public static string? EchoString(string? str)
    {
        return MarshalToUtf8(str, ptr =>
        {
            var returnPtr = echo_string(ptr);
            // echo_string gives us ownership over the string, which means that we have to free it.
            return MarshalFromUtf8(returnPtr, true);
        });
    }
    public static string? ReturnEmptyString() => MarshalFromUtf8(return_empty_string(), false);
    public static string? ReturnHello() => MarshalFromUtf8(return_hello(), false);
    public static string? ReturnCopyRight() => MarshalFromUtf8(return_copyright(), false);
    #endregion
}
