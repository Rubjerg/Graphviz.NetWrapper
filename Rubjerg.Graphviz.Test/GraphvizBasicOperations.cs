using System.Globalization;
using System.IO;
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
        /// It still fails on github hosted VMs: https://gitlab.com/graphviz/graphviz/-/issues/1905
        /// </summary>
        [Test()]
        [TestCase("Times-Roman", 7, 0.01)]
        [TestCase("Times-Roman", 7, 0.5)]
        //[Category("Flaky")]
        public void TestRecordShapeAlignment(string fontname, double fontsize, double margin)
        {
            RootGraph root = CreateUniqueTestGraph();
            // Margin between label and node boundary in inches
            Node.IntroduceAttribute(root, "margin", margin.ToString(CultureInfo.InvariantCulture));
            Node.IntroduceAttribute(root, "fontsize", fontsize.ToString(CultureInfo.InvariantCulture));
            Node.IntroduceAttribute(root, "fontname", fontname);

            Node nodeA = root.GetOrAddNode("A");

            nodeA.SafeSetAttribute("shape", "record", "");
            nodeA.SafeSetAttribute("label", "{20 VH|{1|2}}", "");

            TestContext.Write(root.ToDotString());
            root.ComputeLayout();
            TestContext.Write(root.ToDotString());

            var rects = nodeA.GetRecordRectangles().ToList();
            Assert.That(rects[0].Right, Is.EqualTo(rects[2].Right));
        }

        [Test()]
        public void TestFoo()
        {
            var dir = TestContext.CurrentContext.TestDirectory + "\\";
            File.WriteAllText(dir + "test.gv",
                @"
digraph ""test graph 1"" {

    node[fontname = ""Times-Roman"",
        fontsize = 7,
        margin = 0.01
    ];
    A[label = ""{20 VH|{1|2}}"",
        shape = record];
}");
            LaunchCommandLineApp(dir + "dot.exe", dir + "test.gv -Txdot -O");

            var result = File.ReadAllText(dir + "test.gv.xdot");
            TestContext.Out.WriteLine("^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^");
            TestContext.Out.WriteLine(result);
            TestContext.Out.WriteLine("^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^");

        }

        [Test()]
        public void TestEmptyRecordShapes()
        {
            RootGraph root = CreateUniqueTestGraph();
            Node nodeA = root.GetOrAddNode("A");
            nodeA.SafeSetAttribute("shape", "record", "");
            nodeA.SafeSetAttribute("label", "||||", "");

            root.ComputeLayout();

            var rects = nodeA.GetRecordRectangles().ToList();
            Assert.That(rects.Count, Is.EqualTo(5));
            root.ToSvgFile(GetTestFilePath("out.svg"));
        }

        [Test()]
        [TestCase(true)]
        [TestCase(false)]
        public void TestPortNameConversion(bool escape)
        {
            string port1 = ">|<";
            string port2 = "B";
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

                Node node = root.GetNode("N");
                Assert.That(node.GetAttribute("label"), Is.EqualTo(label));
                Edge edge = root.Edges().First();
                Assert.That(edge.GetAttribute("tailport"), Is.EqualTo(port1 + ":n"));
                Assert.That(edge.GetAttribute("headport"), Is.EqualTo(port2 + ":s"));

                root.ComputeLayout();
                root.ToSvgFile(GetTestFilePath("out.svg"));
                root.ToDotFile(GetTestFilePath("out.dot"));

                var rects = node.GetRecordRectangles();
                if (escape)
                    Assert.That(rects.Count, Is.EqualTo(2));
                else
                    Assert.That(rects.Count, Is.EqualTo(3));
            }
        }

        [Test()]
        [TestCase(true)]
        [TestCase(false)]
        public void TestLabelEscaping(bool escape)
        {
            string label1 = "|";
            string label2 = @"\N\n\L";
            string label3 = "3";
            if (escape)
            {
                label1 = CGraphThing.EscapeLabel(label1);
                label2 = CGraphThing.EscapeLabel(label2);
            }

            {
                RootGraph root = CreateUniqueTestGraph();
                Node node1 = root.GetOrAddNode("1");
                node1.SafeSetAttribute("shape", "record", "");
                node1.SafeSetAttribute("label", label1, "");
                Node node2 = root.GetOrAddNode("2");
                node2.SafeSetAttribute("label", label2, "");
                Node node3 = root.GetOrAddNode("3");
                node3.SafeSetAttribute("label", label3, "");
                root.ToDotFile(GetTestFilePath("out.gv"));
            }

            {
                var root = RootGraph.FromDotFile(GetTestFilePath("out.gv"));

                Node node1 = root.GetNode("1");
                Assert.That(node1.GetAttribute("label"), Is.EqualTo(label1));
                Node node2 = root.GetNode("2");
                Assert.That(node2.GetAttribute("label"), Is.EqualTo(label2));
                Node node3 = root.GetNode("3");
                Assert.That(node3.GetAttribute("label"), Is.EqualTo(label3));

                root.ComputeLayout();
                root.ToSvgFile(GetTestFilePath("out.svg"));
                root.ToDotFile(GetTestFilePath("out.dot"));

                var rects = node1.GetRecordRectangles();
                if (escape)
                {
                    Assert.That(rects.Count, Is.EqualTo(1));
                    Assert.That(node2.BoundingBox().Height, Is.EqualTo(node3.BoundingBox().Height));
                }
                else
                {
                    Assert.That(rects.Count, Is.EqualTo(2));
                    Assert.That(node2.BoundingBox().Height, Is.Not.EqualTo(node3.BoundingBox().Height));
                }
            }
        }
    }
}
