using System;
using System.Drawing;

namespace Rubjerg.Graphviz
{
    // See https://graphviz.org/docs/outputs/canon/#xdot

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

        public PointF ToPointF() => new PointF((float)X, (float)Y);
    }

    public record struct XDotRect
    {
        public double X { get; init; }
        public double Y { get; init; }
        public double Width { get; init; }
        public double Height { get; init; }

        public RectangleF ToRectangleF() => new RectangleF((float)X, (float)Y, (float)Width, (float)Height);
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

    /// <summary>
    /// Represents a line of text to be drawn.
    /// Labels with multiple lines will be represented by multiple <see cref="XDotText"/> instances.
    /// </summary>
    public record struct XDotText
    {
        /// <summary>
        /// The X coordinate of the anchor point of the text.
        /// </summary>
        public double X { get; init; }
        /// <summary>
        /// The Y coordinate of the baseline of the text.
        /// </summary>
        public double Y { get; init; }
        /// <summary>
        /// How the text should be aligned, relative to the given anchor point.
        /// </summary>
        public XDotAlign Align { get; init; }
        public double Width { get; init; }
        public string Text { get; init; }

        /// <summary>
        /// Compute the bounding box of this text element given the necessary font information.
        /// </summary>
        /// <param name="font">Font used to draw the text</param>
        /// <param name="distanceBetweenBaselineAndDescender">Optional property of the font, to more accurately predict the bounding box.</param>
        public RectangleF TextBoundingBox(XDotFont font, float? distanceBetweenBaselineAndDescender = null)
        {
            var size = Size(font);
            var descenderY = Y - (distanceBetweenBaselineAndDescender ?? font.Size / 5);
            var leftX = Align switch
            {
                XDotAlign.Left => X,
                XDotAlign.Center => X + size.Width / 2,
                XDotAlign.Right => X + size.Width,
                _ => throw new InvalidOperationException()
            };
            var bottomLeft = new PointF((float)leftX, (float)descenderY);
            return new RectangleF(bottomLeft, size);
        }

        /// <summary>
        /// The anchor point of the text.
        /// The Y coordinate points to the baseline of the text.
        /// The X coordinate points to the horizontal anchor of the text.
        /// </summary>
        public PointF Anchor() => new PointF((float)X, (float)Y);

        /// <summary>
        /// The width represents the estimated width of the text by GraphViz.
        /// The height represents the font size, which is usually the distance between the ascender and the descender
        /// of the font.
        /// </summary>
        public SizeF Size(XDotFont font) => new SizeF((float)Width, (float)font.Size);
    }

    public record struct XDotImage
    {
        public XDotRect Pos { get; init; }
        public string Name { get; init; }
    }

    public record struct XDotFont
    {
        /// <summary>
        /// Size in points
        /// </summary>
        public double Size { get; init; }
        public string Name { get; init; }
        public static XDotFont Default => new() { Size = 14, Name = "Times-Roman" };
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
            public XDotRect Value { get; init; }
        }
        public sealed record class UnfilledEllipse : XDotOp
        {
            public XDotRect Value { get; init; }
        }
        public sealed record class FilledPolygon : XDotOp
        {
            public XDotPolyline Value { get; init; }
        }
        public sealed record class UnfilledPolygon : XDotOp
        {
            public XDotPolyline Value { get; init; }
        }
        public sealed record class PolyLine : XDotOp
        {
            public XDotPolyline Value { get; init; }
        }
        public sealed record class FilledBezier : XDotOp
        {
            public XDotPolyline Value { get; init; }
        }
        public sealed record class UnfilledBezier : XDotOp
        {
            public XDotPolyline Value { get; init; }
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
            public string Value { get; init; }
        }
        public sealed record class PenColor : XDotOp
        {
            public string Value { get; init; }
        }
        public sealed record class GradFillColor : XDotOp
        {
            public XDotGradColor Value { get; init; }
        }
        public sealed record class GradPenColor : XDotOp
        {
            public XDotGradColor Value { get; init; }
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
