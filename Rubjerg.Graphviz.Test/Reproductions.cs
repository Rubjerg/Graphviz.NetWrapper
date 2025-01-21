using NUnit.Framework;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using static Rubjerg.Graphviz.ForeignFunctionInterface;

namespace Rubjerg.Graphviz.Test;

/// <summary>
/// Test various scenarios that have caused problems in the past.
/// </summary>
[TestFixture()]
public class Reproductions
{
    private string _testDir;

    [SetUp]
    public void SetUp()
    {
        // Store the test directory.
        _testDir = TestContext.CurrentContext.TestDirectory;
    }
    
    [Test()]
    public void TestCloseCrash()
    {
        RootGraph root = Utils.CreateUniqueTestGraph();
        
        string dot = "digraph test { A [label=1]; }";
        root = RootGraph.FromDotString(dot);
        root.ComputeLayout();
        root.Close();
        Utils.CreateUniqueTestGraph().Close();
    }
    
    [Test()]
    public void TestAgcloseCrash()
    {
        string dot = "digraph test { A [label=1]; }";
        var root = Rjagmemread(dot);
        int layout_rc = GvLayout(GVC, root, "dot");
        int render_rc = GvRender(GVC, root, "xdot", IntPtr.Zero);
        Agclose(root);
        root = Rjagopen("test 2", 0);
        Agclose(root);
    }

    [Test()]
    public void ExportPathWithSpaces()
    {
        RootGraph root = RootGraph.CreateNew(GraphType.Directed, "");
        _ = root.GetOrAddNode("A");
        root.ToDotFile(TestContext.CurrentContext.TestDirectory + "/name with spaces.dot");
        root.ToSvgFile(TestContext.CurrentContext.TestDirectory + "/name with spaces.svg");
    }

    [Test()]
    [TestCase("Times-Roman", 7, 0.01)]
    [TestCase("Times-Roman", 7, 0.5)]
    public void TestRecordShapeAlignment(string fontname, double fontsize, double margin)
    {
        RootGraph root = Utils.CreateUniqueTestGraph();
        // Margin between label and node boundary in inches
        Node.IntroduceAttribute(root, "margin", margin.ToString(CultureInfo.InvariantCulture));
        Node.IntroduceAttribute(root, "fontsize", fontsize.ToString(CultureInfo.InvariantCulture));
        Node.IntroduceAttribute(root, "fontname", fontname);

        Node nodeA = root.GetOrAddNode("A");

        nodeA.SetAttribute("shape", "record");
        nodeA.SetAttribute("label", "{20 VH|{1|2}}");

        //TestContext.Write(root.ToDotString());
        root.ComputeLayout();
        //TestContext.Write(root.ToDotString());

        // This test is fixed by passing snapOntoDrawingCoordinates: true
        var rects = nodeA.GetRecordRectangles(snapOntoDrawingCoordinates: true).ToList();
        Assert.That(rects[0].FarPoint().X, Is.EqualTo(rects[2].FarPoint().X));
    }

    // This test only failed when running in isolation
    [Test()]
    public void MissingLabelRepro()
    {
        var graph = RootGraph.FromDotFile($"{_testDir}/missing-label-repro.dot");
        graph.ComputeLayout();
        graph.ToSvgFile($"{_testDir}/test.svg");
        string svgString = File.ReadAllText($"{_testDir}/test.svg");
        Assert.IsTrue(svgString.Contains(">OpenNode</text>"));
    }

    [Test()]
    public void StackOverflowRepro()
    {
        var graph = RootGraph.FromDotFile($"{_testDir}/stackoverflow-repro.dot");
        graph.ComputeLayout();
    }

    [Test()]
    public void TestFromDotFile()
    {
        _ = RootGraph.FromDotFile($"{_testDir}/missing-label-repro.dot");
    }

    [Test()]
    public void TestDotNewlines()
    {
        RootGraph root = Utils.CreateUniqueTestGraph();
        Node nodeA = root.GetOrAddNode("A");

        nodeA.SetAttribute("shape", "record");
        nodeA.SetAttribute("label", "1|2|3|{4|5}|6|{7|8|9}");

        var dotString = root.ToDotString();
        Assert.IsFalse(dotString!.Contains("\r"));
    }


    [Test()]
    public void TestDotNewlines2()
    {
        RootGraph root = Utils.CreateUniqueTestGraph();
        Node nodeA = root.GetOrAddNode("A");

        nodeA.SetAttribute("shape", "record");
        nodeA.SetAttribute("label", "1|2|3|{4|5}|6|{7|8|9}");

        var xdotGraph = root.CreateLayout();
        var xNodeA = xdotGraph.GetNode("A");
        var ldraw = xNodeA!.GetAttribute("_ldraw_");
        Assert.IsFalse(ldraw!.Contains("\n"));
        Assert.IsFalse(ldraw.Contains("\r"));
        Assert.IsFalse(ldraw.Contains("\\"));
    }

    [Test()]
    public void TestDotNewlines3()
    {
        var dotstrCrLf = @"
digraph ""test graph 1"" {
    graph[_draw_ = ""c 9 -#fffffe00 C 7 -#ffffff P 4 0 0 0 72.25 136.5 72.25 136.5 0 "",
        bb = ""0,0,136.5,72.25"",
        xdotversion = 1.7
    ];
    node[label = ""\N""];
    A[_draw_ = ""c 7 -#000000 p 4 0 0.5 0 71.75 136.5 71.75 136.5 0.5 c 7 -#000000 L 2 22.75 0.5 22.75 71.75 c 7 -#000000 L 2 45.5 0.5 45.5 71.75 \
c 7 -#000000 L 2 68.25 0.5 68.25 71.75 c 7 -#000000 L 2 68.25 37 91 37 c 7 -#000000 L 2 91 0.5 91 71.75 c 7 -#000000 L 2 113.75 \
0.5 113.75 71.75 c 7 -#000000 L 2 113.75 48 136.5 48 c 7 -#000000 L 2 113.75 24.25 136.5 24.25 "",
_ldraw_ = ""F 14 11 -Times-Roman c 7 -#000000 T 11.38 30.7 0 6.75 1 -1 F 14 11 -Times-Roman c 7 -#000000 T 34.12 30.7 0 6.75 1 -2 F 14 11 -Times-Roman \
c 7 -#000000 T 56.88 30.7 0 6.75 1 -3 F 14 11 -Times-Roman c 7 -#000000 T 79.62 48.95 0 6.75 1 -4 F 14 11 -Times-Roman c 7 -#000000 \
T 79.62 13.7 0 6.75 1 -5 F 14 11 -Times-Roman c 7 -#000000 T 102.38 30.7 0 6.75 1 -6 F 14 11 -Times-Roman c 7 -#000000 T 125.12 \
54.45 0 6.75 1 -7 F 14 11 -Times-Roman c 7 -#000000 T 125.12 30.7 0 6.75 1 -8 F 14 11 -Times-Roman c 7 -#000000 T 125.12 6.95 0 \
6.75 1 -9 "",
height = 1.0035,
label = ""1|2|3|{4|5}|6|{7|8|9}"",
pos = ""68.25,36.125"",
rects = ""0,0.5,22.75,71.75 22.75,0.5,45.5,71.75 45.5,0.5,68.25,71.75 68.25,37,91,71.75 68.25,1.25,91,37 91,0.5,113.75,71.75 113.75,48,136.5,\
71.75 113.75,24.25,136.5,48 113.75,0.5,136.5,24.25"",
shape = record,
width = 1.8958];
}";
        var graph = RootGraph.FromDotString(dotstrCrLf);
        var nodeA = graph.GetNode("A");
        var ldraw = nodeA!.GetAttribute("_ldraw_");

        Assert.IsFalse(ldraw!.Contains("\n"));
        Assert.IsFalse(ldraw.Contains("\r"));
        Assert.IsFalse(ldraw.Contains("\\"));
    }

}
