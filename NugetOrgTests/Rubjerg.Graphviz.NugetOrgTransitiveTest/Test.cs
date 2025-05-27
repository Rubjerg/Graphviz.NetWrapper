using NUnit.Framework;
using Rubjerg.Graphviz.Test;

namespace Rubjerg.Graphviz.NugetOrgTest
{
    [TestFixture()]
    public class TestNugetPackageTransitive
    {
        [Test()]
        public void Test()
        {
            var test = new TestNugetPackage();
            test.TestReadDotFile();
        }
    }
}

