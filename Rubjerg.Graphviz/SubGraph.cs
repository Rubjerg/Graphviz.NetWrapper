using System;
using System.Collections.Generic;
using static Rubjerg.Graphviz.ForeignFunctionInterface;

namespace Rubjerg.Graphviz;

public class SubGraph : Graph
{
    /// <summary>
    /// rootgraph must not be null
    /// </summary>
    internal SubGraph(IntPtr ptr, RootGraph rootgraph) : base(ptr, rootgraph) { }

    internal static SubGraph Get(Graph parent, string name = null)
    {
        name = NameString(name);
        IntPtr ptr = Agsubg(parent._ptr, name, 0);
        if (ptr == IntPtr.Zero)
            return null;
        return new SubGraph(ptr, parent.MyRootGraph);

    }

    internal static SubGraph GetOrCreate(Graph parent, string name = null)
    {
        name = NameString(name);
        IntPtr ptr = Agsubg(parent._ptr, name, 1);
        return new SubGraph(ptr, parent.MyRootGraph);
    }

    public void AddExisting(Node node)
    {
        _ = Agsubnode(_ptr, node._ptr, 1);
    }

    public void AddExisting(Edge edge)
    {
        _ = Agsubedge(_ptr, edge._ptr, 1);
    }

    /// <summary>
    /// FIXME: use an actual subg equivalent to agsubedge and agsubnode
    /// https://github.com/ellson/graphviz/issues/1206
    /// This might cause a new subgraph creation.
    /// </summary>
    public void AddExisting(SubGraph subgraph)
    {
        _ = Agsubg(_ptr, subgraph.GetName(), 1);
    }

    public void AddExisting(IEnumerable<Node> nodes)
    {
        foreach (var node in nodes)
            AddExisting(node);
    }

    public void AddExisting(IEnumerable<Edge> edges)
    {
        foreach (var edge in edges)
            AddExisting(edge);
    }

    public void Delete()
    {
        _ = Agclose(_ptr);
    }
}
