using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using static Rubjerg.Graphviz.ForeignFunctionInterface;

namespace Rubjerg.Graphviz
{
    // NOTE ABOUT ATTRIBUTES:
    // Cgraph assumes that all objects of a given kind(graphs/subgraphs, nodes, or edges) have the same attributes
    // - there’s no notion of subtyping within attributes. Information about attributes is stored in data dictionaries.
    // Each graph has three (for graphs/subgraphs, nodes, and edges) for which you'll need the predefined constants
    // AGRAPH, AGNODE and AGEDGE in calls to create, search and walk these dictionaries.
    public abstract class CGraphThing : GraphVizThing
    {
        public readonly RootGraph MyRootGraph;

        /// <summary>
        /// Argument root may be null.
        /// In that case, it is assumed this is a RootGraph, and MyRootGraph is set to `this`.
        /// </summary>
        internal CGraphThing(IntPtr ptr, RootGraph root) : base(ptr)
        {
            if (root == null)
                MyRootGraph = (RootGraph)this;
            else
                MyRootGraph = root;
        }

        public bool HasAttribute(string name)
        {
            return !new[] { null, "" }.Contains(GetAttribute(name));
        }

        /// <summary>
        /// Also works if the attribute has not been introduced for this kind.
        /// </summary>
        public void SafeSetAttribute(string name, string value, string deflt)
        {
            Agsafeset(_ptr, name, value, deflt);
        }

        /// <summary>
        /// Precondition: the attribute has been introduced for this kind.
        /// </summary>
        public void SetAttribute(string name, string value)
        {
            Agset(_ptr, name, value);
        }

        /// <summary>
        /// Precondition: the attribute has been introduced for this kind.
        /// </summary>
        public string GetAttribute(string name)
        {
            return Imagget(_ptr, name);
        }

        /// <summary>
        /// Get the attribute if it was introduced, otherwise return deflt.
        /// </summary>
        public string SafeGetAttribute(string name, string deflt)
        {
            if (HasAttribute(name))
                return GetAttribute(name);
            return deflt;
        }

        /// <summary>
        /// Get all attributes as dictionary.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetAttributes()
        {
            Dictionary<string, string> attributes = new Dictionary<string, string>();
            for (int kind = 0; kind < 3; ++kind)
            {
                IntPtr sym = Agnxtattr(MyRootGraph._ptr, kind, IntPtr.Zero);
                while (sym != IntPtr.Zero)
                {
                    string key = ImsymKey(sym);
                    if (HasAttribute(key))
                    {
                        string value = GetAttribute(key);
                        if (!string.IsNullOrEmpty(value))
                        {
                            attributes[key] = value;
                        }
                    }
                    sym = Agnxtattr(MyRootGraph._ptr, kind, sym);
                }
            }
            return attributes;
        }

        public string GetName()
        {
            return Imagnameof(_ptr);
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

        public Color GetColor()
        {
            string colorstring = SafeGetAttribute("color", "Black");
            return Color.FromName(colorstring);
        }

        public bool HasPosition()
        {
            return HasAttribute("pos");
        }

        public void MakeInvisible()
        {
            SafeSetAttribute("style", "invis", "normal");
        }

        public void MakeInvisibleAndSmall()
        {
            SafeSetAttribute("style", "invis", "normal");
            SafeSetAttribute("margin", "0", "");
            SafeSetAttribute("width", "0", "");
            SafeSetAttribute("height", "0", "");
        }

        public bool IsInvisible()
        {
            return SafeGetAttribute("style", "normal") == "invis";
        }
    }
}
