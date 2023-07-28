using NUnit.Framework;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Rubjerg.Graphviz.Test
{
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

        /// <summary>
        /// This test used to fail: https://gitlab.com/graphviz/graphviz/-/issues/1894
        /// It still fails on github hosted VMs: https://gitlab.com/graphviz/graphviz/-/issues/1905
        /// </summary>
        [Test()]
        [TestCase("Times-Roman", 7, 0.01)]
        [TestCase("Times-Roman", 7, 0.5)]
        [Category("Flaky")]
        public void TestRecordShapeAlignment(string fontname, double fontsize, double margin)
        {
            RootGraph root = Utils.CreateUniqueTestGraph();
            // Margin between label and node boundary in inches
            Node.IntroduceAttribute(root, "margin", margin.ToString(CultureInfo.InvariantCulture));
            Node.IntroduceAttribute(root, "fontsize", fontsize.ToString(CultureInfo.InvariantCulture));
            Node.IntroduceAttribute(root, "fontname", fontname);

            Node nodeA = root.GetOrAddNode("A");

            nodeA.SafeSetAttribute("shape", "record", "");
            nodeA.SafeSetAttribute("label", "{20 VH|{1|2}}", "");

            //TestContext.Write(root.ToDotString());
            root.ComputeLayout();
            //TestContext.Write(root.ToDotString());

            var rects = nodeA.GetRecordRectangles().ToList();
            Assert.That(rects[0].Right, Is.EqualTo(rects[2].Right));
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
    }
}
