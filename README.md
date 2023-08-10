Graphviz.NetWrapper
===================

[![codecov](https://codecov.io/gh/Rubjerg/Graphviz.NetWrapper/branch/master/graph/badge.svg)](https://codecov.io/gh/Rubjerg/Graphviz.NetWrapper)

## Supported platforms

At the moment, `Rubjerg.Graphviz` ships with a bunch of precompiled Graphviz dlls built for 64 bit Windows.
This library is compatible with .NET Standard 2.0.
The unit tests run against .NET Framework 4.8 and .NET 6.0.
In the future support may be extended to other platforms.

## Contributing

This project aims to provide a thin .NET shell around the Graphviz C libraries,
together with some convenience functionality that helps abstracting away some
of the peculiarities of the Graphviz library and make it easier to integrate in
an application.
Pull request that fall within the scope of this project are welcome.

## Installation

You can either add this library as a nuget package to project, or include the source and add a
project reference.

### Adding as a Nuget package

Add the [Rubjerg.Graphviz nuget package](https://www.nuget.org/packages/Rubjerg.Graphviz/) to
your project.

### Adding the Rubjerg.Graphviz code to your project or solution
1. Make this code available to your own code, e.g. by adding this repository as a git submodule to your own repository.
2. Add the projects Rubjerg.Graphviz and GraphvizWrapper to your solution.
3. To use Rubjerg.Graphviz within a project of yours, simply add a project reference to it.

When building your project, you should now see all the Graphviz binaries show up in your output
folder.  If you don't, try setting the `CopyLocalLockFileAssemblies` property in your referencing
project file to `true`.  If that still fails, try reordering the projects in your solution, such
that GraphvizWrapper and Rubjerg.Graphviz are at the top. 
There is an [outstanding issue for this](https://github.com/Rubjerg/Graphviz.NetWrapper/issues/36).

## Documentation

For a reference of attributes to instruct Graphviz have a look at
[Node, Edge and Graph Attributes](https://graphviz.gitlab.io/_pages/doc/info/attrs.html).
For more information on the inner workings of the graphviz libraries, consult the various
documents presented at the [Graphviz documentation page](https://graphviz.org/documentation/).

## Tutorial

```cs 
using NUnit.Framework;
using System.Linq;

namespace Rubjerg.Graphviz.Test;

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

        // A word of advice, Graphviz doesn't play very well with empty strings.
        // Try to avoid them when possible. (https://gitlab.com/graphviz/graphviz/-/issues/1887)
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
        PointD position = nodeA.GetPosition();
        Utils.AssertPattern(PointPattern, position.ToString());

        RectangleD nodeboundingbox = nodeA.GetBoundingBox();
        Utils.AssertPattern(RectPattern, nodeboundingbox.ToString());

        // Or splines between nodes
        Node nodeB = layout.GetNode("B");
        Edge edge = layout.GetEdge(nodeA, nodeB, "Some edge name");
        PointD[] spline = edge.GetFirstSpline();
        string splineString = string.Join(", ", spline.Select(p => p.ToString()));
        Utils.AssertPattern(SplinePattern, splineString);

        // If we require detailed drawing information for any object, we can retrieve the so called "xdot"
        // operations. See https://graphviz.org/docs/outputs/canon/#xdot for a specification.
        var activeFillColor = System.Drawing.Color.Black;
        foreach (var op in nodeA.GetDrawing())
        {
            if (op is XDotOp.FillColor { Value: Color.Uniform { HtmlColor: var htmlColor } })
            {
                activeFillColor = System.Drawing.ColorTranslator.FromHtml(htmlColor);
            }
            else if (op is XDotOp.FilledEllipse { Value: var boundingBox })
            {
                Utils.AssertPattern(RectPattern, boundingBox.ToString());
            }
            // Handle any xdot operation you require
        }

        foreach (var op in nodeA.GetLabelDrawing())
        {
            if (op is XDotOp.Text { Value: var text })
            {
                Utils.AssertPattern(PointPattern, text.Anchor.ToString());
                var boundingBox = text.TextBoundingBoxEstimate();
                Utils.AssertPattern(RectPattern, boundingBox.ToString());
                Assert.AreEqual(text.Text, "A");
                Assert.AreEqual(text.Font.Name, "Times-Roman");
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
        RectangleD clusterbox = cluster.GetBoundingBox();
        RectangleD rootgraphbox = layout.GetBoundingBox();
        Utils.AssertPattern(RectPattern, clusterbox.ToString());
        Utils.AssertPattern(RectPattern, rootgraphbox.ToString());
    }

    [Test, Order(4)]
    public void Records()
    {
        RootGraph root = RootGraph.CreateNew(GraphType.Directed, "Graph with records");
        Node nodeA = root.GetOrAddNode("A");
        nodeA.SetAttribute("shape", "record");
        // New line characters are not supported by record labels, and will be ignored by Graphviz
        nodeA.SetAttribute("label", "1|2|3|{4|5}|6|{7|8|9}");

        var layout = root.CreateLayout();

        // The order of the list matches the order in which the labels occur in the label string above.
        var rects = layout.GetNode("A").GetRecordRectangles().ToList();
        var rectLabels = layout.GetNode("A").GetRecordRectangleLabels().Select(l => l.Text).ToList();
        Assert.AreEqual(9, rects.Count);
        Assert.AreEqual(new[] { "1", "2", "3", "4", "5", "6", "7", "8", "9" }, rectLabels);
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
        nodeB.SetAttribute("shape", "record");
        nodeB.SetAttribute("label", $"<{validPortName}>1|2");

        // The conversion function makes sure different strings don't accidentally map onto the same portname
        Assert.AreNotEqual(Edge.ConvertUidToPortName(":"), Edge.ConvertUidToPortName("|"));
    }
}
```
