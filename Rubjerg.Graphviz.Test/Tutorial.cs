using NUnit.Framework;
using System.Drawing;
using System.Linq;

namespace Rubjerg.Graphviz.Test
{
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

            // When a subgraph name is prefixed with cluster,
            // the dot layout engine will render it as a box around the containing nodes.
            SubGraph cluster = root.GetOrAddSubgraph("cluster_1");
            cluster.AddExisting(nodeB);
            cluster.AddExisting(nodeC);

            // We can attach attributes to nodes, edges and graphs to store information and instruct
            // graphviz by specifying layout parameters. At the moment we only support string
            // attributes. Cgraph assumes that all objects of a given kind (graphs/subgraphs, nodes,
            // or edges) have the same attributesThe attributes first have to be introduced for a
            // certain kind, before we can use it.
            Node.IntroduceAttribute(root, "my attribute", "defaultvalue");
            nodeA.SetAttribute("my attribute", "othervalue");

            // To introduce and set an attribute at the same time, there are convenience wrappers
            edgeAB.SafeSetAttribute("color", "red", "black");
            edgeBC.SafeSetAttribute("arrowsize", "2.0", "1.0");

            // We can simply export this graph to a text file in dot format
            root.ToDotFile(TestContext.CurrentContext.TestDirectory + "/out.dot");
        }

        [Test, Order(2)]
        public void Layouting()
        {
            // If we have a given dot file, we can also simply read it back in
            RootGraph root = RootGraph.FromDotFile(TestContext.CurrentContext.TestDirectory + "/out.dot");

            // Let's have graphviz compute a dot layout for us
            root.ComputeLayout();

            // We can export this to svg
            root.ToSvgFile(TestContext.CurrentContext.TestDirectory + "/dot_out.svg");

            // Or programatically read out the layout attributes
            Node nodeA = root.GetNode("A");
            PointF position = nodeA.Position();
            Assert.AreEqual("{X=43, Y=192.1739}", position.ToString());

            RectangleF nodeboundingbox = nodeA.BoundingBox();
            Assert.AreEqual("{X=16,Y=171.3391,Width=54,Height=41.66957}", nodeboundingbox.ToString());

            Node nodeB = root.GetNode("B");
            Edge edge = root.GetEdge(nodeA, nodeB, "Some edge name");
            PointF[] spline = edge.FirstSpline();
            string splineString = string.Join(", ", spline.Select(p => p.ToString()));
            string expectedSplineString = "{X=0, Y=0}, {X=43, Y=171.29}, {X=43, Y=163.45},"
                + " {X=43, Y=154.26}, {X=43, Y=145.63}";
            Assert.AreEqual(expectedSplineString, splineString);

            GraphVizLabel nodeLabel = nodeA.GetLabel();
            Assert.AreEqual("{X=36.25977,Y=181.4415,Width=13.48047,Height=21.46484}", nodeLabel.BoundingBox().ToString());
            Assert.AreEqual("Times-Roman", nodeLabel.FontName().ToString());

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
    }
}
