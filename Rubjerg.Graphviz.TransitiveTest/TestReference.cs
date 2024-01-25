using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Rubjerg.Graphviz.NugetTest
{
    [TestFixture()]
    public class TestReference
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
            var A = root.GetNode("A");
            ClassicAssert.AreEqual(3, A.EdgesOut().Count());

            var B = root.GetNode("B");
            _ = root.GetOrAddEdge(A, B, "");
            ClassicAssert.AreEqual(4, A.EdgesOut().Count());
        }
    }
}
