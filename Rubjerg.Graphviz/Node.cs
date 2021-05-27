using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using static Rubjerg.Graphviz.ForeignFunctionInterface;

namespace Rubjerg.Graphviz
{
    public class Node : CGraphThing
    {
        /// <summary>
        /// rootgraph must not be null
        /// </summary>
        internal Node(IntPtr ptr, RootGraph rootgraph) : base(ptr, rootgraph) { }

        internal static Node Get(Graph graph, string name)
        {
            IntPtr ptr = Agnode(graph._ptr, name, 0);
            if (ptr != IntPtr.Zero)
                return new Node(ptr, graph.MyRootGraph);
            return null;
        }

        internal static Node GetOrCreate(Graph graph, string name)
        {
            IntPtr ptr = Agnode(graph._ptr, name, 1);
            return new Node(ptr, graph.MyRootGraph);
        }

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

        public IEnumerable<Edge> EdgesOut(Graph graph = null)
        {
            IntPtr graph_ptr = graph?._ptr ?? MyRootGraph._ptr;
            var current = Agfstout(graph_ptr, _ptr);
            while (current != IntPtr.Zero)
            {
                yield return new Edge(current, MyRootGraph);
                current = Agnxtout(graph_ptr, current);
            }
        }

        public IEnumerable<Edge> EdgesIn(Graph graph = null)
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
        public IEnumerable<Edge> Edges(Graph graph = null)
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
        public IEnumerable<Node> NeighborsOut(Graph graph = null)
        {
            return EdgesOut(graph).Select(e => e.OppositeEndpoint(this));
        }

        /// <summary>
        /// Get all neighbors connected via an in edge.
        /// </summary>
        public IEnumerable<Node> NeighborsIn(Graph graph = null)
        {
            return EdgesIn(graph).Select(e => e.OppositeEndpoint(this));
        }

        /// <summary>
        /// Get all neighbors.
        /// </summary>
        public IEnumerable<Node> Neighbors(Graph graph = null)
        {
            return Edges(graph).Select(e => e.OppositeEndpoint(this));
        }

        /// <summary>
        /// Get all neighbors fullfilling a given attribute constraint.
        /// </summary>
        public IEnumerable<Node> NeighborsByAttribute(string attr_name, string attr_value, Graph graph = null)
        {
            return Neighbors(graph).Where(n => n.GetAttribute(attr_name) == attr_value);
        }

        /// <summary>
        /// Get all neighbors connected by an edge with given name.
        /// </summary>
        public IEnumerable<Node> NeighborsByEdgeName(string edgename, Graph graph = null)
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
            Node result = destination.GetOrAddNode(GetName());
            _ = CopyAttributesTo(result);
            return result;
        }

        public int OutDegree(Graph graph = null)
        {
            IntPtr graph_ptr = graph?._ptr ?? MyRootGraph._ptr;
            return Agdegree(graph_ptr, _ptr, 0, 1);
        }

        public int InDegree(Graph graph = null)
        {
            IntPtr graph_ptr = graph?._ptr ?? MyRootGraph._ptr;
            return Agdegree(graph_ptr, _ptr, 1, 0);
        }

        public int TotalDegree(Graph graph = null)
        {
            IntPtr graph_ptr = graph?._ptr ?? MyRootGraph._ptr;
            return Agdegree(graph_ptr, _ptr, 1, 1);
        }

        public bool IsAdjacentTo(Node node)
        {
            return EdgesOut().Any(e => e.Head().Equals(node)) || EdgesIn().Any(e => e.Tail().Equals(node));
        }

        public RectangleF BoundingBox()
        {
            // x and y are the center of the node
            // Coords are in points, sizes in inches. 72 points = 1 inch
            float x = Convert.ToSingle(NodeX(_ptr));
            float y = Convert.ToSingle(NodeY(_ptr));
            float w = Convert.ToSingle(NodeWidth(_ptr) * 72);
            float h = Convert.ToSingle(NodeHeight(_ptr) * 72);
            return new RectangleF(x - w / 2, y - h / 2, w, h);
        }

        /// <summary>
        /// Return null if label not set.
        /// </summary>
        /// <returns></returns>
        public GraphvizLabel GetLabel()
        {
            IntPtr labelptr = NodeLabel(_ptr);
            if (labelptr == IntPtr.Zero)
                return null;
            return new GraphvizLabel(labelptr, BoundingBoxCoords.Centered, new PointF(0, 0));
        }

        public PointF Position()
        {
            return new PointF(Convert.ToSingle(NodeX(_ptr)), Convert.ToSingle(NodeY(_ptr)));
        }

        public void MakeInvisibleAndSmall()
        {
            SafeSetAttribute("style", "invis", "");
            SafeSetAttribute("margin", "0", "");
            SafeSetAttribute("width", "0", "");
            SafeSetAttribute("height", "0", "");
            SafeSetAttribute("shape", "point", "");
        }

        /// <summary>
        /// If the shape of this node was set to 'record', this method allows you to retrieve the
        /// resulting rectangles.
        /// </summary>
        public IEnumerable<RectangleF> GetRecordRectangles()
        {
            if (!HasAttribute("rects"))
                yield break;

            foreach (string rect in GetAttribute("rects").Split(' '))
                yield return ParseRect(rect);
        }

        private RectangleF ParseRect(string rect)
        {
            string[] points = rect.Split(',');
            float leftX = float.Parse(points[0], NumberStyles.Any, CultureInfo.InvariantCulture);
            float upperY = float.Parse(points[1], NumberStyles.Any, CultureInfo.InvariantCulture);
            float rightX = float.Parse(points[2], NumberStyles.Any, CultureInfo.InvariantCulture);
            float lowerY = float.Parse(points[3], NumberStyles.Any, CultureInfo.InvariantCulture);
            return new RectangleF(leftX, upperY, rightX - leftX, lowerY - upperY);
        }
    }
}
