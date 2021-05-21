using System;
using System.IO;
using System.Linq;
using static Rubjerg.Graphviz.ForeignFunctionInterface;

namespace Rubjerg.Graphviz
{
    public enum GraphType
    {
        Directed = 0,
        StrictDirected = 1,
        Undirected = 2,
        StrictUndirected = 3
    }

    /// <summary>
    /// Wraps a cgraph root graph.
    /// NB: If there is no .net wrapper left that points to any part of a root graph, the root graph is destroyed.
    /// </summary>
    public class RootGraph : Graph
    {
        private long added_pressure = 0;
        private RootGraph(IntPtr ptr) : base(ptr, null) { }
        ~RootGraph()
        {
            if (added_pressure > 0)
                GC.RemoveMemoryPressure(added_pressure);
            _ = Agclose(_ptr);
        }

        /// <summary>
        /// Notify the garbage collector of the approximate allocated unmanaged memory used by this graph.
        /// Because it is too much of a hassle to track the exact amount of unmanaged bytes allocated,
        /// we use a rough estimate that is hopefully large enough in most cases to prevent OutOfMemory exceptions,
        /// but hopefully not too large to completely kill GC performance.
        /// This method ignores memory used by attributes.
        /// </summary>
        public void UpdateMemoryPressure()
        {
            if (added_pressure > 0)
                GC.RemoveMemoryPressure(added_pressure);

            long unmanaged_bytes_estimate = Nodes().Count() * 104 + Edges().Count() * 64;
            if (unmanaged_bytes_estimate > 0)
                GC.AddMemoryPressure(unmanaged_bytes_estimate);
            added_pressure = unmanaged_bytes_estimate;
        }

        /// <summary>
        /// Create a new graph.
        /// </summary>
        /// <param name="name">Unique identifier</param>
        public static RootGraph CreateNew(string name, GraphType graphtype)
        {
            var ptr = Rjagopen(name, (int)graphtype);
            return new RootGraph(ptr);
        }

        public static RootGraph FromDotFile(string filename)
        {
            string input;
            using (var sr = new StreamReader(filename))
                input = sr.ReadToEnd();

            return FromDotString(input);
        }

        public static RootGraph FromDotString(string graph)
        {
            IntPtr ptr = Rjagmemread(graph);
            var result = new RootGraph(ptr);
            result.UpdateMemoryPressure();
            return result;
        }

        public void ConvertToUndirectedGraph()
        {
            ConvertToUndirected(_ptr);
        }
    }
}
