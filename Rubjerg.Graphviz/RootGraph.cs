using System;
using System.IO;
using System.Linq;
using static Rubjerg.Graphviz.ForeignFunctionInterface;

namespace Rubjerg.Graphviz;

/// <summary>
/// Strict means that there can be at most one edge between any two nodes.
/// </summary>
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
/// This can also be done manually through the Close function, 
/// but this is generally unnecessary unless the graphs are really big.
/// </summary>
public sealed class RootGraph : Graph
{
    private long _added_pressure = 0;
    private bool _closed = false;

    public CoordinateSystem CoordinateSystem { get; }
    /// <summary>
    /// Contains any warnings that Graphviz generated during computation of the layout.
    /// </summary>
    public string? Warnings { get; internal set; }

    private RootGraph(IntPtr ptr, CoordinateSystem coordinateSystem) : base(ptr, null)
    {
        CoordinateSystem = coordinateSystem;
    }
    
    ~RootGraph()
    {
        if (!_closed)
            Close();
    }

    /// <summary>
    /// Manually destroy the graph. All subsequent method calls will likely crash.
    /// Not recommended to call manually, as it is generally unnecessary, unless the graph is really big.
    /// </summary>
    public void Close()
    {
        if (_added_pressure > 0)
            GC.RemoveMemoryPressure(_added_pressure);
        _closed = true;
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
        if (_added_pressure > 0)
            GC.RemoveMemoryPressure(_added_pressure);

        // Up memory pressure proportional to the amount of unmanaged memory in use.
        long unmanaged_bytes_estimate = Nodes().Count() * 104 + Edges().Count() * 64;
        if (unmanaged_bytes_estimate > 0)
            GC.AddMemoryPressure(unmanaged_bytes_estimate);
        _added_pressure = unmanaged_bytes_estimate;
    }

    /// <summary>
    /// Create a new graph.
    /// </summary>
    /// <param name="name">
    /// The name is not interpreted by Graphviz,
    /// except it is recorded and preserved when the graph is written as a file
    /// </param>
    public static RootGraph CreateNew(GraphType graphtype, string? name = null, CoordinateSystem coordinateSystem = CoordinateSystem.BottomLeft)
    {
        name = NameString(name);
        var ptr = Rjagopen(name, (int)graphtype);
        return new RootGraph(ptr, coordinateSystem);
    }

    public static RootGraph FromDotFile(string filename)
    {
        string input;
        using (var sr = new StreamReader(filename))
            input = sr.ReadToEnd();

        return FromDotString(input);
    }

    public static RootGraph FromDotString(string graph, CoordinateSystem coordinateSystem = CoordinateSystem.BottomLeft)
    {
        // Just to be safe, make sure the input has unix line endings. Graphviz does not properly support
        // windows line endings passed to stdin when it comes to attribute line continuations.
        var normalizedDotString = graph.Replace("\r\n", "\n");
        IntPtr ptr = Rjagmemread(normalizedDotString);
        if (ptr == IntPtr.Zero)
        {
            throw new InvalidOperationException("Could not create graph");
        }
        var result = new RootGraph(ptr, coordinateSystem);
        result.UpdateMemoryPressure();
        return result;
    }

    public void ConvertToUndirectedGraph()
    {
        ConvertToUndirected(_ptr);
    }
}
