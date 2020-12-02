using System;
using System.Linq;
using NUnit.Framework;

namespace Rubjerg.Graphviz.Test
{
    [TestFixture()]
    public class GraphvizBasicOperations
    {
        [Test()]
        public void TestHtmlLabels()
        {
            RootGraph root = Utils.CreateUniqueTestGraph();
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
            RootGraph root = Utils.CreateUniqueTestGraph();
            Node nodeA = root.GetOrAddNode("A");

            nodeA.SafeSetAttribute("shape", "record", "");
            nodeA.SafeSetAttribute("label", "1|2|3|{4|5}|6|{7|8|9}", "\\N");

            root.ComputeLayout();

            var rects = nodeA.GetRecordRectangles().ToList();

            //root.ToSvgFile(TestContext.CurrentContext.TestDirectory + "/dot_out.svg");
            //root.ToDotFile(TestContext.CurrentContext.TestDirectory + "/dot_out.dot");

            // Because Graphviz uses a lower-left originated coordinate system, we need to flip the y coordinates
            Utils.AssertOrder(rects, r => (r.Left, -r.Top));
            Assert.That(rects.Count, Is.EqualTo(9));
        }

        [Test()]
        public void TestRecordShapeAlignment()
        {
            RootGraph root = Utils.CreateUniqueTestGraph();
            // Margin between label and node boundary in inches
            Node.IntroduceAttribute(root, "margin", "0.01");

            Node nodeA = root.GetOrAddNode("A");

            nodeA.SafeSetAttribute("shape", "record", "");
            nodeA.SafeSetAttribute("label", "{20 VH|{1|2}}", "");

            root.ComputeLayout();

            var rects = nodeA.GetRecordRectangles().ToList();

            //root.ToSvgFile(TestContext.CurrentContext.TestDirectory + "/dot_out.svg");
            //root.ToDotFile(TestContext.CurrentContext.TestDirectory + "/dot_out.dot");

            Assert.That(rects[0].Right, Is.EqualTo(rects[2].Right));
        }



        [Test()]
        public void TestPortNames()
        {
            Assert.That(Node.ConvertUidToPortName("a:b"), Is.EqualTo(Node.ConvertUidToPortName("a:b")));
            Assert.That(Node.ConvertUidToPortName(":"), Is.Not.EqualTo(Node.ConvertUidToPortName("+")));
            Assert.That(Node.ConvertUidToPortName("<a>:h|b"), Is.Not.EqualTo(Node.ConvertUidToPortName("<a>:h+b")));
            Assert.That(Node.ConvertUidToPortName("<a>:h|b"), Does.Not.Contain("<"));
            Assert.That(Node.ConvertUidToPortName("<a>:h|b"), Does.Not.Contain(">"));
            Assert.That(Node.ConvertUidToPortName("<a>:h|b"), Does.Not.Contain(":"));
        }
    }
}
