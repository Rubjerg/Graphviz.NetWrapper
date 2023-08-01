using System;

namespace Rubjerg.Graphviz
{
    public class XDotGraph : RootGraph
    {
        private XDotGraph(IntPtr ptr) : base(ptr) { }

        internal static new XDotGraph FromDotString(string graph)
        {
            return FromDotString(graph, ptr => new XDotGraph(ptr));
        }

        public XDot GetDraw() => GetXDotValue(this, "_draw_");
        public XDot GetDraw(Node node) => GetXDotValue(node, "_draw_");
        public XDot GetDraw(Edge edge) => GetXDotValue(edge, "_draw_");

        private static XDot GetXDotValue(CGraphThing obj, string attrName)
        {
            var xdotString = obj.SafeGetAttribute(attrName, null);
            if (xdotString is null)
                throw new InvalidOperationException("Attribute not available");

            IntPtr xdot = XDotFFI.parseXDot(xdotString);
            try
            {
                return XDotTranslator.TranslateXDot(xdot);
            }
            finally
            {
                if (xdot != IntPtr.Zero)
                {
                    XDotFFI.freeXDot(xdot);
                }
            }
        }
    }
}
