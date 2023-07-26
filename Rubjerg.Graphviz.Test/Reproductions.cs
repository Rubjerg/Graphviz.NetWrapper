using NUnit.Framework;

namespace Rubjerg.Graphviz.Test
{
    /// <summary>
    /// Test various interop-related scenarios that have caused problems in the past.
    /// </summary>
    [TestFixture()]
    public class Reproductions
    {
        [Test()]
        public void StackOverflowRepro()
        {
            Assert.AreEqual(0, ForeignFunctionInterface.StackOverflowRepro());
            //ForeignFunctionInterface.StackOverflowRepro();
        }

        [Test()]
        public void MissingLabelRepro()
        {
            Assert.AreEqual(0, ForeignFunctionInterface.MissingLabelRepro());
            //ForeignFunctionInterface.MissingLabelRepro();
        }

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
            _ = RootGraph.FromDotFile("missing-label-repro.dot");
        }
    }
}
