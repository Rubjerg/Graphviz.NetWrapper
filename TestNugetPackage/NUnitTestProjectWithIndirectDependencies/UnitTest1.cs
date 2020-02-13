using NUnit.Framework;

namespace NUnitTestProjectWithIndirectDependencies
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void TestGraphvizCalls()
        {
            IntermediateProject.Class1.RunTest();
        }
    }
}