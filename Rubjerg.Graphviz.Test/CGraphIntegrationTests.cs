using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Rubjerg.Graphviz.Test
{
    using static Utils;

    [TestFixture()]
    public class CGraphIntegrationTests
    {
        protected virtual int SizeMultiplier => 1;

        [TestCase(10, 5)]
        public void TestTopologicalEqualsIdentity(int nodes, int degree)
        {
            var root = CreateRandomConnectedGraph(nodes * SizeMultiplier, degree);
            Assert.IsTrue(GraphComparer.CheckTopologicallyEquals(root, root, log));
        }

        [TestCase(10, 5)]
        public void TestTopologicalEqualsClone(int nodes, int degree)
        {
            var root = CreateRandomConnectedGraph(nodes * SizeMultiplier, degree);
            var clone = root.Clone(root.GetName() + "clone");
            Assert.IsTrue(GraphComparer.CheckTopologicallyEquals(root, clone, log));
        }

        /// <summary>
        /// This test fails if the locking doesn't work, and the GC runs async.
        /// </summary>
        [TestCase(100, 10000)]
        public void TestGetNode(int initial_graphs, int get_attempts)
        {
            for (int j = 0; j < initial_graphs; j++)
            {
                var pre = CreateUniqueTestGraph();
                for (int i = 0; i < 10; i++)
                {
                    pre.GetOrAddNode(i.ToString());
                }
            }

            var root = CreateUniqueTestGraph();
            root.GetOrAddNode("node1");
            for (int i = 0; i < get_attempts * SizeMultiplier; i++)
            {
                var node = root.GetNode("node1");
                Assert.IsNotNull(node);
            }
        }

        [TestCase(500, 10)]
        public void TestAddNode(int nodes, int degree)
        {
            int initcount = nodes * SizeMultiplier;
            var root = CreateRandomConnectedGraph(initcount, degree);
            int addcount = nodes * SizeMultiplier;
            var watch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = initcount; i < initcount + addcount; i++)
            {
                root.GetOrAddNode(i.ToString());
            }
            watch.Stop();
            var elapsedms = watch.ElapsedMilliseconds;
            log($"Elapsed ms: {elapsedms}");
            Assert.AreEqual(initcount + addcount, root.Nodes().Count());
        }

        [TestCase(500, 10)]
        public void TestDeleteNode(int nodes, int degree)
        {
            int initcount = nodes * 2 * SizeMultiplier;
            var root = CreateRandomConnectedGraph(initcount, degree);
            int delcount = nodes * SizeMultiplier;
            var watch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = initcount - delcount; i < initcount; i++)
            {
                var node = root.GetOrAddNode(i.ToString());
                root.Delete(node);
            }
            watch.Stop();
            var elapsedms = watch.ElapsedMilliseconds;
            log($"Elapsed ms: {elapsedms}");
            Assert.AreEqual(initcount - delcount, root.Nodes().Count());
        }

        [TestCase(100, 10)]
        public void TestBFS(int nodes, int degree)
        {
            int initcount = nodes * SizeMultiplier;
            var root = CreateRandomConnectedGraph(initcount, degree);
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var start = root.GetOrAddNode(0.ToString());
            var visited = new HashSet<Node>();
            var front = new Queue<Node>();
            front.Enqueue(start);
            while (front.Any())
            {
                Node current = front.Dequeue();
                foreach (var n in current.NeighborsOut())
                {
                    if (!visited.Contains(n))
                    {
                        visited.Add(n);
                        front.Enqueue(n);
                    }
                }
            }

            watch.Stop();
            var elapsedms = watch.ElapsedMilliseconds;
            log($"Elapsed ms: {elapsedms}");
            Assert.AreEqual(initcount, visited.Count);
        }
    }
}
