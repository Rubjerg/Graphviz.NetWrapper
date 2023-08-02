using System;

namespace Rubjerg.Graphviz
{
    /// <summary>
    /// See https://graphviz.org/docs/outputs/canon/#xdot for specs
    /// </summary>
    public class XDotGraph : RootGraph
    {
        private XDotGraph(IntPtr ptr) : base(ptr) { }

        internal static new XDotGraph FromDotString(string graph)
        {
            return FromDotString(graph, ptr => new XDotGraph(ptr));
        }

        public XDot GetDrawing() => GetXDotValue(this, "_draw_");
        public XDot GetDrawing(Node node) => GetXDotValue(node, "_draw_");
        public XDot GetDrawing(Edge edge) => GetXDotValue(edge, "_draw_");
        public XDot GetLabelDrawing() => GetXDotValue(this, "_ldraw_");
        public XDot GetLabelDrawing(Node node) => GetXDotValue(node, "_ldraw_");
        public XDot GetLabelDrawing(Edge edge) => GetXDotValue(edge, "_ldraw_");
        public XDot GetHeadArrowDrawing(Edge edge) => GetXDotValue(edge, "_hdraw_");
        public XDot GetTailArrowDrawing(Edge edge) => GetXDotValue(edge, "_tdraw_");
        public XDot GetHeadLabelDrawing(Edge edge) => GetXDotValue(edge, "_hldraw_");
        public XDot GetTailLabelDrawing(Edge edge) => GetXDotValue(edge, "_tldraw_");

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
