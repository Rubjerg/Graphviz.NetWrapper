namespace Rubjerg.Graphviz
{
    // FIXNOW: point to relevant documentation

    public record struct XDotColorStop
    {
        public float Frac { get; init; }
        public string Color { get; init; }
    }

    public record struct XDotLinearGrad
    {
        public double X0 { get; init; }
        public double Y0 { get; init; }
        public double X1 { get; init; }
        public double Y1 { get; init; }
        public int NStops { get; init; }
        public XDotColorStop[] Stops { get; init; }
    }

    public record struct XDotRadialGrad
    {
        public double X0 { get; init; }
        public double Y0 { get; init; }
        public double R0 { get; init; }
        public double X1 { get; init; }
        public double Y1 { get; init; }
        public double R1 { get; init; }
        public int NStops { get; init; }
        public XDotColorStop[] Stops { get; init; }
    }

    public abstract record class XDotColor
    {
        private XDotColor() { }
        public sealed record class Uniform : XDotColor
        {
            public string Color { get; init; }
        }
        public sealed record class LinearGradient : XDotColor
        {
            public XDotLinearGrad LinearGrad { get; init; }
        }
        public sealed record class RadialGradient : XDotColor
        {
            public XDotRadialGrad RadialGrad { get; init; }
        }
    }

    public enum XDotAlign
    {
        Left,
        Center,
        Right
    }

    public record struct XDotPoint
    {
        public double X { get; init; }
        public double Y { get; init; }
        public double Z { get; init; }
    }

    public record struct XDotRect
    {
        public double X { get; init; }
        public double Y { get; init; }
        public double Width { get; init; }
        public double Height { get; init; }
    }

    public record struct XDotPolyline
    {
        public int Count { get; init; }
        public XDotPoint[] Points { get; init; }
    }

    public record struct XDotText
    {
        public double X { get; init; }
        public double Y { get; init; }
        public XDotAlign Align { get; init; }
        public double Width { get; init; }
        public string Text { get; init; }
    }

    public record struct XDotImage
    {
        public XDotRect Pos { get; init; }
        public string Name { get; init; }
    }

    public record struct XDotFont
    {
        public double Size { get; init; }
        public string Name { get; init; }
    }

    public abstract record class XDotOp
    {
        private XDotOp() { }

        public sealed record class FilledEllipse : XDotOp
        {
            public XDotRect Ellipse { get; init; }
        }
        public sealed record class UnfilledEllipse : XDotOp
        {
            public XDotRect Ellipse { get; init; }
        }
        public sealed record class FilledPolygon : XDotOp
        {
            public XDotPolyline Polygon { get; init; }
        }
        public sealed record class UnfilledPolygon : XDotOp
        {
            public XDotPolyline Polygon { get; init; }
        }
        public sealed record class PolyLine : XDotOp
        {
            public XDotPolyline Polyline { get; init; }
        }
        public sealed record class FilledBezier : XDotOp
        {
            public XDotPolyline Bezier { get; init; }
        }
        public sealed record class UnfilledBezier : XDotOp
        {
            public XDotPolyline Bezier { get; init; }
        }
        public sealed record class Text : XDotOp
        {
            public XDotText Value { get; init; }
        }
        public sealed record class Image : XDotOp
        {
            public XDotImage Value { get; init; }
        }
        public sealed record class FillColor : XDotOp
        {
            public string Color { get; init; }
        }
        public sealed record class PenColor : XDotOp
        {
            public string Color { get; init; }
        }
        public sealed record class GradFillColor : XDotOp
        {
            public XDotColor GradColor { get; init; }
        }
        public sealed record class GradPenColor : XDotOp
        {
            public XDotColor GradColor { get; init; }
        }
        public sealed record class Font : XDotOp
        {
            public XDotFont Value { get; init; }
        }
        public sealed record class Style : XDotOp
        {
            public string Value { get; init; }
        }
        public sealed record class FontChar : XDotOp
        {
            public uint Value { get; init; }
        }
    }
}
