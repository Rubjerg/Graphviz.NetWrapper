using System;
using System.Collections.Generic;

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

        public List<XDotOp> GetDrawing() => GetXDotValue(this, "_draw_");
        public List<XDotOp> GetDrawing(Node node) => GetXDotValue(node, "_draw_");
        public List<XDotOp> GetDrawing(Edge edge) => GetXDotValue(edge, "_draw_");
        public List<XDotOp> GetLabelDrawing() => GetXDotValue(this, "_ldraw_");
        public List<XDotOp> GetLabelDrawing(Node node) => GetXDotValue(node, "_ldraw_");
        public List<XDotOp> GetLabelDrawing(Edge edge) => GetXDotValue(edge, "_ldraw_");
        public List<XDotOp> GetHeadArrowDrawing(Edge edge) => GetXDotValue(edge, "_hdraw_");
        public List<XDotOp> GetTailArrowDrawing(Edge edge) => GetXDotValue(edge, "_tdraw_");
        public List<XDotOp> GetHeadLabelDrawing(Edge edge) => GetXDotValue(edge, "_hldraw_");
        public List<XDotOp> GetTailLabelDrawing(Edge edge) => GetXDotValue(edge, "_tldraw_");

        private static List<XDotOp> GetXDotValue(CGraphThing obj, string attrName)
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
