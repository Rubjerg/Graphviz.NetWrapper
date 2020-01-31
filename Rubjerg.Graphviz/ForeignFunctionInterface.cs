using System;
using System.Runtime.InteropServices;

namespace Rubjerg.Graphviz
{
    /// <summary>
    /// Graphviz is thread unsafe, so we wrap all function calls inside a lock to make sure we don't run into
    /// issues caused by multiple threads accessing the graphviz datastructures (like the GC executing a destructor).
    /// </summary>
    internal static class ForeignFunctionInterface
    {
        private static object Mutex = new object();

        public static IntPtr GvContext()
        {
            lock (Mutex)
            {
                return gvContext();
            }
        }
        public static int GvFreeContext(IntPtr gvc)
        {
            lock (Mutex)
            {
                return gvFreeContext(gvc);
            }
        }
        public static string GvcVersion(IntPtr gvc)
        {
            lock (Mutex)
            {
                return gvcVersion(gvc);
            }
        }
        public static string GvcInfo(IntPtr gvc)
        {
            lock (Mutex)
            {
                return gvcInfo(gvc);
            }
        }
        public static string GvcBuildDate(IntPtr gvc)
        {
            lock (Mutex)
            {
                return gvcBuildDate(gvc);
            }
        }
        public static int GvLayout(IntPtr gvc, IntPtr graph, string engine)
        {
            lock (Mutex)
            {
                return gvLayout(gvc, graph, engine);
            }
        }
        public static int GvFreeLayout(IntPtr gvc, IntPtr graph)
        {
            lock (Mutex)
            {
                return gvFreeLayout(gvc, graph);
            }
        }
        public static IntPtr GvRender(IntPtr gvc, IntPtr graph, string format, IntPtr @out)
        {
            lock (Mutex)
            {
                return gvRender(gvc, graph, format, @out);
            }
        }
        public static IntPtr GvRenderFilename(IntPtr gvc, IntPtr graph, string format, string filename)
        {
            lock (Mutex)
            {
                return gvRenderFilename(gvc, graph, format, filename);
            }
        }
        public static IntPtr Agnode(IntPtr graph, string name, int create)
        {
            lock (Mutex)
            {
                return agnode(graph, name, create);
            }
        }
        public static int Agdegree(IntPtr graph, IntPtr node, int inset, int outset)
        {
            lock (Mutex)
            {
                return agdegree(graph, node, inset, outset);
            }
        }
        public static IntPtr Agfstout(IntPtr graph, IntPtr node)
        {
            lock (Mutex)
            {
                return agfstout(graph, node);
            }
        }
        public static IntPtr Agnxtout(IntPtr graph, IntPtr edge)
        {
            lock (Mutex)
            {
                return agnxtout(graph, edge);
            }
        }
        public static IntPtr Agfstin(IntPtr graph, IntPtr node)
        {
            lock (Mutex)
            {
                return agfstin(graph, node);
            }
        }
        public static IntPtr Agnxtin(IntPtr graph, IntPtr edge)
        {
            lock (Mutex)
            {
                return agnxtin(graph, edge);
            }
        }
        public static IntPtr Agfstedge(IntPtr graph, IntPtr node)
        {
            lock (Mutex)
            {
                return agfstedge(graph, node);
            }
        }
        public static IntPtr Agnxtedge(IntPtr graph, IntPtr edge, IntPtr node)
        {
            lock (Mutex)
            {
                return agnxtedge(graph, edge, node);
            }
        }
        public static void Agattr(IntPtr graph, int type, string name, string deflt)
        {
            lock (Mutex)
            {
                agattr(graph, type, name, deflt);
            }
        }
        public static void Agset(IntPtr obj, string name, string value)
        {
            lock (Mutex)
            {
                agset(obj, name, value);
            }
        }
        public static void Agsafeset(IntPtr obj, string name, string val, string deflt)
        {
            lock (Mutex)
            {
                agsafeset(obj, name, val, deflt);
            }
        }
        public static IntPtr Agroot(IntPtr obj)
        {
            lock (Mutex)
            {
                return agroot(obj);
            }
        }
        public static IntPtr Agnxtattr(IntPtr obj, int kind, IntPtr attribute)
        {
            lock (Mutex)
            {
                return agnxtattr(obj, kind, attribute);
            }
        }
        public static int Agcopyattr(IntPtr from, IntPtr to)
        {
            lock (Mutex)
            {
                return agcopyattr(from, to);
            }
        }
        public static bool Ageqedge(IntPtr edge1, IntPtr edge2)
        {
            lock (Mutex)
            {
                return ageqedge(edge1, edge2);
            }
        }
        public static IntPtr Agtail(IntPtr node)
        {
            lock (Mutex)
            {
                return agtail(node);
            }
        }
        public static IntPtr Aghead(IntPtr node)
        {
            lock (Mutex)
            {
                return aghead(node);
            }
        }
        public static IntPtr Agedge(IntPtr graph, IntPtr tail, IntPtr head, string name, int create)
        {
            lock (Mutex)
            {
                return agedge(graph, tail, head, name, create);
            }
        }
        public static IntPtr Agmkin(IntPtr edge)
        {
            lock (Mutex)
            {
                return agmkin(edge);
            }
        }
        public static IntPtr Agmkout(IntPtr edge)
        {
            lock (Mutex)
            {
                return agmkout(edge);
            }
        }
        public static IntPtr Agparent(IntPtr obj)
        {
            lock (Mutex)
            {
                return agparent(obj);
            }
        }
        public static int Agclose(IntPtr graph)
        {
            lock (Mutex)
            {
                return agclose(graph);
            }
        }
        public static int Agdelete(IntPtr graph, IntPtr item)
        {
            lock (Mutex)
            {
                return agdelete(graph, item);
            }
        }
        public static IntPtr Agfstnode(IntPtr graph)
        {
            lock (Mutex)
            {
                return agfstnode(graph);
            }
        }
        public static IntPtr Agnxtnode(IntPtr graph, IntPtr node)
        {
            lock (Mutex)
            {
                return agnxtnode(graph, node);
            }
        }
        public static int Agcontains(IntPtr graph, IntPtr obj)
        {
            lock (Mutex)
            {
                return agcontains(graph, obj);
            }
        }
        public static IntPtr Agsubg(IntPtr graph, string name, int create)
        {
            lock (Mutex)
            {
                return agsubg(graph, name, create);
            }
        }
        public static IntPtr Agfstsubg(IntPtr graph)
        {
            lock (Mutex)
            {
                return agfstsubg(graph);
            }
        }
        public static IntPtr Agnxtsubg(IntPtr graph)
        {
            lock (Mutex)
            {
                return agnxtsubg(graph);
            }
        }
        public static int Agisstrict(IntPtr ptr)
        {
            lock (Mutex)
            {
                return agisstrict(ptr);
            }
        }
        public static int Agisdirected(IntPtr ptr)
        {
            lock (Mutex)
            {
                return agisdirected(ptr);
            }
        }
        public static int Agisundirected(IntPtr ptr)
        {
            lock (Mutex)
            {
                return agisundirected(ptr);
            }
        }
        public static IntPtr Agsubedge(IntPtr graph, IntPtr edge, int create)
        {
            lock (Mutex)
            {
                return agsubedge(graph, edge, create);
            }
        }
        public static IntPtr Agsubnode(IntPtr graph, IntPtr node, int create)
        {
            lock (Mutex)
            {
                return agsubnode(graph, node, create);
            }
        }
        public static void Imdebug(IntPtr graph)
        {
            lock (Mutex)
            {
                imdebug(graph);
            }
        }
        public static IntPtr EdgeLabel(IntPtr node)
        {
            lock (Mutex)
            {
                return edge_label(node);
            }
        }
        public static void Imagwrite(IntPtr graph, string filename)
        {
            lock (Mutex)
            {
                imagwrite(graph, filename);
            }
        }
        public static string Imagmemwrite(IntPtr graph)
        {
            lock (Mutex)
            {
                return imagmemwrite(graph);
            }
        }
        public static IntPtr GraphLabel(IntPtr node)
        {
            lock (Mutex)
            {
                return graph_label(node);
            }
        }
        public static string Imagget(IntPtr obj, string name)
        {
            lock (Mutex)
            {
                return imagget(obj, name);
            }
        }
        public static string Imagnameof(IntPtr obj)
        {
            lock (Mutex)
            {
                return imagnameof(obj);
            }
        }
        public static void CloneAttributeDeclarations(IntPtr graphfrom, IntPtr graphto)
        {
            lock (Mutex)
            {
                clone_attribute_declarations(graphfrom, graphto);
            }
        }
        public static string ImsymKey(IntPtr sym)
        {
            lock (Mutex)
            {
                return imsym_key(sym);
            }
        }
        public static double LabelX(IntPtr label)
        {
            lock (Mutex)
            {
                return label_x(label);
            }
        }
        public static double LabelY(IntPtr label)
        {
            lock (Mutex)
            {
                return label_y(label);
            }
        }
        public static double LabelWidth(IntPtr label)
        {
            lock (Mutex)
            {
                return label_width(label);
            }
        }
        public static double LabelHeight(IntPtr label)
        {
            lock (Mutex)
            {
                return label_height(label);
            }
        }
        public static string LabelText(IntPtr label)
        {
            lock (Mutex)
            {
                return label_text(label);
            }
        }
        public static double LabelFontsize(IntPtr label)
        {
            lock (Mutex)
            {
                return label_fontsize(label);
            }
        }
        public static string LabelFontname(IntPtr label)
        {
            lock (Mutex)
            {
                return label_fontname(label);
            }
        }
        public static double NodeX(IntPtr node)
        {
            lock (Mutex)
            {
                return node_x(node);
            }
        }
        public static double NodeY(IntPtr node)
        {
            lock (Mutex)
            {
                return node_y(node);
            }
        }
        public static double NodeWidth(IntPtr node)
        {
            lock (Mutex)
            {
                return node_width(node);
            }
        }
        public static double NodeHeight(IntPtr node)
        {
            lock (Mutex)
            {
                return node_height(node);
            }
        }
        public static IntPtr NodeLabel(IntPtr node)
        {
            lock (Mutex)
            {
                return node_label(node);
            }
        }
        public static void ConvertToUndirected(IntPtr graph)
        {
            lock (Mutex)
            {
                convert_to_undirected(graph);
            }
        }
        public static IntPtr Imagmemread(string input)
        {
            lock (Mutex)
            {
                return imagmemread(input);
            }
        }
        public static IntPtr Imagopen(string name, int graphtype)
        {
            lock (Mutex)
            {
                return imagopen(name, graphtype);
            }
        }

        [DllImport("gvc.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr gvContext();
        [DllImport("gvc.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern int gvFreeContext(IntPtr gvc);
        [DllImport("gvc.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private static extern string gvcVersion(IntPtr gvc);
        [DllImport("gvc.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private static extern string gvcInfo(IntPtr gvc);
        [DllImport("gvc.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private static extern string gvcBuildDate(IntPtr gvc);
        [DllImport("gvc.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern int gvLayout(IntPtr gvc, IntPtr graph, [MarshalAs(UnmanagedType.LPStr)] string engine);
        [DllImport("gvc.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern int gvFreeLayout(IntPtr gvc, IntPtr graph);
        [DllImport("gvc.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr gvRender(IntPtr gvc, IntPtr graph, [MarshalAs(UnmanagedType.LPStr)] string format, IntPtr @out);
        [DllImport("gvc.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr gvRenderFilename(IntPtr gvc, IntPtr graph, [MarshalAs(UnmanagedType.LPStr)] string format, [MarshalAs(UnmanagedType.LPStr)] string filename);

        [DllImport("cgraph.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr agnode(IntPtr graph, [MarshalAs(UnmanagedType.LPStr)] string name, int create);
        [DllImport("cgraph.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern int agdegree(IntPtr graph, IntPtr node, int inset, int outset);
        [DllImport("cgraph.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr agfstout(IntPtr graph, IntPtr node);
        [DllImport("cgraph.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr agnxtout(IntPtr graph, IntPtr edge);
        [DllImport("cgraph.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr agfstin(IntPtr graph, IntPtr node);
        [DllImport("cgraph.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr agnxtin(IntPtr graph, IntPtr edge);
        [DllImport("cgraph.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr agfstedge(IntPtr graph, IntPtr node);
        [DllImport("cgraph.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr agnxtedge(IntPtr graph, IntPtr edge, IntPtr node);

        [DllImport("cgraph.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern void agattr(IntPtr graph, int type, [MarshalAs(UnmanagedType.LPStr)] string name, [MarshalAs(UnmanagedType.LPStr)] string deflt);
        [DllImport("cgraph.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern void agset(IntPtr obj, [MarshalAs(UnmanagedType.LPStr)] string name, [MarshalAs(UnmanagedType.LPStr)] string value);
        [DllImport("cgraph.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern void agsafeset(IntPtr obj, [MarshalAs(UnmanagedType.LPStr)] string name, [MarshalAs(UnmanagedType.LPStr)] string val, [MarshalAs(UnmanagedType.LPStr)] string deflt);
        [DllImport("cgraph.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr agroot(IntPtr obj);
        [DllImport("cgraph.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr agnxtattr(IntPtr obj, int kind, IntPtr attribute);
        [DllImport("cgraph.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern int agcopyattr(IntPtr from, IntPtr to);

        [DllImport("cgraph.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool ageqedge(IntPtr edge1, IntPtr edge2);
        [DllImport("cgraph.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr agtail(IntPtr node);
        [DllImport("cgraph.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr aghead(IntPtr node);
        [DllImport("cgraph.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr agedge(IntPtr graph, IntPtr tail, IntPtr head, [MarshalAs(UnmanagedType.LPStr)] string name, int create);
        [DllImport("cgraph.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr agmkin(IntPtr edge);
        [DllImport("cgraph.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr agmkout(IntPtr edge);

        [DllImport("cgraph.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr agparent(IntPtr obj);

        [DllImport("cgraph.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern int agclose(IntPtr graph);
        [DllImport("cgraph.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern int agdelete(IntPtr graph, IntPtr item);
        [DllImport("cgraph.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr agfstnode(IntPtr graph);
        [DllImport("cgraph.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr agnxtnode(IntPtr graph, IntPtr node);
        [DllImport("cgraph.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern int agcontains(IntPtr graph, IntPtr obj);

        [DllImport("cgraph.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr agsubg(IntPtr graph, [MarshalAs(UnmanagedType.LPStr)] string name, int create);
        [DllImport("cgraph.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr agfstsubg(IntPtr graph);
        [DllImport("cgraph.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr agnxtsubg(IntPtr graph);
        [DllImport("cgraph.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern int agisstrict(IntPtr ptr);
        [DllImport("cgraph.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern int agisdirected(IntPtr ptr);
        [DllImport("cgraph.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern int agisundirected(IntPtr ptr);

        [DllImport("cgraph.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr agsubedge(IntPtr graph, IntPtr edge, int create);
        [DllImport("cgraph.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr agsubnode(IntPtr graph, IntPtr node, int create);

        [DllImport("GraphvizWrapper.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern void imdebug(IntPtr graph);
        [DllImport("GraphvizWrapper.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr edge_label(IntPtr node);
        [DllImport("GraphvizWrapper.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern void imagwrite(IntPtr graph, [MarshalAs(UnmanagedType.LPStr)] string filename);
        [DllImport("GraphvizWrapper.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private static extern string imagmemwrite(IntPtr graph);
        [DllImport("GraphvizWrapper.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr graph_label(IntPtr node);

        [DllImport("GraphvizWrapper.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private static extern string imagget(IntPtr obj, [MarshalAs(UnmanagedType.LPStr)] string name);
        [DllImport("GraphvizWrapper.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private static extern string imagnameof(IntPtr obj);
        [DllImport("GraphvizWrapper.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern void clone_attribute_declarations(IntPtr graphfrom, IntPtr graphto);
        [DllImport("GraphvizWrapper.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private static extern string imsym_key(IntPtr sym);

        [DllImport("GraphvizWrapper.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern double label_x(IntPtr label);
        [DllImport("GraphvizWrapper.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern double label_y(IntPtr label);
        [DllImport("GraphvizWrapper.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern double label_width(IntPtr label);
        [DllImport("GraphvizWrapper.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern double label_height(IntPtr label);
        [DllImport("GraphvizWrapper.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private static extern string label_text(IntPtr label);
        [DllImport("GraphvizWrapper.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern double label_fontsize(IntPtr label);
        [DllImport("GraphvizWrapper.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        private static extern string label_fontname(IntPtr label);

        [DllImport("GraphvizWrapper.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern double node_x(IntPtr node);
        [DllImport("GraphvizWrapper.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern double node_y(IntPtr node);
        [DllImport("GraphvizWrapper.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern double node_width(IntPtr node);
        [DllImport("GraphvizWrapper.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern double node_height(IntPtr node);
        [DllImport("GraphvizWrapper.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr node_label(IntPtr node);

        [DllImport("GraphvizWrapper.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern void convert_to_undirected(IntPtr graph);
        [DllImport("GraphvizWrapper.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr imagmemread([MarshalAs(UnmanagedType.LPStr)] string input);
        [DllImport("GraphvizWrapper.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr imagopen([MarshalAs(UnmanagedType.LPStr)] string name, int graphtype);
    }
}
