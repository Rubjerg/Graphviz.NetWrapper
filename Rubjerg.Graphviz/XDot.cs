namespace Rubjerg.Graphviz
{
    // FIXNOW: point to relevant documentation

    public struct XDotColorStop
    {
        public float Frac { get; set; }
        public string Color { get; set; }
    }

    public struct XDotLinearGrad
    {
        public double X0 { get; set; }
        public double Y0 { get; set; }
        public double X1 { get; set; }
        public double Y1 { get; set; }
        public int NStops { get; set; }
        public XDotColorStop[] Stops { get; set; }
    }

    public struct XDotRadialGrad
    {
        public double X0 { get; set; }
        public double Y0 { get; set; }
        public double R0 { get; set; }
        public double X1 { get; set; }
        public double Y1 { get; set; }
        public double R1 { get; set; }
        public int NStops { get; set; }
        public XDotColorStop[] Stops { get; set; }
    }

    public abstract class XDotColor
    {
        private XDotColor() { }
        public sealed class Uniform : XDotColor
        {
            public string Color { get; set; }
        }
        public sealed class LinearGradient : XDotColor
        {
            public XDotLinearGrad LinearGrad { get; set; }
        }
        public sealed class RadialGradient : XDotColor
        {
            public XDotRadialGrad RadialGrad { get; set; }
        }
    }

    public enum XDotAlign
    {
        Left,
        Center,
        Right
    }

    public struct XDotPoint
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
    }

    public struct XDotRect
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
    }

    public struct XDotPolyline
    {
        public int Count { get; set; }
        public XDotPoint[] Points { get; set; }
    }

    public struct XDotText
    {
        public double X { get; set; }
        public double Y { get; set; }
        public XDotAlign Align { get; set; }
        public double Width { get; set; }
        public string Text { get; set; }
    }

    public struct XDotImage
    {
        public XDotRect Pos { get; set; }
        public string Name { get; set; }
    }

    public struct XDotFont
    {
        public double Size { get; set; }
        public string Name { get; set; }
    }

    public abstract class XDotOp
    {
        private XDotOp() { }
        // We define a custom structure for each kind of operation
        // and a custom field in the union for each structure
        public sealed class FilledEllipse : XDotOp
        {
            public XDotRect Ellipse { get; set; }      // FilledEllipse, UnfilledEllipse
        }
        public sealed class UnfilledEllipse : XDotOp
        {
            public XDotRect Ellipse { get; set; }      // FilledEllipse, UnfilledEllipse
        }
        public sealed class FilledPolygon : XDotOp
        {
            public XDotPolyline Polygon { get; set; }  // FilledPolygon, UnfilledPolygon
        }
        public sealed class UnfilledPolygon : XDotOp
        {
            public XDotPolyline Polygon { get; set; }  // FilledPolygon, UnfilledPolygon
        }
        public sealed class PolyLine : XDotOp
        {
            public XDotPolyline Polyline { get; set; } // Polyline
        }
        public sealed class FilledBezier : XDotOp
        {
            public XDotPolyline Bezier { get; set; }   // FilledBezier, UnfilledBezier
        }
        public sealed class UnfilledBezier : XDotOp
        {
            public XDotPolyline Bezier { get; set; }   // FilledBezier, UnfilledBezier
        }
        public sealed class Text : XDotOp
        {
            public XDotText Value { get; set; }         // Text
        }
        public sealed class Image : XDotOp
        {
            public XDotImage Value { get; set; }       // Image
        }
        public sealed class FillColor : XDotOp
        {
            public string Color { get; set; }          // FillColor, PenColor
        }
        public sealed class PenColor : XDotOp
        {
            public string Color { get; set; }          // FillColor, PenColor
        }
        public sealed class GradFillColor : XDotOp
        {
            public XDotColor GradColor { get; set; }   // GradFillColor, GradPenColor
        }
        public sealed class GradPenColor : XDotOp
        {
            public XDotColor GradColor { get; set; }   // GradFillColor, GradPenColor
        }
        public sealed class Font : XDotOp
        {
            public XDotFont Value { get; set; }         // Font
        }
        public sealed class Style : XDotOp
        {
            public string Value { get; set; }          // Style
        }
        public sealed class FontChar : XDotOp
        {
            public uint Value { get; set; }         // FontChar
        }
    }
}
