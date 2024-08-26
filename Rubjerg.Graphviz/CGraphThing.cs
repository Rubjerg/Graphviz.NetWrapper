using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using static Rubjerg.Graphviz.ForeignFunctionInterface;
using System.Globalization;

namespace Rubjerg.Graphviz;

// NOTE ABOUT ATTRIBUTES:
// Cgraph assumes that all objects of a given kind(graphs/subgraphs, nodes, or edges) have the same attributes
// - there’s no notion of subtyping within attributes. Information about attributes is stored in data dictionaries.
// Each graph has three (for graphs/subgraphs, nodes, and edges) for which you'll need the predefined constants
// AGRAPH, AGNODE and AGEDGE in calls to create, search and walk these dictionaries.
public abstract class CGraphThing : GraphvizThing
{
    public readonly RootGraph MyRootGraph;

    /// <summary>
    /// Argument root may be null.
    /// In that case, it is assumed this is a RootGraph, and MyRootGraph is set to `this`.
    /// </summary>
    internal CGraphThing(IntPtr ptr, RootGraph? root) : base(ptr)
    {
        if (root is null)
            MyRootGraph = (RootGraph)this;
        else
            MyRootGraph = root;
    }

    protected static string? NameString(string? name)
    {
        // Because graphviz does not properly export empty strings to dot, this opens a can of worms.
        // So we disallow it, and map it onto null.
        // Related issue: https://gitlab.com/graphviz/graphviz/-/issues/1887
        return name == string.Empty ? null : name;
    }

    /// <summary>
    /// Identifier for this object. Used to distinghuish multi edges.
    /// Edges can be nameless, and in that case this method returns null.
    /// </summary>
    public string? GetName()
    {
        return NameString(Rjagnameof(_ptr));
    }

    public bool HasAttribute(string name)
    {
        return !new[] { null, "" }.Contains(GetAttribute(name));
    }

    /// <summary>
    /// Set attribute, and introduce it with the given default if it is not introduced yet.
    /// </summary>
    public void SafeSetAttribute(string name, string? value, string? deflt)
    {
        _ = deflt ?? throw new ArgumentNullException(nameof(deflt));
        Agsafeset(_ptr, name, value, deflt);
    }

    /// <summary>
    /// Set attribute, and introduce it with the empty string if it does not exist yet.
    /// </summary>
    public void SetAttribute(string name, string? value)
    {
        Agsafeset(_ptr, name, value, "");
    }

    /// <summary>
    /// Get the attribute value for this object, or the default value of the attribute if no explicit value was set.
    /// If the attribute was not introduced, return null.
    /// </summary>
    public string? GetAttribute(string name)
    {
        return Agget(_ptr, name);
    }

    /// <summary>
    /// Get the attribute if it was introduced and contains a non-empty value, otherwise return deflt.
    /// </summary>
    public string SafeGetAttribute(string name, string deflt)
    {
        if (HasAttribute(name))
            return GetAttribute(name)!;
        return deflt;
    }

    /// <summary>
    /// Get the attribute if it was introduced and contains a non-empty value, otherwise return null.
    /// </summary>
    public string? SafeGetAttribute(string name)
    {
        if (HasAttribute(name))
            return GetAttribute(name)!;
        return null;
    }

    public void SetAttributeHtml(string name, string value)
    {
        AgsetHtml(_ptr, name, value);
    }

    /// <summary>
    /// Get all attributes as dictionary.
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, string> GetAttributes()
    {
        var attributes = new Dictionary<string, string>();
        for (int kind = 0; kind < 3; ++kind)
        {
            IntPtr sym = Agnxtattr(MyRootGraph._ptr, kind, IntPtr.Zero);
            while (sym != IntPtr.Zero)
            {
                string? key = ImsymKey(sym);
                if (key is not null && HasAttribute(key))
                {
                    string? value = GetAttribute(key);
                    if (!string.IsNullOrEmpty(value))
                    {
                        attributes[key] = value!;
                    }
                }
                sym = Agnxtattr(MyRootGraph._ptr, kind, sym);
            }
        }
        return attributes;
    }

    public override string ToString()
    {
        return $"Name: {GetName()}, root name: {MyRootGraph.GetName()}";
    }

    /// <summary>
    /// Copy attributes from one cgraph object to another.
    /// Throw argument exception if self and destination are not of the same type.
    /// Also copies attributes that haven't been introduced in the destination object,
    /// unless introduce_new_attrs is false.
    /// Return code indicates success or failure.
    /// </summary>
    public int CopyAttributesTo(CGraphThing destination, bool introduce_new_attrs = true)
    {
        if (!((this is Node && destination is Node)
            || (this is Edge && destination is Edge)
            || (this is Graph && destination is Graph)))
            throw new ArgumentException("Argument must be of the same type as self.");

        // Copying the attributes doesn't work if they have not been introduced in the graph
        // Moreover, the copying may just stop at some point if copying of a single attribute fails
        if (introduce_new_attrs && MyRootGraph._ptr != destination.MyRootGraph._ptr)
            CloneAttributeDeclarations(MyRootGraph._ptr, destination.MyRootGraph._ptr);

        // agcopyattr returns non-zero number on failure.

        // Problems have been observed while copying between edges (of the same graph)
        // Hypothesis: are the pointers different in this case?
        // Hypothesis 2: does it have something to do with the comment "Do not copy key attribute for edges,
        // as this must be distinct." in the graphviz source code?
        // Returncode 1 has been observed while copying rootgraphs, while the number of attributes was 0
        Debug.Assert(_ptr != destination._ptr);
        int success = Agcopyattr(_ptr, destination._ptr);
        // We implement a workaround for copying between edges
        if (success != 0)
        {
            // Fail for unknown failing cases
            if (GetType() != typeof(Edge) && GetAttributes().Count != 0)
                Debug.Fail("Copying attributes failed");

            // We work around this doing the following:
            var attrs = GetAttributes();
            foreach (var key in attrs.Keys)
                destination.SetAttribute(key, attrs[key]);
        }

        return 0;
    }

    /// <summary>
    /// Some characters and character sequences have a special meaning.
    /// If you intend to display a literal string, use this function to properly escape the string.
    /// See also
    /// https://www.graphviz.org/doc/info/shapes.html#record
    /// https://www.graphviz.org/doc/info/attrs.html#k:escString
    /// </summary>
    public static string EscapeLabel(string label)
    {
        // From the graphviz docs:
        // Braces, vertical bars and angle brackets must be escaped with a backslash character if
        // you wish them to appear as a literal character. Spaces are interpreted as separators
        // between tokens, so they must be escaped if you want spaces in the text.
        string result = label;
        foreach (char c in new[] { '\\', '<', '>', '{', '}', ' ', '|' })
        {
            result = result.Replace(c.ToString(), "\\" + c);
        }
        return result;
    }

    #region layout functions

    public bool HasPosition()
    {
        return HasAttribute("pos");
    }

    public void MakeInvisible()
    {
        SetAttribute("style", "invis");
    }

    public bool IsInvisible()
    {
        return GetAttribute("style") == "invis";
    }

    /// <summary>
    /// See documentation on <see cref="XDotOp"/>
    /// </summary>
    public IReadOnlyList<XDotOp> GetDrawing() => GetXDotValue(this, "_draw_");
    /// <summary>
    /// See documentation on <see cref="XDotOp"/>
    /// </summary>
    public IReadOnlyList<XDotOp> GetLabelDrawing() => GetXDotValue(this, "_ldraw_");

    protected List<XDotOp> GetXDotValue(CGraphThing obj, string attrName)
    {
        var xdotString = obj.SafeGetAttribute(attrName);
        if (xdotString is null)
            return new List<XDotOp>();

        return XDotParser.ParseXDot(xdotString, MyRootGraph.CoordinateSystem, MyRootGraph.RawMaxY());
    }

    protected static RectangleD ParseRect(string rect)
    {
        // Rectangles are anchored by their lower left and upper right points 
        // https://www.graphviz.org/docs/attr-types/rect/
        string[] points = rect.Split(',');
        var x = double.Parse(points[0], NumberStyles.Any, CultureInfo.InvariantCulture);
        var y = double.Parse(points[1], NumberStyles.Any, CultureInfo.InvariantCulture);
        var w = double.Parse(points[2], NumberStyles.Any, CultureInfo.InvariantCulture) - x;
        var h = double.Parse(points[3], NumberStyles.Any, CultureInfo.InvariantCulture) - y;
        return RectangleD.Create(x, y, w, h);
    }

    #endregion
}
