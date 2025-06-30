using System;
using System.Linq;
using NUnit.Framework;

namespace Rubjerg.Graphviz.Test;

[TestFixture()]
public class CGraphBasicOperations
{
    [Test()]
    public void TestCopyAttributes()
    {
        RootGraph root = Utils.CreateUniqueTestGraph();
        Node n1 = root.GetOrAddNode("1");
        Node.IntroduceAttribute(root, "test", "foo");
        Assert.AreEqual("foo", n1.GetAttribute("test"));
        n1.SetAttribute("test", "bar");
        Assert.AreEqual("bar", n1.GetAttribute("test"));

        RootGraph root2 = Utils.CreateUniqueTestGraph();
        Node n2 = root2.GetOrAddNode("2");
        Assert.AreEqual(null, n2.GetAttribute("test"));
        Assert.AreEqual(0, n1.CopyAttributesTo(n2));
        Assert.AreEqual("bar", n2.GetAttribute("test"));
    }

    [Test()]
    public void TestDeletions()
    {
        RootGraph root = Utils.CreateUniqueTestGraph();

        Node tail = root.GetOrAddNode("1");
        Node head = root.GetOrAddNode("2");
        Node other = root.GetOrAddNode("3");

        Edge edge = root.GetOrAddEdge(tail, head, "edge");
        Edge tailout = root.GetOrAddEdge(tail, other, "tailout");
        Edge headout = root.GetOrAddEdge(head, other, "headout");
        Edge tailin = root.GetOrAddEdge(other, tail, "tailin");
        Edge headin = root.GetOrAddEdge(other, head, "headin");

        Assert.IsTrue(root.Equals(root.MyRootGraph));
        Assert.IsTrue(root.Equals(tail.MyRootGraph));
        Assert.IsTrue(root.Equals(edge.MyRootGraph));

        Assert.AreEqual(3, tail.TotalDegree());
        Assert.AreEqual(3, head.TotalDegree());
        Assert.AreEqual(3, root.Nodes().Count());

        root.Delete(edge);

        Assert.AreEqual(2, tail.TotalDegree());
        Assert.AreEqual(2, head.TotalDegree());
        Assert.AreEqual(3, root.Nodes().Count());

        root.Delete(tail);

        Assert.AreEqual(2, root.Nodes().Count());
        Assert.AreEqual(2, other.TotalDegree());
    }

    [Test()]
    public void TestNodeMerge()
    {
        RootGraph root = Utils.CreateUniqueTestGraph();
        Node merge = root.GetOrAddNode("merge");
        Node target = root.GetOrAddNode("target");
        Node other = root.GetOrAddNode("other");

        Edge selfloop = root.GetOrAddEdge(merge, merge, "selfloop");
        Edge contracted = root.GetOrAddEdge(merge, target, "contracted");
        Edge counter = root.GetOrAddEdge(target, merge, "counter");
        Edge mergeout = root.GetOrAddEdge(merge, other, "mergeout");
        Edge targetout = root.GetOrAddEdge(target, other, "targetout");
        Edge mergein = root.GetOrAddEdge(other, merge, "mergein");
        Edge targetin = root.GetOrAddEdge(other, target, "targetin");

        Assert.AreEqual(6, merge.TotalDegree());
        Assert.AreEqual(4, target.TotalDegree());
        Assert.AreEqual(3, root.Nodes().Count());

        //root.ComputeDotLayout();
        //root.ToSvgFile("dump1.svg");
        //root.FreeLayout();
        //root.ToDotFile("dump1.dot");

        root.Merge(merge, target);

        //root.ComputeDotLayout();
        //root.ToSvgFile("dump2.svg");
        //root.FreeLayout();
        //root.ToDotFile("dump2.dot");

        Assert.AreEqual(2, root.Nodes().Count());
        Assert.AreEqual(3, target.InDegree());
        Assert.AreEqual(3, target.OutDegree());
        Assert.AreEqual(2, other.InDegree());
        Assert.AreEqual(2, other.OutDegree());
    }

    [Test()]
    public void TestEdgeContraction()
    {
        RootGraph root = Utils.CreateUniqueTestGraph();
        Node tail = root.GetOrAddNode("x");
        Node head = root.GetOrAddNode("xx");
        Node other = root.GetOrAddNode("xxx");

        Edge contracted = root.GetOrAddEdge(tail, head, "tocontract");
        Edge parallel = root.GetOrAddEdge(tail, head, "parallel");
        Edge counterparallel = root.GetOrAddEdge(head, tail, "counterparallel");
        Edge tailout = root.GetOrAddEdge(tail, other, "tailout");
        Edge headout = root.GetOrAddEdge(head, other, "headout");
        Edge tailin = root.GetOrAddEdge(other, tail, "tailin");
        Edge headin = root.GetOrAddEdge(other, head, "headin");

        foreach (Node n in root.Nodes())
        {
            n.SafeSetAttribute("label", n.GetName(), "no");
            n.SafeSetAttribute("fontname", "Arial", "Arial");
            foreach (Edge e in n.EdgesOut())
            {
                e.SafeSetAttribute("label", e.GetName(), "no");
                e.SafeSetAttribute("fontname", "Arial", "Arial");
            }
        }


        Assert.AreEqual(5, tail.TotalDegree());
        Assert.AreEqual(5, head.TotalDegree());
        Assert.AreEqual(3, root.Nodes().Count());

        Node contraction = root.Contract(contracted, "contraction result");

        foreach (Node n in root.Nodes())
        {
            n.SafeSetAttribute("label", n.GetName(), "no");
            n.SafeSetAttribute("fontname", "Arial", "Arial");
            foreach (Edge e in n.EdgesOut())
            {
                e.SafeSetAttribute("label", e.GetName(), "no");
                e.SafeSetAttribute("fontname", "Arial", "Arial");
            }
        }

        //Console.Read();
        Assert.AreEqual(2, root.Nodes().Count());
        Assert.AreEqual(2, contraction.InDegree());
        Assert.AreEqual(2, contraction.OutDegree());
        Assert.AreEqual(2, other.InDegree());
        Assert.AreEqual(2, other.OutDegree());
    }

}
