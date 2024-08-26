using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using static Rubjerg.Graphviz.ForeignFunctionInterface;

namespace Rubjerg.Graphviz;

public class Node : CGraphThing
{
    /// <summary>
    /// rootgraph must not be null
    /// </summary>
    internal Node(IntPtr ptr, RootGraph rootgraph) : base(ptr, rootgraph) { }

    internal static Node? Get(Graph graph, string? name)
    {
        name = NameString(name);
        IntPtr ptr = Agnode(graph._ptr, name, 0);
        if (ptr != IntPtr.Zero)
            return new Node(ptr, graph.MyRootGraph);
        return null;
    }

    internal static Node GetOrCreate(Graph graph, string name)
    {
        name = NameString(name)!;
        IntPtr ptr = Agnode(graph._ptr, name, 1);
        return new Node(ptr, graph.MyRootGraph);
    }

    /// <summary>
    /// Identifier for this node.
    /// </summary>
    public new string GetName() => base.GetName()!;

    /// <summary>
    /// Introduces an attribute for nodes in the given graph by giving a default value.
    /// A given default can be overwritten by calling this method again.
    /// </summary>
    public static void IntroduceAttribute(RootGraph root, string name, string deflt)
    {
        _ = deflt ?? throw new ArgumentNullException(nameof(deflt));
        Agattr(root._ptr, 1, name, deflt);
    }

    public static void IntroduceAttributeHtml(RootGraph root, string name, string deflt)
    {
        _ = deflt ?? throw new ArgumentNullException(nameof(deflt));
        AgattrHtml(root._ptr, 1, name, deflt);
    }

    public IEnumerable<Edge> EdgesOut(Graph? graph = null)
    {
        IntPtr graph_ptr = graph?._ptr ?? MyRootGraph._ptr;
        var current = Agfstout(graph_ptr, _ptr);
        while (current != IntPtr.Zero)
        {
            yield return new Edge(current, MyRootGraph);
            current = Agnxtout(graph_ptr, current);
        }
    }

    public IEnumerable<Edge> EdgesIn(Graph? graph = null)
    {
        IntPtr graph_ptr = graph?._ptr ?? MyRootGraph._ptr;
        var current = Agfstin(graph_ptr, _ptr);
        while (current != IntPtr.Zero)
        {
            yield return new Edge(current, MyRootGraph);
            current = Agnxtin(graph_ptr, current);
        }
    }

    /// <summary>
    /// Iterate over both in and out edges. This will not yield self loops twice.
    /// </summary>
    public IEnumerable<Edge> Edges(Graph? graph = null)
    {
        IntPtr graph_ptr = graph?._ptr ?? MyRootGraph._ptr;
        var current = Agfstedge(graph_ptr, _ptr);
        while (current != IntPtr.Zero)
        {
            yield return new Edge(current, MyRootGraph);
            current = Agnxtedge(graph_ptr, current, _ptr); // This line crashes at some point
        }
    }

    /// <summary>
    /// Get all neighbors connected via an out edge.
    /// </summary>
    public IEnumerable<Node> NeighborsOut(Graph? graph = null)
    {
        return EdgesOut(graph).Select(e => e.OppositeEndpoint(this));
    }

    /// <summary>
    /// Get all neighbors connected via an in edge.
    /// </summary>
    public IEnumerable<Node> NeighborsIn(Graph? graph = null)
    {
        return EdgesIn(graph).Select(e => e.OppositeEndpoint(this));
    }

    /// <summary>
    /// Get all neighbors.
    /// </summary>
    public IEnumerable<Node> Neighbors(Graph? graph = null)
    {
        return Edges(graph).Select(e => e.OppositeEndpoint(this));
    }

    /// <summary>
    /// Get all neighbors fullfilling a given attribute constraint.
    /// </summary>
    public IEnumerable<Node> NeighborsByAttribute(string attr_name, string attr_value, Graph? graph = null)
    {
        return Neighbors(graph).Where(n => n.GetAttribute(attr_name) == attr_value);
    }

    /// <summary>
    /// Get all neighbors connected by an edge with given name.
    /// </summary>
    public IEnumerable<Node> NeighborsByEdgeName(string edgename, Graph? graph = null)
    {
        return Edges(graph).Where(e => e.GetName() == edgename).Select(e => e.OppositeEndpoint(this));
    }


    /// <summary>
    /// Copy the node to another root graph.
    /// Copies the attributes as well, as far as the attributes have been
    /// introduced in the destination graph.
    /// </summary>
    public Node CopyToOtherRoot(RootGraph destination)
    {
        Node result = destination.GetOrAddNode(GetName()!);
        _ = CopyAttributesTo(result);
        return result;
    }

    public int OutDegree(Graph? graph = null)
    {
        IntPtr graph_ptr = graph?._ptr ?? MyRootGraph._ptr;
        return Agdegree(graph_ptr, _ptr, 0, 1);
    }

    public int InDegree(Graph? graph = null)
    {
        IntPtr graph_ptr = graph?._ptr ?? MyRootGraph._ptr;
        return Agdegree(graph_ptr, _ptr, 1, 0);
    }

    public int TotalDegree(Graph? graph = null)
    {
        IntPtr graph_ptr = graph?._ptr ?? MyRootGraph._ptr;
        return Agdegree(graph_ptr, _ptr, 1, 1);
    }

    public bool IsAdjacentTo(Node node)
    {
        return EdgesOut().Any(e => e.Head().Equals(node)) || EdgesIn().Any(e => e.Tail().Equals(node));
    }

    public void MakeInvisibleAndSmall()
    {
        SetAttribute("style", "invis");
        SetAttribute("margin", "0");
        SetAttribute("width", "0");
        SetAttribute("height", "0");
        SetAttribute("shape", "point");
    }

    #region layout attributes

    /// <summary>
    /// The position of the center of the node.
    /// </summary>
    public PointD GetPosition()
    {
        // The "pos" attribute is available as part of xdot output
        PointD result;
        if (HasAttribute("pos") && GetAttribute("pos") is string posString)
        {
            var coords = posString.Split(',');
            double x = double.Parse(coords[0], NumberStyles.Any, CultureInfo.InvariantCulture);
            double y = double.Parse(coords[1], NumberStyles.Any, CultureInfo.InvariantCulture);
            result = new PointD(x, y);
        }
        else
        {
            // If the "pos" attribute is not available, try the following FFI functions,
            // which are available after a ComputeLayout
            result = new PointD(Convert.ToSingle(NodeX(_ptr)), Convert.ToSingle(NodeY(_ptr)));
        }
        return result.ForCoordSystem(MyRootGraph.CoordinateSystem, MyRootGraph.RawMaxY());
    }

    /// <summary>
    /// The size of bounding box of the node.
    /// </summary>
    public SizeD GetSize()
    {
        // The "width" and "height" attributes are available as part of xdot output
        double w, h;
        if (HasAttribute("width") && HasAttribute("height"))
        {
            w = double.Parse(GetAttribute("width"), NumberStyles.Any, CultureInfo.InvariantCulture);
            h = double.Parse(GetAttribute("height"), NumberStyles.Any, CultureInfo.InvariantCulture);
        }
        else
        {
            // If they are not available, try the following FFI functions,
            // which are available after a ComputeLayout
            w = Convert.ToSingle(NodeWidth(_ptr));
            h = Convert.ToSingle(NodeHeight(_ptr));
        }
        // Coords are in points, sizes in inches. 72 points = 1 inch
        // We return everything in terms of points.
        return new SizeD(w * 72, h * 72);
    }

    public RectangleD GetBoundingBox()
    {
        var size = GetSize();
        var center = GetPosition();
        var rectangleOrigin = new PointD(center.X - size.Width / 2, center.Y - size.Height / 2);
        return new RectangleD(rectangleOrigin, size);
    }

    /// <summary>
    /// If the shape of this node was set to 'record', this method allows you to retrieve the
    /// resulting rectangles.
    /// The order of the list matches the order in which the labels occur in the label string.
    /// </summary>
    /// <param name="snapOntoDrawingCoordinates">
    /// There is a lingering issue in Graphviz where the coordinates of the record rectangles may be off.
    /// As a workaround we snap onto the coordinates from the drawing info, which seem to be more reliable.
    /// https://github.com/Rubjerg/Graphviz.NetWrapper/issues/30
    /// </param>
    public IEnumerable<RectangleD> GetRecordRectangles(bool snapOntoDrawingCoordinates = false)
    {
        if (HasAttribute("rects") && GetAttribute("rects") is string rects)
        {
            var polylinePoints = GetDrawing().OfType<IHasPoints>().SelectMany(p => p.Points).ToList();
            var validXCoords = polylinePoints.Select(p => p.X).OrderBy(x => x).Distinct().ToList();
            var validYCoords = polylinePoints.Select(p => p.Y).OrderBy(x => x).Distinct().ToList();

            var maxY = MyRootGraph.RawMaxY();
            foreach (var rectStr in rects.Split(' '))
            {
                var rect = ParseRect(rectStr).ForCoordSystem(MyRootGraph.CoordinateSystem, maxY);
                if (!snapOntoDrawingCoordinates)
                {
                    yield return rect;
                }
                else
                {
                    var x1 = rect.X;
                    var x2 = rect.X + rect.Width;
                    var y1 = rect.Y;
                    var y2 = rect.Y + rect.Height;
                    var snappedX1 = FindClosest(validXCoords, x1);
                    var snappedX2 = FindClosest(validXCoords, x2);
                    var snappedY1 = FindClosest(validYCoords, y1);
                    var snappedY2 = FindClosest(validYCoords, y2);
                    var snappedRect = new RectangleD(
                        new PointD(snappedX1, snappedY1),
                        new SizeD(snappedX2 - snappedX1, snappedY2 - snappedY1));
                    yield return snappedRect;
                }
            }
        }
    }

    /// <summary>
    /// If the shape of this node was set to 'record', this method allows you to retrieve the
    /// text objects of the resulting rectangles.
    /// The order of the list matches the order in which the labels occur in the label string.
    /// </summary>
    public IEnumerable<TextInfo> GetRecordRectangleLabels()
    {
        return GetLabelDrawing().OfType<XDotOp.Text>().Select(x => x.Value);
    }

    /// <summary>
    /// Return the value that is closest to the given target value.
    /// Return target if the sequence if empty.
    /// </summary>
    private static double FindClosest(IEnumerable<double> self, double target)
    {
        if (self.Any())
            return self.OrderBy(x => Math.Abs(x - target)).First();
        return target;
    }

    #endregion
}
