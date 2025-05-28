using System;
using System.Runtime.InteropServices;

namespace Rubjerg.Graphviz.FFI;

using static Marshaling;
using static Platform;

/// <summary>
/// Graphviz is thread unsafe, so we wrap all function calls inside a lock to make sure we don't run into
/// issues caused by multiple threads accessing the graphviz datastructures (like the GC executing a destructor).
/// </summary>
internal static class GraphvizFFI
{
    private static readonly object _mutex = new object();

    public static IntPtr GvContext()
    {
        lock (_mutex)
        {
            return IsWindows ? GraphvizLibWindows.gvContext() : GraphvizLibLinux.gvContext();
        }
    }
    public static int GvFreeContext(IntPtr gvc)
    {
        lock (_mutex)
        {
            return IsWindows ? GraphvizLibWindows.gvFreeContext(gvc) : GraphvizLibLinux.gvFreeContext(gvc);
        }
    }
    public static int GvLayout(IntPtr gvc, IntPtr graph, string engine)
    {
        lock (_mutex)
        {
            return MarshalToUtf8(engine, enginePtr => IsWindows ? GraphvizLibWindows.gvLayout(gvc, graph, enginePtr) : GraphvizLibLinux.gvLayout(gvc, graph, enginePtr));
        }
    }
    public static int GvFreeLayout(IntPtr gvc, IntPtr graph)
    {
        lock (_mutex)
        {
            return IsWindows ? GraphvizLibWindows.gvFreeLayout(gvc, graph) : GraphvizLibLinux.gvFreeLayout(gvc, graph);
        }
    }
    public static int GvRender(IntPtr gvc, IntPtr graph, string? format, IntPtr @out)
    {
        lock (_mutex)
        {
            return MarshalToUtf8(format, formatPtr => IsWindows ? GraphvizLibWindows.gvRender(gvc, graph, formatPtr, @out) : GraphvizLibLinux.gvRender(gvc, graph, formatPtr, @out));
        }
    }
    public static int GvRenderFilename(IntPtr gvc, IntPtr graph, string? format, string? filename)
    {
        lock (_mutex)
        {
            return MarshalToUtf8(format, formatPtr => MarshalToUtf8(filename, filenamePtr => IsWindows ? GraphvizLibWindows.gvRenderFilename(gvc, graph, formatPtr, filenamePtr) : GraphvizLibLinux.gvRenderFilename(gvc, graph, formatPtr, filenamePtr)));
        }
    }
    public static IntPtr Agnode(IntPtr graph, string? name, int create)
    {
        lock (_mutex)
        {
            return MarshalToUtf8(name, namePtr => IsWindows ? GraphvizLibWindows.agnode(graph, namePtr, create) : GraphvizLibLinux.agnode(graph, namePtr, create));
        }
    }
    public static int Agdegree(IntPtr graph, IntPtr node, int inset, int outset)
    {
        lock (_mutex)
        {
            return IsWindows ? GraphvizLibWindows.agdegree(graph, node, inset, outset) : GraphvizLibLinux.agdegree(graph, node, inset, outset);
        }
    }
    public static IntPtr Agfstout(IntPtr graph, IntPtr node)
    {
        lock (_mutex)
        {
            return IsWindows ? GraphvizLibWindows.agfstout(graph, node) : GraphvizLibLinux.agfstout(graph, node);
        }
    }
    public static IntPtr Agnxtout(IntPtr graph, IntPtr edge)
    {
        lock (_mutex)
        {
            return IsWindows ? GraphvizLibWindows.agnxtout(graph, edge) : GraphvizLibLinux.agnxtout(graph, edge);
        }
    }
    public static IntPtr Agfstin(IntPtr graph, IntPtr node)
    {
        lock (_mutex)
        {
            return IsWindows ? GraphvizLibWindows.agfstin(graph, node) : GraphvizLibLinux.agfstin(graph, node);
        }
    }
    public static IntPtr Agnxtin(IntPtr graph, IntPtr edge)
    {
        lock (_mutex)
        {
            return IsWindows ? GraphvizLibWindows.agnxtin(graph, edge) : GraphvizLibLinux.agnxtin(graph, edge);
        }
    }
    public static IntPtr Agfstedge(IntPtr graph, IntPtr node)
    {
        lock (_mutex)
        {
            return IsWindows ? GraphvizLibWindows.agfstedge(graph, node) : GraphvizLibLinux.agfstedge(graph, node);
        }
    }
    public static IntPtr Agnxtedge(IntPtr graph, IntPtr edge, IntPtr node)
    {
        lock (_mutex)
        {
            return IsWindows ? GraphvizLibWindows.agnxtedge(graph, edge, node) : GraphvizLibLinux.agnxtedge(graph, edge, node);
        }
    }
    public static void Agattr(IntPtr graph, int type, string name, string deflt)
    {
        lock (_mutex)
        {
            MarshalToUtf8(deflt, defltPtr =>
            MarshalToUtf8(name, namePtr =>
            {
                if (IsWindows)
                    GraphvizLibWindows.agattr(graph, type, namePtr, defltPtr);
                else
                    GraphvizLibLinux.agattr(graph, type, namePtr, defltPtr);
            }));
        }
    }
    public static void AgattrHtml(IntPtr graph, int type, string name, string deflt)
    {
        lock (_mutex)
        {
            MarshalToUtf8(name, namePtr =>
            MarshalToUtf8(deflt, defltPtr =>
            {
                if (IsWindows)
                {
                    var htmlPtr = GraphvizLibWindows.agstrdup_html(GraphvizLibWindows.agroot(graph), defltPtr);
                    GraphvizLibWindows.agattr(graph, type, namePtr, htmlPtr);
                }
                else
                {
                    var htmlPtr = GraphvizLibLinux.agstrdup_html(GraphvizLibLinux.agroot(graph), defltPtr);
                    GraphvizLibLinux.agattr(graph, type, namePtr, htmlPtr);
                }
            }));
        }
    }

    public static void Agset(IntPtr obj, string name, string value)
    {
        lock (_mutex)
        {
            MarshalToUtf8(name, namePtr =>
            MarshalToUtf8(value, valuePtr =>
            {
                if (IsWindows)
                    GraphvizLibWindows.agset(obj, namePtr, valuePtr);
                else
                    GraphvizLibLinux.agset(obj, namePtr, valuePtr);
            }));
        }
    }

    public static void AgsetHtml(IntPtr obj, string name, string value)
    {
        lock (_mutex)
        {
            MarshalToUtf8(name, namePtr =>
            MarshalToUtf8(value, valuePtr =>
            {
                if (IsWindows)
                {
                    var htmlPtr = GraphvizLibWindows.agstrdup_html(GraphvizLibWindows.agroot(obj), valuePtr);
                    GraphvizLibWindows.agset(obj, namePtr, htmlPtr);
                }
                else
                {
                    var htmlPtr = GraphvizLibLinux.agstrdup_html(GraphvizLibLinux.agroot(obj), valuePtr);
                    GraphvizLibLinux.agset(obj, namePtr, htmlPtr);
                }
            }));
        }
    }

    public static void Agsafeset(IntPtr obj, string name, string? val, string? deflt)
    {
        lock (_mutex)
        {
            MarshalToUtf8(name, namePtr => MarshalToUtf8(val, valPtr => MarshalToUtf8(deflt, defltPtr =>
            {
                if (IsWindows)
                    GraphvizLibWindows.agsafeset(obj, namePtr, valPtr, defltPtr);
                else
                    GraphvizLibLinux.agsafeset(obj, namePtr, valPtr, defltPtr);
            })));
        }
    }
    public static void AgsafesetHtml(IntPtr obj, string name, string? val, string? deflt)
    {
        lock (_mutex)
        {
            MarshalToUtf8(name, namePtr => MarshalToUtf8(val, valPtr => MarshalToUtf8(deflt, defltPtr =>
            {
                if (IsWindows)
                {
                    var htmlPtr = GraphvizLibWindows.agstrdup_html(GraphvizLibWindows.agroot(obj), defltPtr);
                    GraphvizLibWindows.agsafeset(obj, namePtr, valPtr, htmlPtr);
                }
                else
                {
                    var htmlPtr = GraphvizLibLinux.agstrdup_html(GraphvizLibLinux.agroot(obj), defltPtr);
                    GraphvizLibLinux.agsafeset(obj, namePtr, valPtr, htmlPtr);
                }
            })));
        }
    }
    public static IntPtr Agroot(IntPtr obj)
    {
        lock (_mutex)
        {
            return IsWindows ? GraphvizLibWindows.agroot(obj) : GraphvizLibLinux.agroot(obj);
        }
    }
    public static IntPtr Agnxtattr(IntPtr obj, int kind, IntPtr attribute)
    {
        lock (_mutex)
        {
            return IsWindows ? GraphvizLibWindows.agnxtattr(obj, kind, attribute) : GraphvizLibLinux.agnxtattr(obj, kind, attribute);
        }
    }
    public static int Agcopyattr(IntPtr from, IntPtr to)
    {
        lock (_mutex)
        {
            return IsWindows ? GraphvizLibWindows.agcopyattr(from, to) : GraphvizLibLinux.agcopyattr(from, to);
        }
    }
    public static bool Ageqedge(IntPtr edge1, IntPtr edge2)
    {
        lock (_mutex)
        {
            return GraphvizWrapperLib.rj_ageqedge(edge1, edge2);
        }
    }
    public static IntPtr Agtail(IntPtr node)
    {
        lock (_mutex)
        {
            return GraphvizWrapperLib.rj_agtail(node);
        }
    }
    public static IntPtr Aghead(IntPtr node)
    {
        lock (_mutex)
        {
            return GraphvizWrapperLib.rj_aghead(node);
        }
    }
    public static IntPtr Agedge(IntPtr graph, IntPtr tail, IntPtr head, string? name, int create)
    {
        lock (_mutex)
        {
            return MarshalToUtf8(name, namePtr => IsWindows ? GraphvizLibWindows.agedge(graph, tail, head, namePtr, create) : GraphvizLibLinux.agedge(graph, tail, head, namePtr, create));
        }
    }
    public static IntPtr Agmkin(IntPtr edge)
    {
        lock (_mutex)
        {
            return GraphvizWrapperLib.rj_agmkin(edge);
        }
    }
    public static IntPtr Agmkout(IntPtr edge)
    {
        lock (_mutex)
        {
            return GraphvizWrapperLib.rj_agmkout(edge);
        }
    }
    public static IntPtr Agparent(IntPtr obj)
    {
        lock (_mutex)
        {
            return IsWindows ? GraphvizLibWindows.agparent(obj) : GraphvizLibLinux.agparent(obj);
        }
    }
    public static int Agclose(IntPtr graph)
    {
        lock (_mutex)
        {
            return IsWindows ? GraphvizLibWindows.agclose(graph) : GraphvizLibLinux.agclose(graph);
        }
    }
    public static int Agdelete(IntPtr graph, IntPtr item)
    {
        lock (_mutex)
        {
            return IsWindows ? GraphvizLibWindows.agdelete(graph, item) : GraphvizLibLinux.agdelete(graph, item);
        }
    }
    public static IntPtr Agfstnode(IntPtr graph)
    {
        lock (_mutex)
        {
            return IsWindows ? GraphvizLibWindows.agfstnode(graph) : GraphvizLibLinux.agfstnode(graph);
        }
    }
    public static IntPtr Agnxtnode(IntPtr graph, IntPtr node)
    {
        lock (_mutex)
        {
            return IsWindows ? GraphvizLibWindows.agnxtnode(graph, node) : GraphvizLibLinux.agnxtnode(graph, node);
        }
    }
    public static int Agcontains(IntPtr graph, IntPtr obj)
    {
        lock (_mutex)
        {
            return IsWindows ? GraphvizLibWindows.agcontains(graph, obj) : GraphvizLibLinux.agcontains(graph, obj);
        }
    }
    public static IntPtr Agsubg(IntPtr graph, string? name, int create)
    {
        lock (_mutex)
        {
            return MarshalToUtf8(name, namePtr => IsWindows ? GraphvizLibWindows.agsubg(graph, namePtr, create) : GraphvizLibLinux.agsubg(graph, namePtr, create));
        }
    }
    public static IntPtr Agfstsubg(IntPtr graph)
    {
        lock (_mutex)
        {
            return IsWindows ? GraphvizLibWindows.agfstsubg(graph) : GraphvizLibLinux.agfstsubg(graph);
        }
    }
    public static IntPtr Agnxtsubg(IntPtr graph)
    {
        lock (_mutex)
        {
            return IsWindows ? GraphvizLibWindows.agnxtsubg(graph) : GraphvizLibLinux.agnxtsubg(graph);
        }
    }
    public static int Agisstrict(IntPtr ptr)
    {
        lock (_mutex)
        {
            return IsWindows ? GraphvizLibWindows.agisstrict(ptr) : GraphvizLibLinux.agisstrict(ptr);
        }
    }
    public static int Agisdirected(IntPtr ptr)
    {
        lock (_mutex)
        {
            return IsWindows ? GraphvizLibWindows.agisdirected(ptr) : GraphvizLibLinux.agisdirected(ptr);
        }
    }
    public static int Agisundirected(IntPtr ptr)
    {
        lock (_mutex)
        {
            return IsWindows ? GraphvizLibWindows.agisundirected(ptr) : GraphvizLibLinux.agisundirected(ptr);
        }
    }
    public static IntPtr Agsubedge(IntPtr graph, IntPtr edge, int create)
    {
        lock (_mutex)
        {
            return IsWindows ? GraphvizLibWindows.agsubedge(graph, edge, create) : GraphvizLibLinux.agsubedge(graph, edge, create);
        }
    }
    public static IntPtr Agsubnode(IntPtr graph, IntPtr node, int create)
    {
        lock (_mutex)
        {
            return IsWindows ? GraphvizLibWindows.agsubnode(graph, node, create) : GraphvizLibLinux.agsubnode(graph, node, create);
        }
    }
    public static IntPtr EdgeLabel(IntPtr node)
    {
        lock (_mutex)
        {
            return GraphvizWrapperLib.edge_label(node);
        }
    }
    public static string? Rjagmemwrite(IntPtr graph)
    {
        lock (_mutex)
        {
            var strPtr = GraphvizWrapperLib.rj_agmemwrite(graph);
            return MarshalFromUtf8(strPtr, true);
        }
    }
    public static IntPtr GraphLabel(IntPtr node)
    {
        lock (_mutex)
        {
            return GraphvizWrapperLib.graph_label(node);
        }
    }
    public static string? Agget(IntPtr obj, string name)
    {
        lock (_mutex)
        {
            return MarshalToUtf8(name, namePtr => MarshalFromUtf8(IsWindows ? GraphvizLibWindows.agget(obj, namePtr) : GraphvizLibLinux.agget(obj, namePtr), false));
        }
    }
    public static string? Rjagnameof(IntPtr obj)
    {
        lock (_mutex)
        {
            return MarshalFromUtf8(IsWindows ? GraphvizLibWindows.agnameof(obj) : GraphvizLibLinux.agnameof(obj), false);
        }
    }
    public static void CloneAttributeDeclarations(IntPtr graphfrom, IntPtr graphto)
    {
        lock (_mutex)
        {
            GraphvizWrapperLib.clone_attribute_declarations(graphfrom, graphto);
        }
    }
    public static string? ImsymKey(IntPtr sym)
    {
        lock (_mutex)
        {
            return MarshalFromUtf8(GraphvizWrapperLib.rj_sym_key(sym), false);
        }
    }
    public static double LabelX(IntPtr label)
    {
        lock (_mutex)
        {
            return GraphvizWrapperLib.label_x(label);
        }
    }
    public static double LabelY(IntPtr label)
    {
        lock (_mutex)
        {
            return GraphvizWrapperLib.label_y(label);
        }
    }
    public static double LabelWidth(IntPtr label)
    {
        lock (_mutex)
        {
            return GraphvizWrapperLib.label_width(label);
        }
    }
    public static double LabelHeight(IntPtr label)
    {
        lock (_mutex)
        {
            return GraphvizWrapperLib.label_height(label);
        }
    }
    public static string? LabelText(IntPtr label)
    {
        lock (_mutex)
        {
            return MarshalFromUtf8(GraphvizWrapperLib.label_text(label), false);
        }
    }
    public static double LabelFontsize(IntPtr label)
    {
        lock (_mutex)
        {
            return GraphvizWrapperLib.label_fontsize(label);
        }
    }
    public static string? LabelFontname(IntPtr label)
    {
        lock (_mutex)
        {
            return MarshalFromUtf8(GraphvizWrapperLib.label_fontname(label), false);
        }
    }
    public static double NodeX(IntPtr node)
    {
        lock (_mutex)
        {
            return GraphvizWrapperLib.node_x(node);
        }
    }
    public static double NodeY(IntPtr node)
    {
        lock (_mutex)
        {
            return GraphvizWrapperLib.node_y(node);
        }
    }
    public static double NodeWidth(IntPtr node)
    {
        lock (_mutex)
        {
            return GraphvizWrapperLib.node_width(node);
        }
    }
    public static double NodeHeight(IntPtr node)
    {
        lock (_mutex)
        {
            return GraphvizWrapperLib.node_height(node);
        }
    }
    public static IntPtr NodeLabel(IntPtr node)
    {
        lock (_mutex)
        {
            return GraphvizWrapperLib.node_label(node);
        }
    }
    public static void ConvertToUndirected(IntPtr graph)
    {
        lock (_mutex)
        {
            GraphvizWrapperLib.convert_to_undirected(graph);
        }
    }
    public static IntPtr Rjagmemread(string input)
    {
        lock (_mutex)
        {
            return MarshalToUtf8(input, GraphvizWrapperLib.rj_agmemread);
        }
    }
    public static IntPtr Rjagopen(string? name, int graphtype)
    {
        lock (_mutex)
        {
            return MarshalToUtf8(name, namePtr => GraphvizWrapperLib.rj_agopen(namePtr, graphtype));
        }
    }


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
    static GraphvizFFI()
    {
        // We initialize the gvc here before interacting with graphviz
        // https://gitlab.com/graphviz/graphviz/-/issues/2434
        GVC = GvContext();
    }

}
