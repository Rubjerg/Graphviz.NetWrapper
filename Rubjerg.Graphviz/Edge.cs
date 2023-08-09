using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using static Rubjerg.Graphviz.ForeignFunctionInterface;

namespace Rubjerg.Graphviz;

public class Edge : CGraphThing
{
    /// <summary>
    /// rootgraph must not be null
    /// </summary>
    internal Edge(IntPtr ptr, RootGraph rootgraph) : base(ptr, rootgraph) { }

    internal static Edge Get(Graph graph, Node tail, Node head, string name)
    {
        name = NameString(name);
        IntPtr ptr = Agedge(graph._ptr, tail._ptr, head._ptr, name, 0);
        if (ptr == IntPtr.Zero)
            return null;
        return new Edge(ptr, graph.MyRootGraph);
    }

    internal static Edge GetOrCreate(Graph graph, Node tail, Node head, string name)
    {
        name = NameString(name);
        IntPtr ptr = Agedge(graph._ptr, tail._ptr, head._ptr, name, 1);
        return new Edge(ptr, graph.MyRootGraph);
    }

    /// <summary>
    /// Introduces an attribute for edges in the given graph by given a default.
    /// A given default can be overwritten by calling this method again.
    /// </summary>
    public static void IntroduceAttribute(RootGraph root, string name, string deflt)
    {
        _ = deflt ?? throw new ArgumentNullException(nameof(deflt));
        Agattr(root._ptr, 2, name, deflt);
    }

    public static void IntroduceAttributeHtml(RootGraph root, string name, string deflt)
    {
        _ = deflt ?? throw new ArgumentNullException(nameof(deflt));
        AgattrHtml(root._ptr, 2, name, deflt);
    }

    protected internal IntPtr HeadPtr()
    {
        return Aghead(_ptr);
    }

    protected internal IntPtr TailPtr()
    {
        return Agtail(_ptr);
    }

    public Node Head()
    {
        return new Node(HeadPtr(), MyRootGraph);
    }

    public Node Tail()
    {
        return new Node(TailPtr(), MyRootGraph);
    }

    public Node OppositeEndpoint(Node node)
    {
        var tail = Tail();
        var head = Head();
        Debug.Assert(node == tail || node == head);
        return node == tail ? head : tail;
    }

    public bool IsAdjacentTo(Node node)
    {
        return node.Equals(Head()) || node.Equals(Tail());
    }

    public bool IsBetween(Node node1, Node node2)
    {
        return IsAdjacentTo(node1) && IsAdjacentTo(node2);
    }

    /// <summary>
    /// An edge can define a cluster as logical tail.
    /// This is used to fake edges to and from clusters by clipping the edge on the borders of the logical tail.
    /// </summary>
    /// <returns></returns>
    public void SetLogicalTail(SubGraph ltail)
    {
        if (!ltail.IsCluster())
            throw new InvalidOperationException("ltail must be a cluster");
        if (!MyRootGraph.IsCompound())
            throw new InvalidOperationException("rootgraph must be compound for lheads/ltails to be used");
        string ltailname = ltail.GetName();
        SetAttribute("ltail", ltailname);
    }

    /// <summary>
    /// An edge can define a cluster as logical head.
    /// This is used to fake edges to and from clusters by clipping the edge on the borders of the logical head.
    /// </summary>
    public void SetLogicalHead(SubGraph lhead)
    {
        if (!lhead.IsCluster())
            throw new InvalidOperationException("ltail must be a cluster");
        if (!MyRootGraph.IsCompound())
            throw new InvalidOperationException("rootgraph must be compound for lheads/ltails to be used");
        string lheadname = lhead.GetName();
        SetAttribute("lhead", lheadname);
    }

    /// <summary>
    /// Port names cannot contain certain characters, and other characters must be escaped.
    /// This function converts a string to an ID that is valid as a port name.
    /// It makes sure there are no collisions.
    /// </summary>
    public static string ConvertUidToPortName(string id)
    {
        string result = id;
        foreach (char c in new[] { '<', '>', '{', '}', '|', ':' })
        {
            result = result.Replace("+", "[+]");
            result = result.Replace(c, '+');
        }
        return result;
    }

    // Because there are two valid pointers to each edge, we have to override the default equals behaviour
    // which simply compares the wrapped pointers.
    public override bool Equals(GraphvizThing obj)
    {
        if (obj is Edge)
            return Ageqedge(_ptr, obj._ptr);
        return false;
    }

    public override int GetHashCode()
    {
        // Return the ptr to the in-edge, which is unique and consistent for each edge.
        // The following line can result in an OverflowException:
        //return (int) agmkin(ptr);
        return (int)(long)Agmkin(_ptr);
    }

    #region layout attributes

    /// <summary>
    /// This method only returns the first spline that is defined.
    /// Returns null if no splines exist.
    /// </summary>
    public PointF[] GetFirstSpline()
    {
        return GetSplines().FirstOrDefault();
    }

    /// <summary>
    /// The splines contain 3n+1 points, just like expected by .net drawing methods.
    /// Sometimes there are multiple splines per edge. However, this is not always correct:
    /// https://github.com/ellson/graphviz/issues/1277
    /// Edge arrows are ignored.
    /// </summary>
    public IEnumerable<PointF[]> GetSplines()
    {
        // FIXNOW
        return GetDrawing().OfType<XDotOp.UnfilledBezier>()
            .Select(x => x.Value.Points.Select(p => new PointF((float)p.X, (float)p.Y)).ToArray());
    }

    public IReadOnlyList<XDotOp> GetDrawing() => GetXDotValue(this, "_draw_");
    public IReadOnlyList<XDotOp> GetLabelDrawing() => GetXDotValue(this, "_ldraw_");
    public IReadOnlyList<XDotOp> GetHeadArrowDrawing() => GetXDotValue(this, "_hdraw_");
    public IReadOnlyList<XDotOp> GetTailArrowDrawing() => GetXDotValue(this, "_tdraw_");
    public IReadOnlyList<XDotOp> GetHeadLabelDrawing() => GetXDotValue(this, "_hldraw_");
    public IReadOnlyList<XDotOp> GetTailLabelDrawing() => GetXDotValue(this, "_tldraw_");

    #endregion
}
