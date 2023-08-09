using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using static Rubjerg.Graphviz.ForeignFunctionInterface;

namespace Rubjerg.Graphviz;

/// <summary>
/// Wraps a cgraph graph object - either a subgraph or a rootgraph.
/// </summary>
public class Graph : CGraphThing
{
    /// <summary>
    /// rootgraph may be null
    /// </summary>
    protected Graph(IntPtr ptr, RootGraph rootgraph) : base(ptr, rootgraph) { }
    public bool IsStrict() { return Agisstrict(_ptr) != 0; }
    public bool IsDirected() { return Agisdirected(_ptr) != 0; }
    public bool IsUndirected() { return Agisundirected(_ptr) != 0; }
    public GraphType GetGraphType()
    {
        if (IsDirected() && !IsStrict())
            return GraphType.Directed;
        if (IsDirected() && IsStrict())
            return GraphType.StrictDirected;
        if (IsUndirected() && !IsStrict())
            return GraphType.Undirected;
        return GraphType.StrictUndirected;
    }

    /// <summary>
    /// Introduces an attribute for subgraphs of the given graph by given a default.
    /// A given default can be overwritten by calling this method again.
    /// </summary>
    public static void IntroduceAttribute(RootGraph root, string name, string deflt)
    {
        _ = deflt ?? throw new ArgumentNullException(nameof(deflt));
        Agattr(root._ptr, 0, name, deflt);
    }

    public static void IntroduceAttributeHtml(RootGraph root, string name, string deflt)
    {
        _ = deflt ?? throw new ArgumentNullException(nameof(deflt));
        AgattrHtml(root._ptr, 0, name, deflt);
    }

    public bool Contains(CGraphThing thing)
    {
        return Agcontains(_ptr, thing._ptr) != 0;
    }

    /// <summary>
    /// Delete the given thing from the graph.
    /// Is this is a root graph, and thing is a node, its edges are delete too.
    /// </summary>
    public void Delete(CGraphThing thing)
    {
        int return_code = Agdelete(_ptr, thing._ptr);
        Debug.Assert(return_code == 0);
    }

    public IEnumerable<Node> Nodes()
    {
        var current = Agfstnode(_ptr);
        while (current != IntPtr.Zero)
        {
            yield return new Node(current, MyRootGraph);
            current = Agnxtnode(_ptr, current);
        }
    }

    public IEnumerable<Edge> Edges()
    {
        return Nodes().SelectMany(n => n.EdgesOut());
    }

    public IEnumerable<SubGraph> Children()
    {
        var current = Agfstsubg(_ptr);
        while (current != IntPtr.Zero)
        {
            yield return new SubGraph(current, MyRootGraph);
            current = Agnxtsubg(current);
        }
    }

    /// <summary>
    /// Return all subgraphs recursively in a depthfirst order.
    /// Don't delete subgraphs while iterating. Instead, use the method SafeDeleteSubgraphs.
    /// </summary>
    public IEnumerable<SubGraph> Descendants()
    {
        foreach (var child in Children())
        {
            yield return child;
            foreach (SubGraph descendant in child.Descendants())
                yield return descendant;
        }
    }

    public Graph Parent()
    {
        IntPtr p = Agparent(_ptr);
        if (p == IntPtr.Zero)
            return null;
        return new Graph(p, MyRootGraph);
    }

    public IEnumerable<Node> GetNodesByAttribute(string attr_name, string attr_value)
    {
        return Nodes().Where(n => n.GetAttribute(attr_name) == attr_value);
    }

    public SubGraph GetDescendantByName(string name)
    {
        return Descendants().FirstOrDefault(d => d.GetName() == name);
    }

    public SubGraph GetOrAddSubgraph(string name)
    {
        return SubGraph.GetOrCreate(this, name);
    }

    public SubGraph GetSubgraph(string name)
    {
        return SubGraph.Get(this, name);
    }

    public Node GetOrAddNode(string name)
    {
        return Node.GetOrCreate(this, name);
    }

    public Node GetNode(string name)
    {
        return Node.Get(this, name);
    }

    /// <param name="name">
    /// Passing null as edge name (the default) will result in a new unique edge without a name.
    /// Passing the empty string has the same effect as passing null.
    /// </param>
    public Edge GetOrAddEdge(Node tail, Node head, string name = null)
    {
        return Edge.GetOrCreate(this, tail, head, name);
    }

    /// <param name="name">
    /// Passing null as edge name will return any edge between the given endpoints, regardless of their name.
    /// Passing the empty string has the same effect as passing null.
    /// </param>
    public Edge GetEdge(Node tail, Node head, string name = null)
    {
        return Edge.Get(this, tail, head, name);
    }

    ///<summary>Return an edge between two nodes, disregarding it's direction.</summary>
    /// <param name="name">
    /// Passing null as edge name will return any edge between the given endpoints, regardless of their name.
    /// Passing the empty string has the same effect as passing null.
    /// </param>
    public Edge GetEdgeBetween(Node node1, Node node2, string name = null)
    {
        return Edge.Get(this, node1, node2, name) ?? Edge.Get(this, node2, node1, name);
    }

    public IEnumerable<Edge> GetEdgesBetween(Node node1, Node node2)
    {
        return node1.Edges().Where(edge => edge.IsBetween(node1, node2));
    }

    public override string ToString()
    {
        return $"Graph {GetName()} with {Nodes().Count()} nodes.";
    }

    /// <summary>
    /// Attributes with the empty string as default are not correctly exported.
    /// https://gitlab.com/graphviz/graphviz/-/issues/1887
    /// </summary>
    public string ToDotString()
    {
        return Rjagmemwrite(_ptr);
    }

    /// <summary>
    /// Attributes with the empty string as default are not correctly exported.
    /// https://gitlab.com/graphviz/graphviz/-/issues/1887
    /// </summary>
    public void ToDotFile(string filename)
    {
        File.WriteAllText(filename, ToDotString());
    }

    /// <summary>
    /// Create and return a subgraph containing the given edges and their endpoints.
    /// </summary>
    public SubGraph AddSubgraphFromEdgeSet(string name, HashSet<Edge> edges)
    {
        var result = SubGraph.GetOrCreate(this, name);
        result.AddExisting(edges);
        // Since subgraphs can contain edges independently of their endpoints,
        // we need to add the endpoints explicitly.
        result.AddExisting(edges.SelectMany(e => new[] { e.Tail(), e.Head() }));
        return result;
    }

    /// <summary>
    /// Create a subgraph consisting of nodes from the given nodes.
    /// Edges are added to the result if both endpoints are among the nodes.
    /// Subgraphs are added to the result if they have nodes in the given nodelist.
    /// The names of the Subgraphs are of the form "name:subgraphname".
    ///
    /// Side effect: adds the returned subgraph (and its children) to self.
    /// </summary>
    public SubGraph AddSubgraphFromNodes(string name, IEnumerable<Node> nodes)
    {
        // Freeze the list of descendants,
        // since we are going to add subgraphs while iterating over existing subgraphs
        List<SubGraph> descendants = Descendants().ToList();

        SubGraph result = GetOrAddSubgraph(name);
        foreach (var node in nodes)
            result.AddExisting(node);

        Debug.Assert(result.Nodes().Count() == nodes.Count());

        // All that remains to do is to patch up the result by adding edges and subgraphs
        foreach (var node in result.Nodes())
            foreach (var edge in node.EdgesOut(this))
                if (result.Contains(edge.Head()))
                    result.AddExisting(edge);

        Debug.Assert(result.Nodes().Count() == nodes.Count());

        // Iterate over the (frozen) existing subgraphs and add new filtered subgraphs
        // in the same hierarchical position as their unfiltered counterparts.
        foreach (var subgraph in descendants)
        {
            string filteredsubgraphname = name + ":" + subgraph.GetName();
            Debug.WriteLine("Adding filtered subgraph {0}", filteredsubgraphname);
            Graph parent = subgraph.Parent();
            Graph filteredparent;
            if (parent.Equals(this))
                filteredparent = result;
            else
            {
                string parentname = name + ":" + parent.GetName();
                filteredparent = result.GetDescendantByName(parentname);
                Debug.Assert(filteredparent != null);
            }

            _ = filteredparent.AddSubgraphFilteredByNodes(filteredsubgraphname, subgraph, nodes);
        }

        Debug.Assert(result.Nodes().Count() == nodes.Count());

        // Remove subgraphs again if they are empty
        // Again, we have to freeze the descendants we are enumerating, since we are disposing on the fly
        result.SafeDeleteSubgraphs(s => !s.Nodes().Any());

        Debug.Assert(result.Nodes().Count() == nodes.Count());

        return result;
    }

    /// <summary>
    /// Delete all subgraphs in self fulfilling the predicate, without running into AccessViolationExceptions.
    /// </summary>
    public void SafeDeleteSubgraphs(Func<SubGraph, bool> predicate)
    {
        // Everytime we delete something, we restart the loop.
        // This is not efficient, but easy to implement for now.
        bool work_to_do = true;

        while (work_to_do)
        {
            work_to_do = false;
            foreach (var subgraph in Descendants())
            {
                if (!predicate(subgraph)) continue;
                subgraph.Delete();
                work_to_do = true;
                break;
            }
        }
    }

    /// <summary>
    /// Add a plain subgraph with given name to self, containing the nodes that occur both in origin
    /// and the filter. In other words, filter the origin subgraph by the filter subgraph on node-level.
    /// Attributes are copied from origin to the result.
    ///
    /// Side effect: adds the returned subgraph to self.
    /// </summary>
    public SubGraph AddSubgraphFilteredByNodes(string name, SubGraph origin, IEnumerable<Node> filter)
    {
        SubGraph result = GetOrAddSubgraph(name);
        foreach (var node in origin.Nodes().Where(filter.Contains))
            result.AddExisting(node);

        _ = origin.CopyAttributesTo(result);
        return result;
    }

    /// <summary>
    /// Create a deepcopy of the graph as a new root graph.
    /// All nodes, edges and subgraphs contained in self are copied.
    ///
    /// No side effects to self.
    /// </summary>
    /// <returns></returns>
    public RootGraph Clone(string resultname)
    {
        RootGraph result = RootGraph.CreateNew(GetGraphType(), resultname);
        _ = CopyAttributesTo(result);
        CloneInto(result);
        result.UpdateMemoryPressure();
        return result;
    }

    public void CloneInto(RootGraph target)
    {
        // Copy all nodes and edges
        foreach (var node in Nodes())
        {
            string nodename = node.GetName();
            Node newnode = target.GetOrAddNode(nodename);

            foreach (var edge in node.EdgesOut(this))
            {
                Node head = edge.Head();
                Debug.Assert(Contains(head));
                Node tail = edge.Tail();
                Debug.Assert(node.Equals(tail));
                string headname = head.GetName();
                Node newhead = target.GetOrAddNode(headname);
                string tailname = tail.GetName();
                Node newtail = target.GetNode(tailname);

                string edgename = edge.GetName();
                Edge newedge = target.GetOrAddEdge(newtail, newhead, edgename);
                _ = edge.CopyAttributesTo(newedge);
            }
            _ = node.CopyAttributesTo(newnode);
        }

        // Copy all subgraphs
        foreach (var subgraph in Descendants())
        {
            string subgraphname = subgraph.GetName();
            Graph parent = subgraph.Parent();
            Graph newparent;
            if (parent.Equals(this))
                newparent = target;
            else
            {
                string parentname = parent.GetName();
                newparent = target.GetDescendantByName(parentname);
                Debug.Assert(newparent != null);
            }
            SubGraph newsubgraph = newparent.GetOrAddSubgraph(subgraphname);
            _ = subgraph.CopyAttributesTo(newsubgraph);

            // Add the (already created) nodes and edges to newly created subgraph
            foreach (var node in subgraph.Nodes())
            {
                string nodename = node.GetName();
                Node newnode = target.GetNode(nodename);
                Debug.Assert(newnode != null);
                newsubgraph.AddExisting(newnode);

                foreach (var edge in node.EdgesOut(subgraph))
                {
                    Node head = edge.Head();
                    Node tail = edge.Tail();
                    Debug.Assert(node.Equals(tail));

                    string headname = head.GetName();
                    Node newhead = target.GetNode(headname);
                    string tailname = tail.GetName();
                    Node newtail = target.GetNode(tailname);

                    string edgename = edge.GetName();
                    Edge newedge = target.GetEdge(newtail, newhead, edgename);
                    newsubgraph.AddExisting(newedge);
                }
                _ = node.CopyAttributesTo(newnode);
            }
        }
    }

    /// <summary>
    /// Contract an edge into a newly created node with given target name.
    /// The attributes of the endpoints are merged and copied to the target node,
    /// with head attributes taking precedence over tail attributes.
    ///
    /// The end points of the given edge are removed, as well as the edge itself.
    /// Then all the neighbours of both endpoints are attached to the target,
    /// preserving direction and attributes.
    /// The new edges will be added to the root graph.
    /// If the graph is strict, no multiple edges will be added between nodes.
    /// </summary>
    /// <returns>target</returns>
    public Node Contract(Edge edge, string targetname)
    {
        return Contract(edge.Head(), edge.Tail(), targetname);
    }

    /// <summary>
    /// Perform a node contraction (also: node identification) on two nodes.
    /// The resulting node will have targetname as name.
    /// The attributes of the endpoints are merged and copied to the target node,
    /// with head attributes taking precedence over tail attributes.
    ///
    /// Both node1 and node2 will be removed from the graph.
    /// Then all the neighbours of both endpoints are attached to the target,
    /// preserving direction and attributes.
    /// The new edges will be added to the root graph.
    /// If the graph is strict, no multiple edges will be added between nodes.
    /// </summary>
    /// <returns>target</returns>
    public Node Contract(Node node1, Node node2, string targetname)
    {
        Node target = MyRootGraph.GetOrAddNode(targetname);
        _ = node1.CopyAttributesTo(target);
        _ = node2.CopyAttributesTo(target);
        Merge(node1, target);
        Merge(node2, target);
        return target;
    }

    /// <summary>
    /// Merge a node into a target node.
    /// Basically, add the neighborhood of the node to the neighborhood of the target.
    /// The merge node will be removed from the graph.
    /// The new edges will be added to the root graph.
    ///
    /// If the graph is strict, no multiple edges will be added between nodes.
    ///
    /// If add_self_loops is true, edges between the merge node and the target node will be
    /// added as self loops to the target node. Self loops that already exist as such are always added.
    /// </summary>
    public void Merge(Node merge, Node target, bool add_self_loops = false)
    {
        // .Edges() won't iterate twice over self loops
        foreach (var e in merge.Edges())
            if (!e.IsBetween(merge, target) || add_self_loops) // Only add self loops if we want that
            {
                Node newtail = e.Tail().Equals(merge) ? target : e.Tail();
                Node newhead = e.Head().Equals(merge) ? target : e.Head();
                Edge newedge = MyRootGraph.GetOrAddEdge(newtail, newhead, e.GetName());
                int returncode = e.CopyAttributesTo(newedge);
                // For some reason this may fail, even when the copying seems to have succeeded.
                Debug.Assert(returncode == 0);
            }

        // The following will delete all edges connected to merge.
        MyRootGraph.Delete(merge);
    }

    public bool IsCluster()
    {
        return GetName().StartsWith("cluster");
    }

    /// <summary>
    /// Must be true for logical tails/heads to be used in drawing.
    /// </summary>
    public bool IsCompound()
    {
        return GetAttribute("compound") == "true";
    }

    public SubGraph GetOrCreateCluster(string name)
    {
        string clustername = "cluster_" + name;
        SubGraph gvCluster = GetOrAddSubgraph(clustername);
        return gvCluster;
    }

    private static int _dummyNodeIdCounter = 0;
    private int GetNextDummyNodeId()
    {
        return _dummyNodeIdCounter++;
    }

    public Node CreateInvisibleDummyNode()
    {
        var result = GetOrAddNode("dummynode-" + GetNextDummyNodeId().ToString());
        result.MakeInvisible();
        return result;
    }

    public Node CreateSmallInvisibleDummyNode()
    {
        var result = GetOrAddNode("dummynode-" + GetNextDummyNodeId().ToString());
        result.MakeInvisibleAndSmall();
        return result;
    }

    /// <summary>
    /// Creates an invisble dummy node as landingpoint for the cluster.
    /// </summary>
    public Edge GetOrAddEdge(Node gvNode, SubGraph gvCluster, bool makeLandingSpace, string edgeName)
    {
        // If there are any edges to a cluster, we need the an invisble dummy node as endpoint,
        // because Graphviz does not support edges to clusters. We make it invisible but still
        // take it up some space because there needs to be space for the edge to land on the
        // cluster. Otherwise the edge will overlap with other edges too much, because if the
        // invisible node takes no space it will be squeezed against another node.
        Node invisibleHead;
        if (makeLandingSpace)
            invisibleHead = gvCluster.CreateInvisibleDummyNode();
        else
            invisibleHead = gvCluster.CreateSmallInvisibleDummyNode();
        var edge = GetOrAddEdge(gvNode, invisibleHead, edgeName);
        edge.SetLogicalHead(gvCluster);
        return edge;
    }

    /// <summary>
    /// Creates an invisble dummy node as landingpoint for the cluster.
    /// </summary>
    public Edge GetOrAddEdge(SubGraph gvCluster, Node gvNode, bool makeLandingSpace, string edgeName)
    {
        Node invisibleTail;
        if (makeLandingSpace)
            invisibleTail = gvCluster.CreateInvisibleDummyNode();
        else
            invisibleTail = gvCluster.CreateSmallInvisibleDummyNode();
        var edge = GetOrAddEdge(invisibleTail, gvNode, edgeName);
        edge.SetLogicalTail(gvCluster);
        return edge;
    }

    /// <summary>
    /// Creates an invisble dummy node as landingpoint for the cluster.
    /// </summary>
    public Edge GetOrAddEdge(SubGraph gvClusterTail, SubGraph gvClusterHead, bool makeLandingSpace, string edgeName)
    {
        Node invisibleTail;
        Node invisibleHead;
        if (makeLandingSpace)
        {
            invisibleTail = gvClusterTail.CreateInvisibleDummyNode();
            invisibleHead = gvClusterHead.CreateInvisibleDummyNode();
        }
        else
        {
            invisibleTail = gvClusterTail.CreateSmallInvisibleDummyNode();
            invisibleHead = gvClusterHead.CreateSmallInvisibleDummyNode();
        }
        var edge = GetOrAddEdge(invisibleTail, invisibleHead, edgeName);
        edge.SetLogicalTail(gvClusterTail);
        edge.SetLogicalHead(gvClusterHead);
        return edge;
    }

    #region layout functions and attributes

    /// <summary>
    /// Compute the layout in a separate process by calling dot.exe, and return a new graph, which is a copy of the old
    /// graph with the xdot information added to it.
    /// </summary>
    public RootGraph CreateLayout(string engine = LayoutEngines.Dot)
    {
        return GraphvizCommand.CreateLayout(this, engine: engine);
    }

    public RectangleF GetBoundingBox()
    {
        string bb_string = Agget(_ptr, "bb");
        if (string.IsNullOrEmpty(bb_string))
            return default;
        // x and y are the topleft point of the bb
        char sep = ',';
        string[] bb = bb_string.Split(sep);
        float x = float.Parse(bb[0], NumberStyles.Any, CultureInfo.InvariantCulture);
        float y = float.Parse(bb[1], NumberStyles.Any, CultureInfo.InvariantCulture);
        float w = float.Parse(bb[2], NumberStyles.Any, CultureInfo.InvariantCulture) - x;
        float h = float.Parse(bb[3], NumberStyles.Any, CultureInfo.InvariantCulture) - y;
        return new RectangleF(x, y, w, h);
    }

    public IReadOnlyList<XDotOp> GetDrawing() => GetXDotValue(this, "_draw_");
    public IReadOnlyList<XDotOp> GetLabelDrawing() => GetXDotValue(this, "_ldraw_");

    private void ToFile(string filepath, string format, string engine)
    {
        _ = GraphvizCommand.Exec(this, format: format, filepath, engine: engine);
    }

    public void ToSvgFile(string filepath, string engine = LayoutEngines.Dot) => ToFile(filepath, "svg", engine);
    public void ToPngFile(string filepath, string engine = LayoutEngines.Dot) => ToFile(filepath, "png", engine);
    public void ToPdfFile(string filepath, string engine = LayoutEngines.Dot) => ToFile(filepath, "pdf", engine);
    public void ToPsFile(string filepath, string engine = LayoutEngines.Dot) => ToFile(filepath, "ps", engine);
    #endregion


    #region in-place layout computation

    /// <summary>
    /// Compute a layout for this graph, in-process, on the given graph.
    /// It is recommended to use <see cref="CreateLayout"/> instead, as that comes with less footguns and a better API. 
    /// Moreover, experience shows it is less likely to trip over lingering graphviz bugs as well.
    /// NB: The method FreeLayout should always be called as soon as the layout information
    /// of a graph is not needed anymore.
    /// </summary>
    public void ComputeLayout(string engine = LayoutEngines.Dot)
    {
        int layout_rc = GvLayout(GVC, _ptr, engine);
        if (layout_rc != 0)
            throw new ApplicationException($"Graphviz layout returned error code {layout_rc}");

        // Calling gvRender this way sets attributes to the graph etc
        // The engine specified here doesn't have to be the same as the above.
        // We always want to use xdot here, independently of the layout algorithm,
        // to ensure a consistent attribute layout.
        int render_rc = GvRender(GVC, _ptr, "xdot", IntPtr.Zero);
        if (render_rc != 0)
            throw new ApplicationException($"Graphviz render returned error code {render_rc}");
    }

    /// <summary>
    /// Clean up the layout information stored in this graph. This does not include the attributes set by GvRender.
    /// This method should always be called as soon as the layout information of a graph is not needed anymore.
    /// NB: this method must not be called after modifications to the graph have been made!
    /// This could result an AccessViolationException.
    /// </summary>
    public void FreeLayout()
    {
        var free_rc = GvFreeLayout(GVC, _ptr);
        if (free_rc != 0)
            throw new ApplicationException($"Graphviz render returned error code {free_rc}");
    }

    /// <summary>
    /// Should only be called after <see cref="ComputeLayout"/> has been called.
    /// </summary>
    [Obsolete("This method is only available after ComputeLayout(), and may crash otherwise. It is obsoleted by the other ToXXXFile methods.")]
    public void RenderToFile(string filename, string format)
    {
        var render_rc = GvRenderFilename(GVC, _ptr, format, filename);
        if (render_rc != 0)
            throw new ApplicationException($"Graphviz render returned error code {render_rc}");
    }

    [Obsolete("This method is only available after ComputeLayout(), and may crash otherwise. It is obsoleted by GetLabelDrawing(). Refer to tutorial.")]
    public GraphvizLabel GetLabel()
    {
        IntPtr labelptr = GraphLabel(_ptr);
        if (labelptr == IntPtr.Zero)
            return null;
        return new GraphvizLabel(labelptr, BoundingBoxCoords.Centered);
    }

    #endregion
}
