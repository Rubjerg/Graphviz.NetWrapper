using NUnit.Framework;
using System.IO;
using System;
using static Rubjerg.Graphviz.FFI.GraphvizFFI;
using static Rubjerg.Graphviz.FFI.TestLib;

namespace Rubjerg.Graphviz.Test;

[TestFixture()]
public class TestInterop
{
    [Test()]
    public void TestAgclose()
    {
        IntPtr root = Rjagopen("test", 0);
        _ = Agnode(root, "node1", 1);
        Agclose(root);
    }

    [Test()]
    public void TestMarshaling()
    {
        Assert.True(echobool(true));
        Assert.False(echobool(false));
        Assert.True(return_true());
        Assert.False(return_false());

        Assert.AreEqual(0, echoint(0));
        Assert.AreEqual(1, echoint(1));
        Assert.AreEqual(-1, echoint(-1));
        Assert.AreEqual(1, return1());
        Assert.AreEqual(-1, return_1());

        Assert.AreEqual(TestEnum.Val1, return_enum1());
        Assert.AreEqual(TestEnum.Val2, return_enum2());
        Assert.AreEqual(TestEnum.Val5, return_enum5());
        Assert.AreEqual(TestEnum.Val1, echo_enum(TestEnum.Val1));
        Assert.AreEqual(TestEnum.Val2, echo_enum(TestEnum.Val2));
        Assert.AreEqual(TestEnum.Val5, echo_enum(TestEnum.Val5));

        Assert.AreEqual("", ReturnEmptyString());
        Assert.AreEqual("hello", ReturnHello());
        Assert.AreEqual("©", ReturnCopyRight());
        Assert.AreEqual("1", EchoString("1"));
        Assert.AreEqual("", EchoString(""));
        Assert.AreEqual("hello", EchoString("hello"));
        Assert.AreEqual("©", EchoString("©"));
    }

    [Test()]
    public void TestInternalEncoding()
    {
        var root = Utils.CreateUniqueTestGraph();
        var nodeA = root.GetOrAddNode("✅");
        var nodeB = root.GetOrAddNode("✅");
        Assert.AreEqual(nodeA, nodeB);
    }

    [Test()]
    public void TestOutputEncoding()
    {
        var root = Utils.CreateUniqueTestGraph();
        var nodeA = root.GetOrAddNode("A");
        nodeA.SetAttribute("label", "✅");
        var dotStr = root.ToDotString();
        Assert.IsTrue(dotStr.Contains("✅"));
        var svgStr = root.ToSvgString();
        Assert.IsTrue(svgStr.Contains("✅"));

        // Also test with files
        root.ToDotFile(TestContext.CurrentContext.TestDirectory + "/utf8.dot");
        var dotFileStr = File.ReadAllText(TestContext.CurrentContext.TestDirectory + "/utf8.dot");
        Assert.IsTrue(dotFileStr.Contains("✅"));
        root.ToXDotFile(TestContext.CurrentContext.TestDirectory + "/utf8.xdot");
        var xdotFileStr = File.ReadAllText(TestContext.CurrentContext.TestDirectory + "/utf8.xdot");
        Assert.IsTrue(xdotFileStr.Contains("✅"));
        root.ToSvgFile(TestContext.CurrentContext.TestDirectory + "/utf8.svg");
        var svgFileStr = File.ReadAllText(TestContext.CurrentContext.TestDirectory + "/utf8.svg");
        Assert.IsTrue(svgFileStr.Contains("✅"));

        // PDF doesn't seem to support unicode correctly?
        // Open issue: https://gitlab.com/graphviz/graphviz/-/issues/2508
    }
}
