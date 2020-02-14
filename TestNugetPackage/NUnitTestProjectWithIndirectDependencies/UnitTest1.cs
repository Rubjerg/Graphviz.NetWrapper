using NUnit.Framework;

namespace NUnitTestProjectWithIndirectDependencies
{
    [TestFixture]
    public class Tests
    {
        /// <summary>
        /// Test the deployment of the live nuget package by making some basic calls to the graphviz libraries.
        /// </summary>
        [Test]
        public void TestGraphvizCalls()
        {
            IntermediateProject.Class1.RunTest();
        }
    }
}