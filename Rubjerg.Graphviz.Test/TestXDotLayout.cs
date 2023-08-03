using System;
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
            // FIXNOW: parse a sample in which all directives occur
            string xdotString = @"c 7 -#000000 p 4 569.18 36.75 569.18 81.51 590.82 81.51 590.82 36.75 c 7 -#000000 L 2 569.18 70.32 581.12 70.32 c 7 -#000000 L 2 
    569.18 59.13 581.12 59.13 c 7 -#000000 L 2 581.12 47.94 581.12 81.51 c 7 -#000000 L 2 581.12 70.32 590.82 70.32 c 7 -#000000 L 2 
    581.12 59.13 590.82 59.13 c 7 -#000000 L 2 569.18 47.94 590.82 47.94 ";
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
            var xdotDraw = xdotGraph.GetDrawing();
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
