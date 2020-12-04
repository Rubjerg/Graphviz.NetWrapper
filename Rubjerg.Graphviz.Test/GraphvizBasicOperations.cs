using System;
using System.Linq;
using NUnit.Framework;
using static Rubjerg.Graphviz.Test.Utils;

namespace Rubjerg.Graphviz.Test
{
    [TestFixture()]
    public class GraphvizBasicOperations
    {
        [Test()]
        public void TestHtmlLabels()
        {
            RootGraph root = CreateUniqueTestGraph();
            const string labelKey = "label";
            Node.IntroduceAttribute(root, labelKey, "");

            Node n1 = root.GetOrAddNode("1");
            Node n2 = root.GetOrAddNode("2");

            n1.SetAttribute(labelKey, "raw text");
            n2.SetAttribute(labelKey, "<html text>");

            var result = root.ToDotString();

            Assert.That(result, Does.Contain("\"raw text\""));

            // Html labels are not string quoted in dot file
            Assert.That(result, Does.Not.Contain("\"<html text>\""));
            Assert.That(result, Does.Not.Contain("\"<<html text>>\""));
            // Htmls labels have additional angel bracket delimeters added
            Assert.That(result, Does.Contain("<<html text>>"));
        }

        [Test()]
        public void TestRecordShapeOrder()
        {
            RootGraph root = CreateUniqueTestGraph();
            Node nodeA = root.GetOrAddNode("A");

            nodeA.SafeSetAttribute("shape", "record", "");
            nodeA.SafeSetAttribute("label", "1|2|3|{4|5}|6|{7|8|9}", "\\N");

            root.ComputeLayout();

            var rects = nodeA.GetRecordRectangles().ToList();

            // Because Graphviz uses a lower-left originated coordinate system, we need to flip the y coordinates
            Utils.AssertOrder(rects, r => (r.Left, -r.Top));
            Assert.That(rects.Count, Is.EqualTo(9));
        }

        /// <summary>
        /// This test used to fail: https://gitlab.com/graphviz/graphviz/-/issues/1894
        /// </summary>
        [Test()]
        public void TestRecordShapeAlignment()
        {
            RootGraph root = CreateUniqueTestGraph();
            // Margin between label and node boundary in inches
            Node.IntroduceAttribute(root, "margin", "0.01");

            Node nodeA = root.GetOrAddNode("A");

            nodeA.SafeSetAttribute("shape", "record", "");
            nodeA.SafeSetAttribute("label", "{20 VH|{1|2}}", "");

            root.ComputeLayout();

            var rects = nodeA.GetRecordRectangles().ToList();
            Assert.That(rects[0].Right, Is.EqualTo(rects[2].Right));
        }

        [Test()]
        public void TestEmptyRecordShapes()
        {
            RootGraph root = CreateUniqueTestGraph();
            Node nodeA = root.GetOrAddNode("A");
            nodeA.SafeSetAttribute("shape", "record", "");
            nodeA.SafeSetAttribute("label", "||||", "");

            root.ComputeLayout();
            root.ToSvgFile(GetTestFilePath("out.svg"));

            var rects = nodeA.GetRecordRectangles().ToList();
            Assert.That(rects.Count, Is.EqualTo(5));
            root.ToSvgFile(GetTestFilePath("out.svg"));
        }

        [Test()]
        //[TestCase(true)]
        [TestCase(false)]
        public void TestPortNames(bool escape)
        {
            string port1 = ":A<\\|:x";
            string port2 = "B:y";
            if (escape)
            {
                port1 = Edge.ConvertUidToPortName(port1);
                port2 = Edge.ConvertUidToPortName(port2);
            }
            string label = $"{{<{port1}>1|<{port2}>2}}";

            {
                RootGraph root = CreateUniqueTestGraph();
                Node node = root.GetOrAddNode("N");
                node.SafeSetAttribute("shape", "record", "");
                node.SafeSetAttribute("label", label, "");
                Edge edge = root.GetOrAddEdge(node, node, "");
                edge.SafeSetAttribute("tailport", port1 + ":n", "");
                edge.SafeSetAttribute("headport", port2 + ":s", "");
                root.ToDotFile(GetTestFilePath("out.gv"));
            }

            {
                var root = RootGraph.FromDotFile(GetTestFilePath("out.gv"));
                root.ComputeLayout();
                root.ToSvgFile(GetTestFilePath("out.svg"));
                root.ToDotFile(GetTestFilePath("out.dot"));

                Node node = root.GetNode("N");
                Assert.That(node.GetAttribute("label") == label, Is.EqualTo(escape));
                Edge edge = root.Edges().First();
                Assert.That(edge.GetAttribute("tailport") == port1 + ":n", Is.EqualTo(escape));
                Assert.That(edge.GetAttribute("headport") == port2 + ":s", Is.EqualTo(escape));
            }
        }
    }
}
