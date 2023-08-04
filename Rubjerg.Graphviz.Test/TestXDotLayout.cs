using System;
using System.Linq;
using NUnit.Framework;

namespace Rubjerg.Graphviz.Test
{
    [TestFixture()]
    public class TestXDotLayout
    {
        [Test()]
        public void TestXDotTranslate()
        {
            RootGraph root = Utils.CreateUniqueTestGraph();
            Node nodeA = root.GetOrAddNode("A");

            nodeA.SafeSetAttribute("shape", "record", "");
            nodeA.SafeSetAttribute("label", "1|2|3|{4|5}|6|{7|8|9}", "\\N");

            var xdotGraph = root.CreateLayout();
            // FIXNOW: parse a sample in which all directives occur
            var xNodeA = xdotGraph.GetNode("A");
            var ldraw = xNodeA.GetLabelDrawing();
            Assert.AreEqual(27, ldraw.Count);
        }

        [Test()]
        public void TestRecordShapeOrder()
        {
            RootGraph root = Utils.CreateUniqueTestGraph();
            Node nodeA = root.GetOrAddNode("A");

            nodeA.SafeSetAttribute("shape", "record", "");
            nodeA.SafeSetAttribute("label", "1|2|3|{4|5}|6|{7|8|9}", "\\N");


            var xdotGraph = root.CreateLayout();

            var xNodeA = xdotGraph.GetNode("A");
            var rects = xNodeA.GetRecordRectangles().ToList();

            // Because Graphviz uses a lower-left originated coordinate system, we need to flip the y coordinates
            Utils.AssertOrder(rects, r => (r.Left, -r.Top));
            Assert.That(rects.Count, Is.EqualTo(9));

            // Test xdot translation
            var xdotDraw = xdotGraph.GetDrawing();
        }

        [Test()]
        public void TestEmptyRecordShapes()
        {
            RootGraph root = Utils.CreateUniqueTestGraph();
            Node nodeA = root.GetOrAddNode("A");
            nodeA.SafeSetAttribute("shape", "record", "");
            nodeA.SafeSetAttribute("label", "||||", "");

            var xdotGraph = root.CreateLayout();

            var xNodeA = xdotGraph.GetNode("A");
            var rects = xNodeA.GetRecordRectangles().ToList();
            Assert.That(rects.Count, Is.EqualTo(5));
        }
    }
}
