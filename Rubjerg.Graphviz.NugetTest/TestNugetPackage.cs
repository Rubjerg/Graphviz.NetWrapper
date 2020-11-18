using System;
using System.Linq;
using NUnit.Framework;

namespace Rubjerg.Graphviz.Test
{
    [TestFixture()]
    public class TestNugetPackage
    {
        [Test()]
        public void TestReadDotFile()
        {
            RootGraph root = RootGraph.FromDotString(@"
digraph test {
    A;
    B;
    B -> B;
    A -> B[name = edgename];
    A -> B[name = edgename];
    A -> B[name = edgename];
}
");
            var edges = root.Edges().ToList();
            var names = edges.Select(e => e.GetName());
            // This results in ,,, strangely enough
            // There seems to be no way to influence the edge name from dot
            Console.WriteLine(string.Join(", ", names));

            // However, it is strange that all edges seem to have te same name, namely ""
            // According to the documentation, the name is used to distinguish between multiedges
            var A = root.GetNode("A");
            var B = root.GetNode("B");
            Assert.AreEqual(3, A.EdgesOut().Count());

            // The documentation seem to be correct for edges that are added through the C interface
            root.GetOrAddEdge(A, B, "");
            root.GetOrAddEdge(A, B, "");
            root.GetOrAddEdge(A, B, "");
            Assert.AreEqual(4, A.EdgesOut().Count());
        }
    }
}
