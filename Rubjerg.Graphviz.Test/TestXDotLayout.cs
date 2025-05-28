using System;
using System.Linq;
using NUnit.Framework;

namespace Rubjerg.Graphviz.Test;

using FFI;

[TestFixture()]
public class TestXDotLayout
{
    [Test()]
    public void TestXDotTranslateFromString()
    {
        // Test case containing all possible operations
        var testcase = @"
E 10 10 5 3
e 20 10 5 3
P 4 30 10 30 15 35 15 35 10
p 4 40 10 40 15 45 15 45 10
L 4 50 10 50 15 55 15 55 10
B 4 60 10 60 15 65 15 65 10
b 4 70 10 70 15 75 15 75 10
T 80 10 0 5 12 -Hello world
t 31
C 7 -#ff0000
c 7 -#00ff00
F 12 5 -Arial
S 6 -dashed
I 90 10 5 5 8 -image.png
";
        var result = XDotParser.ParseXDot(testcase, CoordinateSystem.BottomLeft, 0);
        Assert.AreEqual(14, result.Count);

    }

    [Test()]
    public void TestXDotRecordNode()
    {
        RootGraph root = Utils.CreateUniqueTestGraph();
        Node nodeA = root.GetOrAddNode("A");

        nodeA.SetAttribute("shape", "record");
        // New lines in record labels are ignored by Graphviz
        nodeA.SetAttribute("label", "1|{2\n3}");

        var xdotGraph = root.CreateLayout();
        var xNodeA = xdotGraph.GetNode("A");
        var ldraw = xNodeA.GetLabelDrawing();
        Assert.IsTrue(ldraw.OfType<XDotOp.Text>().Any(t => t.Value.Text == "23"));
        // Even though the attribute still contains the newline
        Assert.IsTrue(xNodeA.GetAttribute("label") == "1|{2\n3}");
        Assert.AreEqual(6, ldraw.Count);
    }

    [Test()]
    public void TestXDotNewLines()
    {
        RootGraph root = Utils.CreateUniqueTestGraph();
        SubGraph cluster = root.GetOrAddSubgraph("cluster_1");
        cluster.SetAttribute("label", "1\n2");
        Node nodeA = cluster.GetOrAddNode("A");
        nodeA.SetAttribute("label", "a\nb");

        var xdotGraph = root.CreateLayout();

        // New lines result in separate text operations
        var xCluster = xdotGraph.GetSubgraph("cluster_1");
        var ldraw = xCluster.GetLabelDrawing();
        Assert.AreEqual(6, ldraw.Count);

        var xNodeA = xdotGraph.GetNode("A");
        ldraw = xNodeA.GetLabelDrawing();
        Assert.AreEqual(6, ldraw.Count);
    }

    [Test()]
    public void TestRecordShapeOrder()
    {
        RootGraph root = Utils.CreateUniqueTestGraph();
        Node nodeA = root.GetOrAddNode("A");

        nodeA.SetAttribute("shape", "record");
        nodeA.SetAttribute("label", "1|2|3|{4|5}|6|{7|8|9}");


        var xdotGraph = root.CreateLayout(coordinateSystem: CoordinateSystem.TopLeft);

        var xNodeA = xdotGraph.GetNode("A");
        var rects = xNodeA.GetRecordRectangles().ToList();

        Utils.AssertOrder(rects, r => (r.Origin.X, r.Origin.Y));
        Assert.That(rects.Count, Is.EqualTo(9));

        // Test xdot translation
        var xdotDraw = xdotGraph.GetDrawing();
    }

    [Test()]
    public void TestEmptyRecordShapes()
    {
        RootGraph root = Utils.CreateUniqueTestGraph();
        Node nodeA = root.GetOrAddNode("A");
        nodeA.SetAttribute("shape", "record");
        nodeA.SetAttribute("label", "||||");

        var xdotGraph = root.CreateLayout();

        var xNodeA = xdotGraph.GetNode("A");
        var rects = xNodeA.GetRecordRectangles().ToList();
        Assert.That(rects.Count, Is.EqualTo(5));
    }

    [Test()]
    public void TestCoordinateTransformation()
    {
        RootGraph root = Utils.CreateUniqueTestGraph();
        Node nodeA = root.GetOrAddNode("A");
        var xdotGraph = root.CreateLayout(coordinateSystem: CoordinateSystem.TopLeft);
        // Check that translating back gets us the old bounding box
        var translatedBack = xdotGraph.GetBoundingBox().ForCoordSystem(CoordinateSystem.BottomLeft, xdotGraph.RawMaxY());
        Assert.AreEqual(translatedBack, xdotGraph.RawBoundingBox());
    }
}
