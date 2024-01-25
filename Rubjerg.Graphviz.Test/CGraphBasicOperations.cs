using System;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Rubjerg.Graphviz.Test
{
    [TestFixture()]
    public class CGraphBasicOperations
    {
        [Test()]
        public void TestReadDotFile()
        {
            RootGraph root = RootGraph.FromDotString(@"
digraph test {
    A;
    B;
    B -> B;
    A -> B[key = edgename];
    A -> B;
    A -> B;
}
");
            var edges = root.Edges().ToList();
            var names = edges.Select(e => e.GetName());
            // The attribute 'key' maps to the edgename
            ClassicAssert.IsTrue(names.Any(n => n == "edgename"));
            ClassicAssert.IsTrue(names.All(n => n == "edgename" || string.IsNullOrEmpty(n)));

            // However, it is strange that the other two edges both seem to have the same name, namely ""
            // According to the documentation, the name is used to distinguish between multi-edges
            var A = root.GetNode("A");
            var B = root.GetNode("B");
            ClassicAssert.AreEqual(3, A.EdgesOut().Count());

            // The documentation seem to be correct for edges that are added through the C interface
            _ = root.GetOrAddEdge(A, B, "");
            ClassicAssert.AreEqual(4, A.EdgesOut().Count());
            _ = root.GetOrAddEdge(A, B, "");
            ClassicAssert.AreEqual(4, A.EdgesOut().Count());
        }

        [Test()]
        public void TestCopyAttributes()
        {
            RootGraph root = Utils.CreateUniqueTestGraph();
            Node n1 = root.GetOrAddNode("1");
            Node.IntroduceAttribute(root, "test", "foo");
            ClassicAssert.AreEqual("foo", n1.GetAttribute("test"));
            n1.SetAttribute("test", "bar");
            ClassicAssert.AreEqual("bar", n1.GetAttribute("test"));

            RootGraph root2 = Utils.CreateUniqueTestGraph();
            Node n2 = root2.GetOrAddNode("2");
            ClassicAssert.AreEqual(null, n2.GetAttribute("test"));
            ClassicAssert.AreEqual(0, n1.CopyAttributesTo(n2));
            ClassicAssert.AreEqual("bar", n2.GetAttribute("test"));
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

            ClassicAssert.IsTrue(root.Equals(root.MyRootGraph));
            ClassicAssert.IsTrue(root.Equals(tail.MyRootGraph));
            ClassicAssert.IsTrue(root.Equals(edge.MyRootGraph));

            ClassicAssert.AreEqual(3, tail.TotalDegree());
            ClassicAssert.AreEqual(3, head.TotalDegree());
            ClassicAssert.AreEqual(3, root.Nodes().Count());

            root.Delete(edge);

            ClassicAssert.AreEqual(2, tail.TotalDegree());
            ClassicAssert.AreEqual(2, head.TotalDegree());
            ClassicAssert.AreEqual(3, root.Nodes().Count());

            root.Delete(tail);

            ClassicAssert.AreEqual(2, root.Nodes().Count());
            ClassicAssert.AreEqual(2, other.TotalDegree());
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

            ClassicAssert.AreEqual(6, merge.TotalDegree());
            ClassicAssert.AreEqual(4, target.TotalDegree());
            ClassicAssert.AreEqual(3, root.Nodes().Count());

            //root.ComputeDotLayout();
            //root.ToSvgFile("dump1.svg");
            //root.FreeLayout();
            //root.ToDotFile("dump1.dot");

            root.Merge(merge, target);

            //root.ComputeDotLayout();
            //root.ToSvgFile("dump2.svg");
            //root.FreeLayout();
            //root.ToDotFile("dump2.dot");

            ClassicAssert.AreEqual(2, root.Nodes().Count());
            ClassicAssert.AreEqual(3, target.InDegree());
            ClassicAssert.AreEqual(3, target.OutDegree());
            ClassicAssert.AreEqual(2, other.InDegree());
            ClassicAssert.AreEqual(2, other.OutDegree());
        }

        [Test()]
        public void TestEdgeContraction()
        {
            //NativeMethods.AllocConsole();
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


            ClassicAssert.AreEqual(5, tail.TotalDegree());
            ClassicAssert.AreEqual(5, head.TotalDegree());
            ClassicAssert.AreEqual(3, root.Nodes().Count());

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
            ClassicAssert.AreEqual(2, root.Nodes().Count());
            ClassicAssert.AreEqual(2, contraction.InDegree());
            ClassicAssert.AreEqual(2, contraction.OutDegree());
            ClassicAssert.AreEqual(2, other.InDegree());
            ClassicAssert.AreEqual(2, other.OutDegree());
        }

    }
}
