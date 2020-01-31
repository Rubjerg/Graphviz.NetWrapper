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
            Agattr(root._ptr, 2, name, deflt);
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

        /// <summary>
        /// May return null‚ if no logical endpoint is set.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public SubGraph OppositeLogicalEndpoint(Node node)
        {
            Debug.Assert(node == Tail() || node == Head());
            return node == Tail() ? LogicalHead() : LogicalTail();
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
        public override bool Equals(GraphVizThing obj)
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
        /// Delete self, and add a new edge with the same name, but opposite direction.
        /// Return the new edge wrapper. Also copy attributes.
        /// NB: the older edge wrapper is invalid after calling this function!
        /// NB: doesn't work with strict graphs!
        /// </summary>
        public Edge FlipDirection()
        {
            Node tail = Tail();
            Node head = Head();
            RootGraph root = MyRootGraph;
            Debug.Assert(!root.IsStrict());
            string name = GetName();
            string tmp_guid = Guid.NewGuid().ToString();
            Edge tmp = root.GetOrAddEdge(head, tail, tmp_guid);
            CopyAttributesTo(tmp);
            root.Delete(this);
            Edge result = root.GetOrAddEdge(head, tail, name);
            tmp.CopyAttributesTo(result);
            root.Delete(tmp);
            return result;
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

        /// <summary>
        /// An edge can define a cluster as logical tail.
        /// This is used to fake edges to and from clusters by clipping the edge on the borders of the logical tail.
        /// </summary>
        /// <returns></returns>
        public void SetLogicalTail(SubGraph ltail)
        {
            Debug.Assert(ltail.IsCluster(), "ltail must be a cluster");
            Debug.Assert(MyRootGraph.IsCompound(), "rootgraph must be compound for lheads/ltails to be used");
            string ltailname = ltail.GetName();
            SafeSetAttribute("ltail", ltailname, "");
        }

        /// <summary>
        /// An edge can define a cluster as logical head.
        /// This is used to fake edges to and from clusters by clipping the edge on the borders of the logical head.
        /// </summary>
        public void SetLogicalHead(SubGraph lhead)
        {
            Debug.Assert(lhead.IsCluster(), "lhead must be a cluster");
            Debug.Assert(MyRootGraph.IsCompound(), "rootgraph must be compound for lheads/ltails to be used");
            string lheadname = lhead.GetName();
            SafeSetAttribute("lhead", lheadname, "");
        }

        /// <summary>
        /// An edge can define a cluster as logical tail.
        /// This is used to fake edges to and from clusters by clipping the edge on the borders of the logical tail.
        /// </summary>
        public SubGraph LogicalTail()
        {
            Debug.Assert(MyRootGraph.IsCompound(), "rootgraph must be compound for lheads/ltails to be used");
            string ltailname = GetAttribute("ltail");
            if (ltailname == null)
                return null;
            return MyRootGraph.GetSubgraph(ltailname);
        }

        /// <summary>
        /// An edge can define a cluster as logical head.
        /// This is used to fake edges to and from clusters by clipping the edge on the borders of the logical head.
        /// </summary>
        public SubGraph LogicalHead()
        {
            Debug.Assert(MyRootGraph.IsCompound(), "rootgraph must be compound for lheads/ltails to be used");
            string lheadname = GetAttribute("lhead");
            if (lheadname == null)
                return null;
            return MyRootGraph.GetSubgraph(lheadname);
        }

        public SubGraph OppositeLogicalEndpoint(SubGraph s)
        {
            Debug.Assert(s == LogicalTail() || s == LogicalHead());
            return s == LogicalTail() ? LogicalHead() : LogicalTail();
        }
    }
}
