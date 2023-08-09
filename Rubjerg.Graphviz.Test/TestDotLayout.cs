using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using NUnit.Framework;
using static Rubjerg.Graphviz.Test.Utils;

namespace Rubjerg.Graphviz.Test;

[TestFixture()]
public class TestDotLayout
{
    private static void CreateSimpleTestGraph(out RootGraph root, out Node nodeA, out Edge edge)
    {
        root = CreateUniqueTestGraph();
        root.SetAttribute("label", "g");
        nodeA = root.GetOrAddNode("A");
        nodeA.SetAttribute("shape", "record");
        nodeA.SetAttribute("label", "{a|b}");
        nodeA.SetAttribute("color", "red");
        Node nodeB = root.GetOrAddNode("B");
        edge = root.GetOrAddEdge(nodeA, nodeB, "");
        edge.SetAttribute("label", "e");
        edge.SetAttribute("headlabel", "h");
        edge.SetAttribute("taillabel", "t");
        edge.SetAttribute("dir", "both");
        edge.SetAttribute("arrowtail", "vee");
        edge.SetAttribute("arrowhead", "vee");
    }

    [Test()]
    public void TestLayoutMethodsWithoutLayout()
    {
        CreateSimpleTestGraph(out RootGraph root, out Node nodeA, out Edge edge);

        Assert.AreEqual(root.GetBoundingBox(), default(RectangleF));
        Assert.AreEqual(root.GetColor(), Color.Black);
        Assert.AreEqual(root.GetDrawing().Count, 0);
        Assert.AreEqual(root.GetLabelDrawing().Count, 0);

        Assert.AreEqual(nodeA.GetPosition(), default(PointF));
        Assert.AreEqual(nodeA.GetBoundingBox(), default(RectangleF));
        Assert.AreEqual(nodeA.GetSize(), default(SizeF));
        Assert.AreEqual(nodeA.GetRecordRectangles().Count(), 0);
        Assert.AreEqual(nodeA.GetDrawing().Count, 0);
        Assert.AreEqual(nodeA.GetLabelDrawing().Count, 0);

        Assert.AreEqual(edge.GetFirstSpline(), null);
        Assert.AreEqual(edge.GetSplines().Count(), 0);
        Assert.AreEqual(edge.GetDrawing().Count, 0);
        Assert.AreEqual(edge.GetLabelDrawing().Count, 0);
        Assert.AreEqual(edge.GetHeadArrowDrawing().Count, 0);
        Assert.AreEqual(edge.GetTailArrowDrawing().Count, 0);
        Assert.AreEqual(edge.GetHeadLabelDrawing().Count, 0);
        Assert.AreEqual(edge.GetTailLabelDrawing().Count, 0);

        //root.ToSvgFile("xxx.svg");
    }

    [Test()]
    public void TestLayoutMethodsWithInProcessLayout()
    {
        CreateSimpleTestGraph(out RootGraph root, out Node nodeA, out Edge edge);

        root.ComputeLayout();

        Assert.AreEqual(root.GetColor(), Color.Black);
        Assert.AreNotEqual(root.GetBoundingBox(), default(RectangleF));
        Assert.AreNotEqual(root.GetDrawing().Count, 0);
        Assert.AreNotEqual(root.GetLabelDrawing().Count, 0);

        Assert.AreEqual(nodeA.GetColor(), Color.Red);
        Assert.AreEqual(nodeA.GetRecordRectangles().Count(), 2);
        Assert.AreNotEqual(nodeA.GetPosition(), default(PointF));
        Assert.AreNotEqual(nodeA.GetBoundingBox(), default(RectangleF));
        Assert.AreNotEqual(nodeA.GetSize(), default(SizeF));
        Assert.AreNotEqual(nodeA.GetDrawing().Count, 0);
        Assert.AreNotEqual(nodeA.GetLabelDrawing().Count, 0);

        Assert.AreNotEqual(edge.GetFirstSpline(), null);
        Assert.AreNotEqual(edge.GetSplines().Count(), 0);
        Assert.AreNotEqual(edge.GetDrawing().Count, 0);
        Assert.AreNotEqual(edge.GetLabelDrawing().Count, 0);
        Assert.AreNotEqual(edge.GetHeadArrowDrawing().Count, 0);
        Assert.AreNotEqual(edge.GetTailArrowDrawing().Count, 0);
        Assert.AreNotEqual(edge.GetHeadLabelDrawing().Count, 0);
        Assert.AreNotEqual(edge.GetTailLabelDrawing().Count, 0);
    }

    [Test()]
    public void TestLayoutMethodsWithLayout()
    {
        CreateSimpleTestGraph(out RootGraph root, out Node nodeA, out Edge edge);

        var xroot = root.CreateLayout();
        var xnodeA = xroot.GetNode("A");
        var xnodeB = xroot.GetNode("B");
        Edge xedge = xroot.GetEdge(xnodeA, xnodeB, "");

        Assert.AreEqual(xroot.GetColor(), Color.Black);
        Assert.AreNotEqual(xroot.GetBoundingBox(), default(RectangleF));
        Assert.AreNotEqual(xroot.GetDrawing().Count, 0);
        Assert.AreNotEqual(xroot.GetLabelDrawing().Count, 0);

        Assert.AreEqual(xnodeA.GetColor(), Color.Red);
        Assert.AreEqual(xnodeA.GetRecordRectangles().Count(), 2);
        Assert.AreNotEqual(xnodeA.GetPosition(), default(PointF));
        Assert.AreNotEqual(xnodeA.GetBoundingBox(), default(RectangleF));
        Assert.AreNotEqual(xnodeA.GetSize(), default(SizeF));
        Assert.AreNotEqual(xnodeA.GetDrawing().Count, 0);
        Assert.AreNotEqual(xnodeA.GetLabelDrawing().Count, 0);

        Assert.AreNotEqual(xedge.GetFirstSpline(), null);
        Assert.AreNotEqual(xedge.GetSplines().Count(), 0);
        Assert.AreNotEqual(xedge.GetDrawing().Count, 0);
        Assert.AreNotEqual(xedge.GetLabelDrawing().Count, 0);
        Assert.AreNotEqual(xedge.GetHeadArrowDrawing().Count, 0);
        Assert.AreNotEqual(xedge.GetTailArrowDrawing().Count, 0);
        Assert.AreNotEqual(xedge.GetHeadLabelDrawing().Count, 0);
        Assert.AreNotEqual(xedge.GetTailLabelDrawing().Count, 0);
    }

    [Test()]
    public void TestHtmlLabels()
    {
        RootGraph root = CreateUniqueTestGraph();
        const string labelKey = "label";
        Node.IntroduceAttribute(root, labelKey, "");
        Graph.IntroduceAttribute(root, labelKey, "");

        Node n1 = root.GetOrAddNode("1");
        Node n2 = root.GetOrAddNode("2");
        root.SetAttributeHtml(labelKey, "<html 3>");

        n1.SetAttribute(labelKey, "plain 1");
        n2.SetAttributeHtml(labelKey, "<html 2>");

        var result = root.ToDotString();

        Assert.That(result, Does.Contain("\"plain 1\""));
        AssertContainsHtml(result, "<html 2>");
        AssertContainsHtml(result, "<html 3>");
    }

    [Test()]
    public void TestHtmlLabelsDefault()
    {
        RootGraph root = CreateUniqueTestGraph();
        const string labelKey = "label";
        Node.IntroduceAttributeHtml(root, labelKey, "<html default>");

        Node n1 = root.GetOrAddNode("1");
        Node n2 = root.GetOrAddNode("2");
        Node n3 = root.GetOrAddNode("3");

        n1.SetAttribute(labelKey, "plain 1");
        n2.SetAttributeHtml(labelKey, "<html 2>");

        var result = root.ToDotString();

        Assert.That(result, Does.Contain("\"plain 1\""));
        AssertContainsHtml(result, "<html 2>");
        AssertContainsHtml(result, "<html default>");
    }

    private static void AssertContainsHtml(string result, string html)
    {
        // Html labels are not string quoted in dot file
        Assert.That(result, Does.Not.Contain($"\"{html}\""));
        Assert.That(result, Does.Not.Contain($"\"<{html}>\""));
        // Htmls labels have additional angel bracket delimeters added
        Assert.That(result, Does.Contain($"<{html}>"));
    }

    [Test()]
    public void TestRecordShapeOrder()
    {
        RootGraph root = CreateUniqueTestGraph();
        Node nodeA = root.GetOrAddNode("A");

        nodeA.SetAttribute("shape", "record");
        nodeA.SetAttribute("label", "1|2|3|{4|5}|6|{7|8|9}");

        root.ComputeLayout();

        var rects = nodeA.GetRecordRectangles().ToList();

        // Because Graphviz uses a lower-left originated coordinate system, we need to flip the y coordinates
        Utils.AssertOrder(rects, r => (r.Left, -r.Top));
        Assert.That(rects.Count, Is.EqualTo(9));
    }

    [Test()]
    public void TestEmptyRecordShapes()
    {
        RootGraph root = CreateUniqueTestGraph();
        Node nodeA = root.GetOrAddNode("A");
        nodeA.SetAttribute("shape", "record");
        nodeA.SetAttribute("label", "||||");

        root.ComputeLayout();

        var rects = nodeA.GetRecordRectangles().ToList();
        Assert.That(rects.Count, Is.EqualTo(5));
        root.ToSvgFile(GetTestFilePath("out.svg"));
    }

    [Test()]
    [TestCase(true)]
    [TestCase(false)]
    public void TestPortNameConversion(bool escape)
    {
        string port1 = ">|<";
        string port2 = "B";
        if (escape)
        {
            port1 = Edge.ConvertUidToPortName(port1);
            port2 = Edge.ConvertUidToPortName(port2);
        }
        string label = $"{{<{port1}>1|<{port2}>2}}";

        {
            RootGraph root = CreateUniqueTestGraph();
            Node node = root.GetOrAddNode("N");
            node.SetAttribute("shape", "record");
            node.SetAttribute("label", label);
            Edge edge = root.GetOrAddEdge(node, node, "");
            edge.SetAttribute("tailport", port1 + ":n");
            edge.SetAttribute("headport", port2 + ":s");
            root.ToDotFile(GetTestFilePath("out.gv"));
        }

        {
            var root = RootGraph.FromDotFile(GetTestFilePath("out.gv"));

            Node node = root.GetNode("N");
            Assert.That(node.GetAttribute("label"), Is.EqualTo(label));
            Edge edge = root.Edges().First();
            Assert.That(edge.GetAttribute("tailport"), Is.EqualTo(port1 + ":n"));
            Assert.That(edge.GetAttribute("headport"), Is.EqualTo(port2 + ":s"));

            root.ComputeLayout();
            root.ToSvgFile(GetTestFilePath("out.svg"));
            root.ToDotFile(GetTestFilePath("out.dot"));

            var rects = node.GetRecordRectangles();
            if (escape)
                Assert.That(rects.Count, Is.EqualTo(2));
            else
                Assert.That(rects.Count, Is.EqualTo(3));
        }
    }

    [Test()]
    [TestCase(true)]
    [TestCase(false)]
    public void TestLabelEscaping(bool escape)
    {
        string label1 = "|";
        string label2 = @"\N\n\L";
        string label3 = "3";
        if (escape)
        {
            label1 = CGraphThing.EscapeLabel(label1);
            label2 = CGraphThing.EscapeLabel(label2);
        }

        {
            RootGraph root = CreateUniqueTestGraph();
            Node node1 = root.GetOrAddNode("1");
            node1.SetAttribute("shape", "record");
            node1.SetAttribute("label", label1);
            Node node2 = root.GetOrAddNode("2");
            node2.SetAttribute("label", label2);
            Node node3 = root.GetOrAddNode("3");
            node3.SetAttribute("label", label3);
            root.ToDotFile(GetTestFilePath("out.gv"));
        }

        {
            var root = RootGraph.FromDotFile(GetTestFilePath("out.gv"));

            Node node1 = root.GetNode("1");
            Assert.That(node1.GetAttribute("label"), Is.EqualTo(label1));
            Node node2 = root.GetNode("2");
            Assert.That(node2.GetAttribute("label"), Is.EqualTo(label2));
            Node node3 = root.GetNode("3");
            Assert.That(node3.GetAttribute("label"), Is.EqualTo(label3));

            root.ComputeLayout();
            root.ToSvgFile(GetTestFilePath("out.svg"));
            root.ToDotFile(GetTestFilePath("out.dot"));

            var rects = node1.GetRecordRectangles();
            if (escape)
            {
                Assert.That(rects.Count, Is.EqualTo(1));
                Assert.That(node2.GetBoundingBox().Height, Is.EqualTo(node3.GetBoundingBox().Height));
            }
            else
            {
                Assert.That(rects.Count, Is.EqualTo(2));
                Assert.That(node2.GetBoundingBox().Height, Is.Not.EqualTo(node3.GetBoundingBox().Height));
            }
        }
    }
}
