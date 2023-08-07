using NUnit.Framework;
using System.Drawing;
using System.Linq;

namespace Rubjerg.Graphviz.Test
{
    // FIXNOW: rename this tutorial to OldTutorial for test coverage reasons and write a new, adapted tutorial
    [TestFixture()]
    public class Tutorial
    {
        [Test, Order(1)]
        public void GraphConstruction()
        {
            // You can programmatically construct graphs as follows
            RootGraph root = RootGraph.CreateNew("Some Unique Identifier", GraphType.Directed);

            // The node names are unique identifiers within a graph in Graphviz
            Node nodeA = root.GetOrAddNode("A");
            Node nodeB = root.GetOrAddNode("B");
            Node nodeC = root.GetOrAddNode("C");
            Node nodeD = root.GetOrAddNode("D");

            // The edge name is only unique between two nodes
            Edge edgeAB = root.GetOrAddEdge(nodeA, nodeB, "Some edge name");
            Edge edgeBC = root.GetOrAddEdge(nodeB, nodeC, "Some edge name");
            Edge anotherEdgeBC = root.GetOrAddEdge(nodeB, nodeC, "Another edge name");

            // We can attach attributes to nodes, edges and graphs to store information and instruct
            // graphviz by specifying layout parameters. At the moment we only support string
            // attributes. Cgraph assumes that all objects of a given kind (graphs/subgraphs, nodes,
            // or edges) have the same attributes. The attributes first have to be introduced for a
            // certain kind, before we can use it.
            Node.IntroduceAttribute(root, "my attribute", "defaultvalue");
            nodeA.SetAttribute("my attribute", "othervalue");

            // Attributes are introduced per kind (Node, Edge, Graph) per root graph.
            // So to be able to use "my attribute" on edges, we first have to introduce it as well.
            Edge.IntroduceAttribute(root, "my attribute", "defaultvalue");
            edgeAB.SetAttribute("my attribute", "othervalue");

            // To introduce and set an attribute at the same time, there are convenience wrappers
            edgeBC.SafeSetAttribute("arrowsize", "2.0", "1.0");

            // Some attributes - like "label" - accept HTML strings as value
            // To tell graphviz that a string should be interpreted as HTML use the designated methods
            Node.IntroduceAttribute(root, "label", "defaultlabel");
            nodeB.SetAttributeHtml("label", "<b>Some HTML string</b>");

            // We can simply export this graph to a text file in dot format
            root.ToDotFile(TestContext.CurrentContext.TestDirectory + "/out.dot");
        }

        [Test, Order(2)]
        public void Layouting()
        {
            // If we have a given dot file (in this case the one we generated above), we can also read it back in
            RootGraph root = RootGraph.FromDotFile(TestContext.CurrentContext.TestDirectory + "/out.dot");

            // Let's have graphviz compute a dot layout for us
            root.ComputeLayout();

            // We can export this to svg
            root.ToSvgFile(TestContext.CurrentContext.TestDirectory + "/dot_out.svg");

            // Or programatically read out the layout attributes
            Node nodeA = root.GetNode("A");
            PointF position = nodeA.Position();
            Utils.AssertPattern(@"{X=[\d.]+, Y=[\d.]+}", position.ToString());

            // Like a bounding box of an object
            RectangleF nodeboundingbox = nodeA.BoundingBox();
            Utils.AssertPattern(@"{X=[\d.]+,Y=[\d.]+,Width=[\d.]+,Height=[\d.]+}", nodeboundingbox.ToString());

            // Or splines between nodes
            Node nodeB = root.GetNode("B");
            Edge edge = root.GetEdge(nodeA, nodeB, "Some edge name");
            PointF[] spline = edge.FirstSpline();
            string splineString = string.Join(", ", spline.Select(p => p.ToString()));
            string expectedSplinePattern =
                @"{X=[\d.]+, Y=[\d.]+}, {X=[\d.]+, Y=[\d.]+}, {X=[\d.]+, Y=[\d.]+},"
                + @" {X=[\d.]+, Y=[\d.]+}, {X=[\d.]+, Y=[\d.]+}";
            Utils.AssertPattern(expectedSplinePattern, splineString);

            GraphvizLabel nodeLabel = nodeA.GetLabel();
            Utils.AssertPattern(@"{X=[\d.]+,Y=[\d.]+,Width=[\d.]+,Height=[\d.]+}",
                nodeLabel.BoundingBox().ToString());
            Utils.AssertPattern(@"Times-Roman", nodeLabel.FontName().ToString());

            // Once all layout information is obtained from the graph, the resources should be
            // reclaimed. To do this, the application should call the cleanup routine associated
            // with the layout algorithm used to draw the graph. This is done by a call to
            // FreeLayout(). A given graph can be laid out multiple times. The application, however,
            // must clean up the earlier layout's information with a call to FreeLayout before
            // invoking a new layout function.
            root.FreeLayout();

            // We can use layout engines other than dot by explicitly passing the engine we want
            root.ComputeLayout(LayoutEngines.Neato);
            root.ToSvgFile(TestContext.CurrentContext.TestDirectory + "/neato_out.svg");
        }

        [Test, Order(3)]
        public void Clusters()
        {
            RootGraph root = RootGraph.CreateNew("Graph with clusters", GraphType.Directed);
            Node nodeA = root.GetOrAddNode("A");
            Node nodeB = root.GetOrAddNode("B");
            Node nodeC = root.GetOrAddNode("C");
            Node nodeD = root.GetOrAddNode("D");

            // When a subgraph name is prefixed with cluster,
            // the dot layout engine will render it as a box around the containing nodes.
            SubGraph cluster1 = root.GetOrAddSubgraph("cluster_1");
            cluster1.AddExisting(nodeB);
            cluster1.AddExisting(nodeC);
            SubGraph cluster2 = root.GetOrAddSubgraph("cluster_2");
            cluster2.AddExisting(nodeD);

            // COMPOUND EDGES
            // Graphviz does not really support edges from and to clusters. However, by adding an
            // invisible dummynode and setting the ltail or lhead attributes of an edge this
            // behavior can be faked. Graphviz will then draw an edge to the dummy node but clip it
            // at the border of the cluster. We provide convenience methods for this.
            // To enable this feature, Graphviz requires us to set the "compound" attribute to "true".
            Graph.IntroduceAttribute(root, "compound", "true"); // Allow lhead/ltail
            // The boolean indicates whether the dummy node should take up any space. When you pass
            // false and you have a lot of edges, the edges may start to overlap a lot.
            _ = root.GetOrAddEdge(nodeA, cluster1, false, "edge to a cluster");
            _ = root.GetOrAddEdge(cluster1, nodeD, false, "edge from a cluster");
            _ = root.GetOrAddEdge(cluster1, cluster1, false, "edge between clusters");

            root.ComputeLayout();

            SubGraph cluster = root.GetSubgraph("cluster_1");
            RectangleF clusterbox = cluster.BoundingBox();
            RectangleF rootgraphbox = root.BoundingBox();
            Utils.AssertPattern(@"{X=[\d.]+,Y=[\d.]+,Width=[\d.]+,Height=[\d.]+}", clusterbox.ToString());
            Utils.AssertPattern(@"{X=[\d.]+,Y=[\d.]+,Width=[\d.]+,Height=[\d.]+}", rootgraphbox.ToString());
        }

        [Test, Order(4)]
        public void Records()
        {
            RootGraph root = RootGraph.CreateNew("Graph with records", GraphType.Directed);
            Node nodeA = root.GetOrAddNode("A");
            nodeA.SafeSetAttribute("shape", "record", "");
            nodeA.SafeSetAttribute("label", "1|2|3|{4|5}|6|{7|8|9}", "\\N");

            root.ComputeLayout();

            // The order of the list matches the order in which the labels occur in the label string above.
            var rects = nodeA.GetRecordRectangles().ToList();
            Assert.That(rects.Count, Is.EqualTo(9));
        }

        [Test, Order(5)]
        public void StringEscaping()
        {
            RootGraph root = RootGraph.CreateNew("Graph with escaped strings", GraphType.Directed);
            Node.IntroduceAttribute(root, "label", "\\N");
            Node nodeA = root.GetOrAddNode("A");

            // Several characters and character sequences can have special meanings in labels, like \N.
            // When you want to have a literal string in a label, we provide a convenience function for you to do just that.
            nodeA.SetAttribute("label", CGraphThing.EscapeLabel("Some string literal \\N \\n |}>"));

            root.ComputeLayout();

            // When defining portnames, some characters, like ':' and '|', are not allowed and they can't be escaped either.
            // This can be troubling if you have an externally defined ID for such a port.
            // We provide a function that maps strings to valid portnames.
            var somePortId = "port id with :| special characters";
            var validPortName = Edge.ConvertUidToPortName(somePortId);
            Node nodeB = root.GetOrAddNode("B");
            nodeB.SafeSetAttribute("shape", "record", "");
            nodeB.SafeSetAttribute("label", $"<{validPortName}>1|2", "\\N");

            // The function makes sure different strings don't accidentally map onto the same portname
            Assert.That(Edge.ConvertUidToPortName(":"), Is.Not.EqualTo(Edge.ConvertUidToPortName("|")));
        }
    }
}
