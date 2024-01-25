using System.Linq;
using System.Runtime.InteropServices;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Rubjerg.Graphviz.Test
{
    [TestFixture()]
    public class CGraphEdgeCases
    {
        [DllImport("GraphvizWrapper.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        protected static extern void rj_debug();

        [Test()]
        [Ignore("For debugging")]
        public void debug()
        {
            NativeMethods.CreateConsole();
            rj_debug();
        }

        [Test()]
        public void TestAttributeIntroduction()
        {
            RootGraph root = Utils.CreateUniqueTestGraph();
            Graph.IntroduceAttribute(root, "test", "default");
            root.SetAttribute("test", "1");
            ClassicAssert.AreEqual("1", root.GetAttribute("test"));
            Graph.IntroduceAttribute(root, "test", "default");
            // Now the value has been reset!
            ClassicAssert.AreEqual("default", root.GetAttribute("test"));

            // This is not the case for nodes
            Node node = root.GetOrAddNode("nodename");
            Node.IntroduceAttribute(root, "test", "default");
            node.SetAttribute("test", "1");
            ClassicAssert.AreEqual("1", node.GetAttribute("test"));
            Node.IntroduceAttribute(root, "test", "default");
            ClassicAssert.AreEqual("1", node.GetAttribute("test"));
        }

        [Test()]
        public void TestAttributeDefaults()
        {
            {
                RootGraph root = Utils.CreateUniqueTestGraph();
                Node.IntroduceAttribute(root, "label", "");
                Node nodeA = root.GetOrAddNode("A");
                Node nodeB = root.GetOrAddNode("B");
                nodeA.SetAttribute("label", "1");
                ClassicAssert.AreEqual("1", nodeA.GetAttribute("label"));
                ClassicAssert.AreEqual("", nodeB.GetAttribute("label"));
                root.ToDotFile(TestContext.CurrentContext.TestDirectory + "/out.gv");
            }

            {
                var root = RootGraph.FromDotFile(TestContext.CurrentContext.TestDirectory + "/out.gv");
                Node nodeA = root.GetNode("A");
                Node nodeB = root.GetNode("B");
                ClassicAssert.AreEqual("1", nodeA.GetAttribute("label"));
                ClassicAssert.AreEqual("", nodeB.GetAttribute("label"));

                root.ComputeLayout();
                ClassicAssert.AreEqual("1", nodeA.GetAttribute("label"));
                ClassicAssert.AreEqual("", nodeB.GetAttribute("label"));
                root.ToSvgFile(TestContext.CurrentContext.TestDirectory + "/out.svg");
            }
            // The empty label default is not exported, but that appears to be no problem here. The
            // default seems to become the empty string. However, dot.exe gives a different result.
            // When applying dot.exe on out.gv the default label is set to \N, which gives different
            // results entirely.
            // Related issue: https://gitlab.com/graphviz/graphviz/-/issues/1887
        }

        [Test()]
        public void TestCopyUnintroducedAttributes()
        {
            RootGraph root = Utils.CreateUniqueTestGraph();
            Node n1 = root.GetOrAddNode("1");
            Node.IntroduceAttribute(root, "test", "foo");
            ClassicAssert.AreEqual("foo", n1.GetAttribute("test"));
            n1.SetAttribute("test", "bar");
            ClassicAssert.AreEqual("bar", n1.GetAttribute("test"));
            Node.IntroduceAttribute(root, "test2", "foo2");
            ClassicAssert.AreEqual("foo2", n1.GetAttribute("test2"));
            n1.SetAttribute("test2", "bar2");
            ClassicAssert.AreEqual("bar2", n1.GetAttribute("test2"));

            RootGraph root2 = Utils.CreateUniqueTestGraph();
            Node n2 = root2.GetOrAddNode("2");
            // Only introduce the second attr, and see whether it gets copied
            Node.IntroduceAttribute(root, "test2", "foo2");
            ClassicAssert.AreEqual(null, n2.GetAttribute("test"));
            // While one would expect test2 to be copied correctly, it isn't, as copying test failed before that.
            //ClassicAssert.AreEqual("bar2", n2.GetAttribute("test2"));
            ClassicAssert.AreEqual(null, n2.GetAttribute("test2"));
        }


        [Test()]
        public void TestCopyToNewRoot()
        {
            RootGraph root = Utils.CreateUniqueTestGraph();
            Node n1 = root.GetOrAddNode("1");
            Node.IntroduceAttribute(root, "test", "foo");
            ClassicAssert.AreEqual("foo", n1.GetAttribute("test"));
            n1.SetAttribute("test", "bar");
            ClassicAssert.AreEqual("bar", n1.GetAttribute("test"));

            RootGraph root2 = Utils.CreateUniqueTestGraph();
            Node.IntroduceAttribute(root2, "test", "foo");
            Node n2 = n1.CopyToOtherRoot(root2);
            ClassicAssert.AreEqual("1", n2.GetName());
            ClassicAssert.AreEqual("bar", n2.GetAttribute("test"));
        }

        [Test()]
        public void TestNodeAndGraphWithSameName()
        {
            RootGraph root = Utils.CreateUniqueTestGraph();
            SubGraph sub = root.GetOrAddSubgraph("name");
            Node node = sub.GetOrAddNode("name");
            ClassicAssert.True(root.Contains(sub));
            ClassicAssert.True(sub.Contains(node));
        }

        [Test()]
        public void TestRootOfRoot()
        {
            RootGraph root = Utils.CreateUniqueTestGraph();
            RootGraph root2 = root.MyRootGraph;
        }

        [Test()]
        public void TestSelfLoopEnumeration()
        {
            RootGraph graph = Utils.CreateUniqueTestGraph();
            Node node = graph.GetOrAddNode("node 1");
            Node node2 = graph.GetOrAddNode("node 2");
            Edge edgein = graph.GetOrAddEdge(node2, node, "in");
            Edge edgeout = graph.GetOrAddEdge(node, node2, "out");
            Edge edgeself = graph.GetOrAddEdge(node, node, "self");

            ClassicAssert.AreEqual(2, node.InDegree());
            ClassicAssert.AreEqual(2, node.OutDegree());
            ClassicAssert.AreEqual(4, node.TotalDegree());

            ClassicAssert.AreEqual(2, node.EdgesIn().Count());
            ClassicAssert.AreEqual(2, node.EdgesOut().Count());
            ClassicAssert.AreEqual(3, node.Edges().Count());
        }

        // .NET uses UnmanagedType.Bool by default for P/Invoke, but our C++ code uses UnmanagedType.U1
        [DllImport("GraphvizWrapper.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        protected static extern bool echobool([MarshalAs(UnmanagedType.U1)] bool arg);
        [DllImport("GraphvizWrapper.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        protected static extern int echoint(int arg);
        [Test()]
        public void TestMarshaling()
        {
            ClassicAssert.True(echobool(true));
            ClassicAssert.False(echobool(false));

            ClassicAssert.AreEqual(0, echoint(0));
            ClassicAssert.AreEqual(1, echoint(1));
            ClassicAssert.AreEqual(-1, echoint(-1));
        }

        [Test()]
        public void TestNonStrictGraph()
        {
            RootGraph graph = Utils.CreateUniqueTestGraph();
            ClassicAssert.False(graph.IsStrict());
            ClassicAssert.False(graph.IsUndirected());
            ClassicAssert.True(graph.IsDirected());

            Node node = graph.GetOrAddNode("node 1");
            Node node2 = graph.GetOrAddNode("node 2");

            Edge edge = graph.GetOrAddEdge(node, node, "edge 1");
            Edge edge2 = graph.GetOrAddEdge(node, node, "edge 2");
            ClassicAssert.AreNotEqual(edge.GetName(), edge2.GetName());

            Edge edge3 = graph.GetOrAddEdge(node, node2, "edge 3");
            Edge edge4 = graph.GetOrAddEdge(node, node2, "edge 4");
            ClassicAssert.AreNotEqual(edge3.GetName(), edge4.GetName());
        }

        [Test()]
        public void TestEdgeEquals()
        {
            RootGraph graph = Utils.CreateUniqueTestGraph();
            Node node = graph.GetOrAddNode("node 1");
            Node node2 = graph.GetOrAddNode("node 2");
            Node node3 = graph.GetOrAddNode("node 3");
            Edge edge = graph.GetOrAddEdge(node, node, "edge 1");
            Edge edge2 = graph.GetOrAddEdge(node, node, "edge 2");
            Edge edge3 = graph.GetOrAddEdge(node, node2, "edge 3");
            Edge edge4 = graph.GetOrAddEdge(node2, node3, "edge 4");

            ClassicAssert.AreEqual(edge, edge);
            ClassicAssert.AreNotEqual(edge, edge2);
            ClassicAssert.AreNotEqual(edge, edge3);
            ClassicAssert.AreNotEqual(edge, edge4);
            ClassicAssert.AreEqual(edge.GetHashCode(), edge.GetHashCode());
            ClassicAssert.AreNotEqual(edge.GetHashCode(), edge2.GetHashCode());
            ClassicAssert.AreNotEqual(edge.GetHashCode(), edge3.GetHashCode());
            ClassicAssert.AreNotEqual(edge.GetHashCode(), edge4.GetHashCode());
        }


        [Test()]
        public void TestCreateNestedStructures()
        {
            // Documentation:
            // Subgraphs are an important construct in Cgraph.They are intended for organizing subsets of
            // graph objects and can be used interchangeably with top - level graphs in almost all Cgraph
            // functions.  A subgraph may contain any nodes or edges of its parent. (When an edge is
            // inserted in a subgraph, its nodes are also implicitly inserted if necessary.Similarly,
            // insertion of a node or edge automatically implies insertion in all containing subgraphs up
            // to the root.) Subgraphs of a graph form a hierarchy(a tree).Cgraph has functions to
            // create, search, and iterate over subgraphs.

            // Conclusion: the hierarchical tree structure is maintained in a sane way across all
            // operations we can do w.r.t. subgraphs.


            // If a node is created in a subgraph, it should also be contained in all supergraphs
            RootGraph graph = Utils.CreateUniqueTestGraph();
            SubGraph supergraph = graph.GetOrAddSubgraph("level 1");
            string subgraphname = "level 2";
            SubGraph subgraph = supergraph.GetOrAddSubgraph(subgraphname);
            string nodename = "test node";
            Node node = subgraph.GetOrAddNode(nodename);

            // Node must be contained in super graph
            ClassicAssert.True(node.MyRootGraph.Equals(graph));
            ClassicAssert.True(supergraph.Contains(node));
            ClassicAssert.True(supergraph.Nodes().Contains(node));
            ClassicAssert.NotNull(supergraph.GetNode(nodename));
            // Node must be contained in root graph
            ClassicAssert.True(graph.Contains(node));
            ClassicAssert.True(graph.Nodes().Contains(node));
            ClassicAssert.NotNull(graph.GetNode(nodename));

            // Subgraph must be contained in super graph
            ClassicAssert.True(supergraph.Contains(subgraph));
            ClassicAssert.True(supergraph.Descendants().Contains(subgraph));
            ClassicAssert.NotNull(supergraph.GetSubgraph(subgraphname));
            // Subgraph must be contained in root graph
            ClassicAssert.True(graph.Contains(subgraph));
            ClassicAssert.True(graph.Descendants().Contains(subgraph));
            // Subgraph cannot be obtained in the following way:
            //graph.GetSubgraph(subgraphname)
            ClassicAssert.Null(graph.GetSubgraph(subgraphname));
            // Use a utility function instead:
            ClassicAssert.NotNull(graph.GetDescendantByName(subgraphname));
        }


        [Test()]
        public void TestEdgesInSubgraphs()
        {
            RootGraph graph = Utils.CreateUniqueTestGraph();
            Node node = graph.GetOrAddNode("node");
            Edge edge = graph.GetOrAddEdge(node, node, "edge 1");

            SubGraph subgraph = graph.GetOrAddSubgraph("sub graph");
            Node subnode = subgraph.GetOrAddNode("subnode");
            Edge subedge_between_node = subgraph.GetOrAddEdge(node, node, "edge 2");
            Edge subedge_between_subnode = subgraph.GetOrAddEdge(subnode, subnode, "edge 3");
            Edge edge_between_subnode = graph.GetOrAddEdge(subnode, subnode, "edge 4");

            ClassicAssert.True(graph.Contains(edge));
            ClassicAssert.True(graph.Contains(subedge_between_node));
            ClassicAssert.True(graph.Contains(subedge_between_subnode));
            ClassicAssert.True(graph.Contains(edge_between_subnode));

            ClassicAssert.False(subgraph.Contains(edge));
            ClassicAssert.True(subgraph.Contains(subedge_between_node));
            ClassicAssert.True(subgraph.Contains(subedge_between_subnode));
            ClassicAssert.False(subgraph.Contains(edge_between_subnode));

            // Conclusion:
            // Subgraphs can contain edges, independently of their endpoints.
            // This affects enumeration as follows:
            ClassicAssert.AreEqual(2, node.EdgesOut(graph).Count());
            ClassicAssert.AreEqual(1, node.EdgesOut(subgraph).Count());
            ClassicAssert.AreEqual(2, subnode.EdgesOut(graph).Count());
            ClassicAssert.AreEqual(1, subnode.EdgesOut(subgraph).Count());
        }


        [Test()]
        public void TestRecursiveSubgraphDeletion()
        {
            RootGraph graph = Utils.CreateUniqueTestGraph();
            Graph.IntroduceAttribute(graph, "label", "");
            graph.SetAttribute("label", "xx");
            Node node = graph.GetOrAddNode("node");
            SubGraph subgraph = graph.GetOrAddSubgraph("subgraph");
            subgraph.SetAttribute("label", "x");
            Node subnode = subgraph.GetOrAddNode("subnode");
            SubGraph subsubgraph = subgraph.GetOrAddSubgraph("subsubgraph");
            subsubgraph.SetAttribute("label", "x");
            Node subsubnode = subsubgraph.GetOrAddNode("subsubnode");

            // Now deleting subgraph should also delete subsubgraph
            ClassicAssert.AreNotEqual(null, subgraph.GetSubgraph("subsubgraph"));
            ClassicAssert.AreNotEqual(null, graph.GetDescendantByName("subsubgraph"));
            subgraph.Delete();
            ClassicAssert.AreEqual(null, graph.GetDescendantByName("subsubgraph"));
        }

    }
}
