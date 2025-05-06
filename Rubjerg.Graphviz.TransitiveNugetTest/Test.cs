using System.Linq;
using NUnit.Framework;

namespace Rubjerg.Graphviz.NugetTest
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
            var A = root.GetNode("A");
            Assert.AreEqual(3, A.EdgesOut().Count());

            var B = root.GetNode("B");
            _ = root.GetOrAddEdge(A, B, "");
            Assert.AreEqual(4, A.EdgesOut().Count());

            root.ToSvgFile(TestContext.CurrentContext.TestDirectory + "/dot_out.svg");

            root.ToSvgFile(TestContext.CurrentContext.TestDirectory + "/neato_out.svg", LayoutEngines.Neato);
        }
    }
}
