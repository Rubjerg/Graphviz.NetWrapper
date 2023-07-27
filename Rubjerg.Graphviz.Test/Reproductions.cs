using NUnit.Framework;
using System.IO;

namespace Rubjerg.Graphviz.Test
{
    /// <summary>
    /// Test various interop-related scenarios that have caused problems in the past.
    /// </summary>
    [TestFixture()]
    public class Reproductions
    {
        // This test only fails when running in isolation
        [Test()]
        public void MissingLabelRepro1()
        {
            Assert.AreEqual(0, ForeignFunctionInterface.MissingLabelRepro());
            //ForeignFunctionInterface.MissingLabelRepro();
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
            //ForeignFunctionInterface.StackOverflowRepro();
        }

        [Test()]
        //[Ignore("This one crashes")]
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
