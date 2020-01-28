using NUnit.Framework;
using System.Drawing;
using System.Linq;

namespace Rubjerg.Graphviz.Test
{
    [TestFixture()]
    public class Tutorial
    {
        [Test()]
        public void RunTutorial()
        {
            {
                // You can programmatically construct graphs as follows
                RootGraph root = RootGraph.CreateNew("Some Unique Identifier", GraphType.Directed);
                // The node names are unique identifiers within a graph in Graphviz
                Node node1 = root.GetOrAddNode("Unique node name 1");
                Node node2 = root.GetOrAddNode("Unique node name 2");
                Node node3 = root.GetOrAddNode("Unique node name 3");
                Node node4 = root.GetOrAddNode("Unique node name 4");
                // The edge name is only unique between two nodes
                Edge edge1 = root.GetOrAddEdge(node1, node2, "Edge name");
                Edge edge2 = root.GetOrAddEdge(node2, node3, "Edge name");

                // We can attach attributes to nodes, edges and graphs to store information and instruct graphviz
                // by specifying layout parameters. At the moment we only support string attributes.
                // Cgraph assumes that all objects of a given kind(graphs/subgraphs, nodes, or edges) have the same
                // attributes - there’s no notion of subtyping within attributes.
                // The attributes first have to be introduced for a certain kind, before we can use it.
                Node.IntroduceAttribute(root, "my attribute", "defaultvalue");
                node1.SetAttribute("my attribute", "othervalue");

                // To introduce and set an attribute at the same time, there are convenience wrappers
                edge1.SafeSetAttribute("color", "red", "black");

                // We can simply export this graph to a text file in dot format
                root.ToDotFile(TestContext.CurrentContext.TestDirectory + "/out.dot");
            }

            {
                // If we have a given dot file, we can also simply read it back in
                RootGraph root = RootGraph.FromDotFile(TestContext.CurrentContext.TestDirectory + "/out.dot");

                // Let's have graphviz compute a dot layout for us
                root.ComputeLayout();
                // TODO: list other supported engines

                // We can export this to svg
                root.ToSvgFile(TestContext.CurrentContext.TestDirectory + "/out.svg");

                // Or programatically read out the layout attributes
                Node node1 = root.GetNode("Unique node name 1");
                Node node2 = root.GetNode("Unique node name 2");
                PointF pos1 = node1.Position();
                TestContext.WriteLine(pos1.ToString());
                RectangleF nodeboundingbox = node1.BoundingBox();
                TestContext.WriteLine(nodeboundingbox.ToString());
                Edge edge = root.GetEdge(node1, node2, "Edge name");
                PointF[] spline1 = edge.FirstSpline();
                TestContext.WriteLine(string.Join(", ", spline1.Select(p => p.ToString())));

                // Once all layout information is obtained from the graph, the resources should be reclaimed. To do this, the
                // application should call the cleanup routine associated with the layout algorithm used to draw the graph.
                // This is done by a call to FreeLayout()
                root.FreeLayout();
                // A given graph can be laid out multiple times. The application, however, must clean up the earlier
                // layout's information with a call to gvFreeLayout before invoking a new layout function
            }



        }
    }
}
