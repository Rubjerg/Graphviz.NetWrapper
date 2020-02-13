using NUnit.Framework;

namespace NUnitTestProjectWithIndirectDependencies
{
    public class Tests
    {
        [Test]
        public void TestGraphvizCalls()
        {
            IntermediateProject.Class1.RunTest();
        }
    }
}