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
            SubGraph cluster1 = root.GetOrAddSubgraph("cluster_1");
            cluster1.AddExisting(nodeB);
            cluster1.AddExisting(nodeC);
            SubGraph cluster2 = root.GetOrAddSubgraph("cluster_2");
            cluster2.AddExisting(nodeD);

            // We can attach attributes to nodes, edges and graphs to store information and instruct
            // graphviz by specifying layout parameters. At the moment we only support string
            // attributes. Cgraph assumes that all objects of a given kind (graphs/subgraphs, nodes,
            // or edges) have the same attributes. The attributes first have to be introduced for a
            // certain kind, before we can use it.
            Node.IntroduceAttribute(root, "my attribute", "defaultvalue");
            nodeA.SetAttribute("my attribute", "othervalue");

            // To introduce and set an attribute at the same time, there are convenience wrappers
            edgeAB.SafeSetAttribute("color", "red", "black");
            edgeBC.SafeSetAttribute("arrowsize", "2.0", "1.0");

            // Graphviz does not really support edges from and to clusters. However, by adding an
            // invisible dummynode and setting the ltail or lhead attributes of an edge this
            // behavior can be faked. Graphviz will then draw an edge to the dummy node but clip it
            // at the border of the cluster. We provide convenience methods for this.
            // To enable this feature, Graphviz requires us to set the "compound" attribute to "true".
            Graph.IntroduceAttribute(root, "compound", "true"); // Allow lhead/ltail
            // The boolean indicates whether the dummy node should take up any space. When you pass
            // false and you have a lot of edges, the edges may start to overlap a lot.
            root.GetOrAddEdge(nodeA, cluster1, false, "edge to a cluster");
            root.GetOrAddEdge(cluster1, nodeD, false, "edge from a cluster");
            root.GetOrAddEdge(cluster1, cluster2, false, "edge between clusters");

            nodeA.SafeSetAttribute("shape", "record", "");
            nodeA.SafeSetAttribute("label", "1 | 2 | {3|4}", "\\N");

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

            SubGraph cluster = root.GetSubgraph("cluster_1");
            RectangleF clusterbox = cluster.BoundingBox();
            RectangleF rootgraphbox = root.BoundingBox();
            Utils.AssertPattern(@"{X=[\d.]+,Y=[\d.]+,Width=[\d.]+,Height=[\d.]+}", clusterbox.ToString());
            Utils.AssertPattern(@"{X=[\d.]+,Y=[\d.]+,Width=[\d.]+,Height=[\d.]+}", rootgraphbox.ToString());

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
