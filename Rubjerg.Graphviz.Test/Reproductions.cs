using NUnit.Framework;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Rubjerg.Graphviz.Test
{
    /// <summary>
    /// Test various scenarios that have caused problems in the past.
    /// </summary>
    [TestFixture()]
    public class Reproductions
    {
        [DllImport("GraphvizWrapper.dll", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        protected static extern void rj_debug();

        [Test()]
        [Ignore("For debugging")]
        public void debug()
        {
            NativeMethods.CreateConsole();
            rj_debug();
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

        // This test only fails when running in isolation
        [Test()]
        public void MissingLabelRepro1()
        {
            Assert.AreEqual(0, ForeignFunctionInterface.MissingLabelRepro());
        }

        // This test only fails when running in isolation
        [Test()]
        public void MissingLabelRepro2()
        {
            var graph = RootGraph.FromDotFile("Rubjerg.Graphviz/missing-label-repro.dot");
            graph.ComputeLayout();
            graph.ToSvgFile("Rubjerg.Graphviz/test.svg");
            string svgString = File.ReadAllText("Rubjerg.Graphviz/test.svg");
            Assert.IsTrue(svgString.Contains(">OpenNode</text>"));
        }

        [Test()]
        public void StackOverflowRepro()
        {
            Assert.AreEqual(0, ForeignFunctionInterface.StackOverflowRepro());
        }

        [Test()]
        public void StackOverflowRepro2()
        {
            var graph = RootGraph.FromDotFile("Rubjerg.Graphviz/stackoverflow-repro.dot");
            graph.ComputeLayout();
        }

        // TODO: remove all agread references; we should always handle file I/O ourselves
        [Test()]
        public void TestAgread()
        {
            Assert.AreEqual(0, ForeignFunctionInterface.TestAgread());
        }

        [Test()]
        public void TestAgmemread()
        {
            Assert.AreEqual(0, ForeignFunctionInterface.TestAgmemread());
        }

        [Test()]
        public void TestRjAgmemread()
        {
            Assert.AreEqual(0, ForeignFunctionInterface.TestRjAgmemread());
        }

        [Test()]
        public void TestFromDotFile()
        {
            _ = RootGraph.FromDotFile("Rubjerg.Graphviz/missing-label-repro.dot");
        }
    }
}
