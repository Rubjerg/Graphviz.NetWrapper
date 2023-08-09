using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Rubjerg.Graphviz.Test;

public static class Utils
{
    private static readonly Random _rand = new Random();

    internal static int _rootGraphCounter = 0;
    public static RootGraph CreateUniqueTestGraph()
    {
        _rootGraphCounter += 1;
        return RootGraph.CreateNew(GraphType.Directed, "test graph " + _rootGraphCounter.ToString());
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
                int j = _rand.Next(size - 1);
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
        _ = source ?? throw new ArgumentNullException("source");

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
}
