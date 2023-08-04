using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using static Rubjerg.Graphviz.ForeignFunctionInterface;

namespace Rubjerg.Graphviz
{
    public class Edge : CGraphThing
    {
        /// <summary>
        /// rootgraph must not be null
        /// </summary>
        internal Edge(IntPtr ptr, RootGraph rootgraph) : base(ptr, rootgraph) { }

        internal static Edge Get(Graph graph, Node tail, Node head, string name)
        {
            IntPtr ptr = Agedge(graph._ptr, tail._ptr, head._ptr, name, 0);
            if (ptr == IntPtr.Zero)
                return null;
            return new Edge(ptr, graph.MyRootGraph);
        }
        internal static Edge GetOrCreate(Graph graph, Node tail, Node head, string name)
        {
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

        /// <summary>
        /// The splines contain 3n+1 points, just like expected by .net drawing methods.
        /// Sometimes there are multiple splines per edge. However, this is not always correct:
        /// https://github.com/ellson/graphviz/issues/1277
        /// This method only returns the first spline that is defined.
        /// Edge arrows are ignored.
        /// </summary>
        public PointF[] FirstSpline()
        {
            if (!HasPosition())
                return null;
            string pos_string = GetAttribute("pos");
            return ParseSpline(pos_string.Split(';').First());
        }

        /// <summary>
        /// The splines contain 3n+1 points, just like expected by .net drawing methods.
        /// Sometimes there are multiple splines per edge. However, this is not always correct:
        /// https://github.com/ellson/graphviz/issues/1277
        /// Edge arrows are ignored.
        /// </summary>
        public IEnumerable<PointF[]> Splines()
        {
            if (!HasPosition())
                yield break;

            foreach (string spline in GetAttribute("pos").Split(';'))
                yield return ParseSpline(spline);
        }

        private PointF[] ParseSpline(string spline)
        {
            string[] points = spline.Split(' ');
            var splinepoints = new PointF[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                if (points[i].StartsWith("e"))
                    continue; // Ignore arrow indicators
                string xstring = points[i].Split(',')[0];
                string ystring = points[i].Split(',')[1];
                // NOTE: if a parse error occurs here, we probably missed a case and e.g. don't deal correctly
                // with edges having arrows
                float x = float.Parse(xstring, NumberStyles.Any, CultureInfo.InvariantCulture);
                float y = float.Parse(ystring, NumberStyles.Any, CultureInfo.InvariantCulture);
                splinepoints[i] = new PointF(x, y);
            }
            return splinepoints;
        }

        public IReadOnlyList<XDotOp> GetDrawing() => GetXDotValue(this, "_draw_");
        public IReadOnlyList<XDotOp> GetLabelDrawing() => GetXDotValue(this, "_ldraw_");
        public IReadOnlyList<XDotOp> GetHeadArrowDrawing() => GetXDotValue(this, "_hdraw_");
        public IReadOnlyList<XDotOp> GetTailArrowDrawing() => GetXDotValue(this, "_tdraw_");
        public IReadOnlyList<XDotOp> GetHeadLabelDrawing() => GetXDotValue(this, "_hldraw_");
        public IReadOnlyList<XDotOp> GetTailLabelDrawing() => GetXDotValue(this, "_tldraw_");

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
            SafeSetAttribute("ltail", ltailname, "");
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
            SafeSetAttribute("lhead", lheadname, "");
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
    }
}
