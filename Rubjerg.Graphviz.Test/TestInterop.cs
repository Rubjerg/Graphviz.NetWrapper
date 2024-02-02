using NUnit.Framework;
using static Rubjerg.Graphviz.ForeignFunctionInterface;

namespace Rubjerg.Graphviz.Test;

[TestFixture()]
public class TestInterop
{
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
    public void TestOutputEncoding()
    {
        var root = Utils.CreateUniqueTestGraph();
        var nodeA = root.GetOrAddNode("A");
        nodeA.SetAttribute("label", "✅");
        var dotStr = root.ToDotString();
        Assert.IsTrue(dotStr.Contains("✅"));
        var svgStr = root.ToSvgString();
        Assert.IsTrue(svgStr.Contains("✅"));
        // FIXNOW: test with files
    }
}
