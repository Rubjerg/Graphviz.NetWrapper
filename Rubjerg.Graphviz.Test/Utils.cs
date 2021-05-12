using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Rubjerg.Graphviz.Test
{
    public static class Utils
    {
        private static readonly Random rand = new Random();

        static int RootGraphCounter = 0;
        public static RootGraph CreateUniqueTestGraph()
        {
            RootGraphCounter += 1;
            return RootGraph.CreateNew("test graph " + RootGraphCounter.ToString(), GraphType.Directed);
        }

        public static string GetTestFilePath(string fileName)
        {
            return $"{TestContext.CurrentContext.TestDirectory}/{fileName}";
        }

        public static RootGraph CreateRandomConnectedGraph(int size, double out_degree)
        {
            RootGraph root = CreateUniqueTestGraph();

            // First generate a star of requested size
            Node centernode = root.GetOrAddNode(0.ToString());
            for (int i = 1; i < size; i++)
            {
                var node = root.GetOrAddNode(i.ToString());
                _ = root.GetOrAddEdge(centernode, node, $"{0} to {i}");
            }

            // For each node pick requested number of random neighbors
            for (int i = 0; i < size; i++)
            {
                // We already have one out edge for each node
                for (int x = 0; x < out_degree - 2; x++)
                {
                    var node = root.GetNode(i.ToString());
                    int j = rand.Next(size - 1);
                    var neighbor = root.GetNode(j.ToString());
                    _ = root.GetOrAddEdge(node, neighbor, $"{i} to {j}");
                }
            }

            return root;
        }

        public static void Log(string message)
        {
#if debug
            TestContext.WriteLine(message);
#endif
        }

        public static void AssertPattern(string expectedRegex, string actual)
        {
            Assert.IsTrue(Regex.IsMatch(actual, expectedRegex));
        }

        public static void AssertOrder<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
        {
            Assert.IsTrue(IsOrdered(source, keySelector));
        }

        public static bool IsOrdered<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            var comparer = Comparer<TKey>.Default;
            using (var iterator = source.GetEnumerator())
            {
                if (!iterator.MoveNext())
                    return true;

                TKey current = keySelector(iterator.Current);

                while (iterator.MoveNext())
                {
                    TKey next = keySelector(iterator.Current);
                    if (comparer.Compare(current, next) > 0)
                        return false;

                    current = next;
                }
            }

            return true;
        }


        /// <summary>
        /// Launch the application with some options set.
        /// </summary>
        public static void LaunchCommandLineApp(string name, string args)
        {
            // Use ProcessStartInfo class
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.FileName = name;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.Arguments = args;

            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using (Process exeProcess = Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                }
            }
            catch (Exception e)
            {
                TestContext.WriteLine(e.Message);
            }
        }
    }
}
