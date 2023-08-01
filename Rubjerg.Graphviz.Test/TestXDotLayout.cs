using System;
using System.Globalization;
using System.IO;
using System.Linq;
using NUnit.Framework;
using static Rubjerg.Graphviz.Test.Utils;

namespace Rubjerg.Graphviz.Test
{
    [TestFixture()]
    public class TestXDotLayout
    {
        [Test()]
        public void TestXDotTranslate()
        {
            string xdotString = "c 9 -#fffffe00 C 7 -#ffffff P 4 0 0 0 72.25 136.5 72.25 136.5 0";
            IntPtr xdot = XDotFFI.parseXDot(xdotString);
            try
            {
                var result = XDotTranslator.TranslateXDot(xdot);
            }
            finally
            {
                if (xdot != IntPtr.Zero)
                {
                    XDotFFI.freeXDot(xdot);
                }
            }
        }

        [Test()]
        public void TestRecordShapeOrder()
        {
            RootGraph root = CreateUniqueTestGraph();
            Node nodeA = root.GetOrAddNode("A");

            nodeA.SafeSetAttribute("shape", "record", "");
            nodeA.SafeSetAttribute("label", "1|2|3|{4|5}|6|{7|8|9}", "\\N");

            var xdotGraph = root.CreateDotLayout();

            var xNodeA = xdotGraph.GetNode("A");
            var rects = xNodeA.GetRecordRectangles().ToList();

            // Because Graphviz uses a lower-left originated coordinate system, we need to flip the y coordinates
            Utils.AssertOrder(rects, r => (r.Left, -r.Top));
            Assert.That(rects.Count, Is.EqualTo(9));

            // Test xdot translation
            var xdotDraw = xdotGraph.GetDraw();
        }

        [Test()]
        public void TestEmptyRecordShapes()
        {
            RootGraph root = CreateUniqueTestGraph();
            Node nodeA = root.GetOrAddNode("A");
            nodeA.SafeSetAttribute("shape", "record", "");
            nodeA.SafeSetAttribute("label", "||||", "");

            var xdotGraph = root.CreateDotLayout();

            var xNodeA = xdotGraph.GetNode("A");
            var rects = xNodeA.GetRecordRectangles().ToList();
            Assert.That(rects.Count, Is.EqualTo(5));
        }
    }
}
