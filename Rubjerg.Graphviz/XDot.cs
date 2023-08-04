using System;
using System.Drawing;

namespace Rubjerg.Graphviz
{
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

    public abstract record class XDotGradColor
    {
        private XDotGradColor() { }
        public sealed record class Uniform : XDotGradColor
        {
            public string Color { get; init; }
        }
        public sealed record class LinearGradient : XDotGradColor
        {
            public XDotLinearGrad LinearGrad { get; init; }
        }
        public sealed record class RadialGradient : XDotGradColor
        {
            public XDotRadialGrad RadialGrad { get; init; }
        }
    }

    public record struct XDotPoint
    {
        public double X { get; init; }
        public double Y { get; init; }
        public double Z { get; init; }

        public PointF ToPointF()
        {
            return new PointF()
            {
                X = (float)X,
                Y = (float)Y,
            };
        }
    }

    public record struct XDotRect
    {
        public double X { get; init; }
        public double Y { get; init; }
        public double Width { get; init; }
        public double Height { get; init; }

        public RectangleF ToRectangleF()
        {
            return new RectangleF()
            {
                X = (float)X,
                Y = (float)Y,
                Width = (float)Width,
                Height = (float)Height,
            };
        }
    }

    public record struct XDotPolyline
    {
        public int Count { get; init; }
        public XDotPoint[] Points { get; init; }
    }

    public enum XDotAlign
    {
        Left,
        Center,
        Right
    }

    public record struct XDotText
    {
        public double X { get; init; }
        public double Y { get; init; }
        public XDotAlign Align { get; init; }
        public double Width { get; init; }
        public string Text { get; init; }

        /// <summary>
        /// Compute the bounding box of this text element given the earlier specified font
        /// </summary>
        public RectangleF TextBoundingBox(XDotFont font)
        {
            return new RectangleF()
            {
                X = (float)X,
                Y = (float)Y,
                Width = (float)Width,
                Height = (float)font.Size,
            };
        }
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

    [Flags]
    public enum XDotFontChar
    {
        None = 0,
        Bold = 1,
        Italic = 2,
        Underline = 4,
        Superscript = 8,
        Subscript = 16,
        StrikeThrough = 32,
        Overline = 64,
    }

    /// <summary>
    /// See https://graphviz.org/docs/outputs/canon/#xdot for semantics
    /// </summary>
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
            public XDotGradColor GradColor { get; init; }
        }
        public sealed record class GradPenColor : XDotOp
        {
            public XDotGradColor GradColor { get; init; }
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
            public XDotFontChar Value { get; init; }
        }
    }
}
