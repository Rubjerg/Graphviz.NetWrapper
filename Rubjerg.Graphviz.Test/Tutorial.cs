using NUnit.Framework;
using System.Drawing;
using System.Linq;

namespace Rubjerg.Graphviz.Test
{
    [TestFixture()]
    public class Tutorial
    {
        public const string PointPattern = @"{X=[\d.]+, Y=[\d.]+}";
        public const string RectPattern = @"{X=[\d.]+,Y=[\d.]+,Width=[\d.]+,Height=[\d.]+}";
        public const string SplinePattern =
            @"{X=[\d.]+, Y=[\d.]+}, {X=[\d.]+, Y=[\d.]+}, {X=[\d.]+, Y=[\d.]+}, {X=[\d.]+, Y=[\d.]+}";

        [Test, Order(1)]
        public void GraphConstruction()
        {
            // You can programmatically construct graphs as follows
            RootGraph root = RootGraph.CreateNew(GraphType.Directed, "Some Unique Identifier");
            // The graph name is optional, and can be omitted. The name is not interpreted by Graphviz,
            // except it is recorded and preserved when the graph is written as a file.

            // The node names are unique identifiers within a graph in Graphviz
            Node nodeA = root.GetOrAddNode("A");
            Node nodeB = root.GetOrAddNode("B");
            Node nodeC = root.GetOrAddNode("C");

            // The edge name is only unique between two nodes
            Edge edgeAB = root.GetOrAddEdge(nodeA, nodeB, "Some edge name");
            Edge edgeBC = root.GetOrAddEdge(nodeB, nodeC, "Some edge name");
            Edge anotherEdgeBC = root.GetOrAddEdge(nodeB, nodeC, "Another edge name");

            // An edge name is optional and omitting it will result in a new nameless edge.
            // There can be multiple nameless edges between any two nodes.
            Edge edgeAB1 = root.GetOrAddEdge(nodeA, nodeB);
            Edge edgeAB2 = root.GetOrAddEdge(nodeA, nodeB);
            Assert.AreNotEqual(edgeAB1, edgeAB2);

            // We can attach attributes to nodes, edges and graphs to store information and instruct
            // Graphviz by specifying layout parameters. At the moment we only support string
            // attributes. Cgraph assumes that all objects of a given kind (graphs/subgraphs, nodes,
            // or edges) have the same attributes. An attribute has to be introduced with a default value
            // first for a certain kind, before we can use it.
            Node.IntroduceAttribute(root, "my attribute", "defaultvalue");
            nodeA.SetAttribute("my attribute", "othervalue");

            // Attributes are introduced per kind (Node, Edge, Graph) per root graph.
            // So to be able to use "my attribute" on edges, we first have to introduce it as well.
            Edge.IntroduceAttribute(root, "my attribute", "defaultvalue");
            edgeAB.SetAttribute("my attribute", "othervalue");

            // To introduce and set an attribute at the same time, there are convenience wrappers
            edgeBC.SafeSetAttribute("arrowsize", "2.0", "1.0");
            // If we set an unintroduced attribute, the attribute will be introduced with an empty default value.
            edgeBC.SetAttribute("new attr", "value");

            // Some attributes - like "label" - accept HTML strings as value
            // To tell Graphviz that a string should be interpreted as HTML use the designated methods
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

            // We can ask Graphviz to compute a layout and render it to svg
            root.ToSvgFile(TestContext.CurrentContext.TestDirectory + "/dot_out.svg");

            // We can use layout engines other than dot by explicitly passing the engine we want
            root.ToSvgFile(TestContext.CurrentContext.TestDirectory + "/neato_out.svg", LayoutEngines.Neato);

            // Or we can ask Graphviz to compute the layout and programatically read out the layout attributes
            // This will create a copy of our original graph with layout information attached to it in the form
            // of attributes.
            RootGraph layout = root.CreateLayout();

            // There are convenience methods available that parse these attributes for us and give
            // back the layout information in an accessible form.
            Node nodeA = layout.GetNode("A");
            PointF position = nodeA.GetPosition();
            Utils.AssertPattern(PointPattern, position.ToString());

            RectangleF nodeboundingbox = nodeA.GetBoundingBox();
            Utils.AssertPattern(RectPattern, nodeboundingbox.ToString());

            // Or splines between nodes
            Node nodeB = layout.GetNode("B");
            Edge edge = layout.GetEdge(nodeA, nodeB, "Some edge name");
            PointF[] spline = edge.GetFirstSpline();
            string splineString = string.Join(", ", spline.Select(p => p.ToString()));
            Utils.AssertPattern(SplinePattern, splineString);

            // If we require detailed drawing information for any object, we can retrieve the so called "xdot"
            // operations. See https://graphviz.org/docs/outputs/canon/#xdot for a specification.
            var activeColor = Color.Black;
            foreach (var op in nodeA.GetDrawing())
            {
                if (op is XDotOp.FillColor { Value: string htmlColor })
                {
                    activeColor = ColorTranslator.FromHtml(htmlColor);
                }
                else if (op is XDotOp.FilledEllipse { Value: var filledEllipse })
                {
                    var boundingBox = filledEllipse.ToRectangleF();
                    Utils.AssertPattern(RectPattern, boundingBox.ToString());
                }
                // Handle any xdot operation you require
            }

            var activeFont = XDotFont.Default;
            foreach (var op in nodeA.GetDrawing())
            {
                if (op is XDotOp.Font { Value: var font })
                {
                    activeFont = font;
                    Utils.AssertPattern(@"Times-Roman", font.Name);
                }
                else if (op is XDotOp.Text { Value: var text })
                {
                    var anchor = text.Anchor();
                    Utils.AssertPattern(PointPattern, anchor.ToString());
                    var boundingBox = text.TextBoundingBox(activeFont);
                    Utils.AssertPattern(RectPattern, boundingBox.ToString());
                    Assert.AreEqual(text.Text, "A");
                }
                // Handle any xdot operation you require
            }

            // These are just simple examples to showcase the structure of xdot operations.
            // In reality the information can be much richer and more complex.
        }

        [Test, Order(3)]
        public void Clusters()
        {
            RootGraph root = RootGraph.CreateNew(GraphType.Directed, "Graph with clusters");
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

            var layout = root.CreateLayout();

            SubGraph cluster = layout.GetSubgraph("cluster_1");
            RectangleF clusterbox = cluster.GetBoundingBox();
            RectangleF rootgraphbox = layout.GetBoundingBox();
            Utils.AssertPattern(RectPattern, clusterbox.ToString());
            Utils.AssertPattern(RectPattern, rootgraphbox.ToString());
        }

        [Test, Order(4)]
        public void Records()
        {
            RootGraph root = RootGraph.CreateNew(GraphType.Directed, "Graph with records");
            Node nodeA = root.GetOrAddNode("A");
            nodeA.SafeSetAttribute("shape", "record", "");
            nodeA.SafeSetAttribute("label", "1|2|3|{4|5}|6|{7|8|9}", "\\N");

            var layout = root.CreateLayout();

            // The order of the list matches the order in which the labels occur in the label string above.
            var rects = layout.GetNode("A").GetRecordRectangles().ToList();
            Assert.AreEqual(9, rects.Count);
        }

        [Test, Order(5)]
        public void StringEscaping()
        {
            RootGraph root = RootGraph.CreateNew(GraphType.Directed, "Graph with escaped strings");
            Node.IntroduceAttribute(root, "label", "\\N");
            Node nodeA = root.GetOrAddNode("A");

            // Several characters and character sequences can have special meanings in labels, like \N.
            // When you want to have a literal string in a label, we provide a convenience function for you to do just that.
            nodeA.SetAttribute("label", CGraphThing.EscapeLabel("Some string literal \\N \\n |}>"));

            // When defining portnames, some characters, like ':' and '|', are not allowed and they can't be escaped either.
            // This can be troubling if you have an externally defined ID for such a port.
            // We provide a function that maps strings to valid portnames.
            var somePortId = "port id with :| special characters";
            var validPortName = Edge.ConvertUidToPortName(somePortId);
            Node nodeB = root.GetOrAddNode("B");
            nodeB.SafeSetAttribute("shape", "record", "");
            nodeB.SafeSetAttribute("label", $"<{validPortName}>1|2", "\\N");

            // The conversion function makes sure different strings don't accidentally map onto the same portname
            Assert.AreNotEqual(Edge.ConvertUidToPortName(":"), Edge.ConvertUidToPortName("|"));
        }
    }
}
