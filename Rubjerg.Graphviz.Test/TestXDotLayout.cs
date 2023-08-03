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
            // FIXNOW: parse a sample in which all directives occur
            string xdotString = @"
F 14 11 -Times-Roman c 7 -#000000 T 11.38 30.7 0 6.75 1 -1 F 14 11 -Times-Roman c 7 -#000000 T 34.12 30.7 0 6.75 1 -2 F 14 11 -Times-Roman \
c 7 -#000000 T 56.88 30.7 0 6.75 1 -3 F 14 11 -Times-Roman c 7 -#000000 T 79.62 48.95 0 6.75 1 -4 F 14 11 -Times-Roman c 7 -#000000 \
T 79.62 13.7 0 6.75 1 -5 F 14 11 -Times-Roman c 7 -#000000 T 102.38 30.7 0 6.75 1 -6 F 14 11 -Times-Roman c 7 -#000000 T 125.12 \
54.45 0 6.75 1 -7 F 14 11 -Times-Roman c 7 -#000000 T 125.12 30.7 0 6.75 1 -8 F 14 11 -Times-Roman c 7 -#000000 T 125.12 6.95 0 \
6.75 1 -9 ";
            IntPtr xdot = XDotFFI.parseXDot(xdotString);
            try
            {
                var result = XDotTranslator.TranslateXDot(xdot);
                Assert.AreEqual(27, result.Count());
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
