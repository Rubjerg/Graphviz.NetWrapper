using NUnit.Framework;
using Rubjerg.Graphviz.Test;

namespace Rubjerg.Graphviz.TransitiveTest
{
    [TestFixture()]
    public class TransitiveTest
    {
        [Test()]
        public void Test()
        {
            var tutorial = new Tutorial();
            tutorial.GraphConstruction();
            tutorial.Layouting();
        }
    }
}
