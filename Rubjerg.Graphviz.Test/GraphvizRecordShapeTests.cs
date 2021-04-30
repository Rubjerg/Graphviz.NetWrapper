using System.Globalization;
using System.IO;
using System.Linq;
using NUnit.Framework;
using static Rubjerg.Graphviz.Test.Utils;

namespace Rubjerg.Graphviz.Test
{
    [TestFixture()]
    public class GraphvizRecordShapeTests
    {
        private RootGraph GenerateProblemGraph(int id, bool fromDotString)
        {
            if (fromDotString)
            {
                return RootGraph.FromDotString($@"
digraph ""problem graph {id}"" {{

    node[fontname = ""Times-Roman"",
        fontsize = 7,
        margin = 0.01
    ];
    A[label = ""{{20 VH|{{1|2}}}}"",
        shape = record];
}}");
            }
            else
            {
                RootGraph root = RootGraph.CreateNew("problem graph " + id.ToString(), GraphType.Directed);
                // Margin between label and node boundary in inches
                Node.IntroduceAttribute(root, "margin", "0.01");
                Node.IntroduceAttribute(root, "fontsize", "7");
                Node.IntroduceAttribute(root, "fontname", "Times-Roman");

                Node nodeA = root.GetOrAddNode("A");

                nodeA.SafeSetAttribute("shape", "record", "");
                nodeA.SafeSetAttribute("label", "{20 VH|{1|2}}", "");
                return root;
            }
        }

        static int id = 0;
        [Test()]
        [Category("Flaky")]
        [TestCase(false, false)]
        // [TestCase(true, false)]
        // [TestCase(false, true)]
        // [TestCase(true, true)]
        public void TestProblem(bool fromDotString, bool testExe)
        {
            var dir = TestContext.CurrentContext.TestDirectory + "\\";
            var problem = GenerateProblemGraph(++id, fromDotString);

            TestContext.Out.WriteLine("INPUT: ");
            TestContext.Out.WriteLine(problem.ToDotString());

            TestContext.Out.WriteLine("OUTPUT: ");
            if (testExe)
            {
                File.WriteAllText(dir + "test.gv", problem.ToDotString());
                LaunchCommandLineApp(dir + "dot.exe", dir + "test.gv -Txdot -O");

                var result = File.ReadAllText(dir + "test.gv.xdot");
                TestContext.Out.WriteLine(result);
            }
            else
            {
                problem.ComputeLayout();
                TestContext.Write(problem.ToDotString());
            }
        }

        /// <summary>
        /// This test used to fail: https://gitlab.com/graphviz/graphviz/-/issues/1894
        /// It still fails on github hosted VMs: https://gitlab.com/graphviz/graphviz/-/issues/1905
        /// </summary>
        [Test()]
        [TestCase("Times-Roman", 7, 0.01)]
        // [TestCase("Times-Roman", 7, 0.5)]
        // [Category("Flaky")]
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

            // TestContext.Write(root.ToDotString());
            root.ComputeLayout();
            // TestContext.Write(root.ToDotString());

            var rects = nodeA.GetRecordRectangles().ToList();
            Assert.That(rects[0].Right, Is.EqualTo(rects[2].Right));
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
    }
}
