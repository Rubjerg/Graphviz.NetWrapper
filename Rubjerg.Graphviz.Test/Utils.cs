using NUnit.Framework;
using System;

namespace Rubjerg.Graphviz.Test
{
    static class Utils
    {
        private static readonly Random rand = new Random();

        static int RootGraphCounter = 0;
        public static RootGraph CreateUniqueTestGraph()
        {
            RootGraphCounter += 1;
            return RootGraph.CreateNew("test graph " + RootGraphCounter.ToString(), GraphType.Directed);
        }

        public static RootGraph CreateRandomConnectedGraph(int size, double out_degree)
        {
            RootGraph root = Utils.CreateUniqueTestGraph();

            // First generate a star of requested size
            Node centernode = root.GetOrAddNode(0.ToString());
            for (int i = 1; i < size; i++)
            {
                var node = root.GetOrAddNode(i.ToString());
                root.GetOrAddEdge(centernode, node, $"{0} to {i}");
            }

            // For each node pick requested number of random neighbors
            for (int i = 0; i < size; i++)
            {
                // We already have one out edge for each node
                for (int _ = 0; _ < out_degree - 2; _++)
                {
                    var node = root.GetNode(i.ToString());
                    int j = rand.Next(size - 1);
                    var neighbor = root.GetNode(j.ToString());
                    root.GetOrAddEdge(node, neighbor, $"{i} to {j}");
                }
            }

            return root;
        }


        public static void log(string message)
        {
            TestContext.WriteLine(message);
        }
    }
}
