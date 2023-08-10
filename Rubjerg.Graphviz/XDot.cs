using System;
using System.Linq;

namespace Rubjerg.Graphviz;

// See https://graphviz.org/docs/outputs/canon/#xdot

/// <summary>
/// In Graphviz, the default coordinate system has the origin on the bottom left.
/// Many rendering applications use a coordinate system with the origin at the top left.
/// </summary>
public enum CoordinateSystem
{
    BottomLeft = 0,
    TopLeft = 1,
}

public record struct SizeD(double Width, double Height);

public record struct PointD(double X, double Y)
{
    internal PointD ForCoordSystem(CoordinateSystem coordSystem, double maxY)
    {
        if (coordSystem == CoordinateSystem.BottomLeft)
            return this;
        return new PointD(X, maxY - Y);
    }
}

/// <param name="Point">The point closest to the origin</param>
/// <param name="Size"></param>
public record struct RectangleD(PointD Point, SizeD Size)
{
    public double X => Point.X; 
    public double Y => Point.Y;
    public double Width => Size.Width;
    public double Height => Size.Height;

    /// <summary>The point farthest from the origin</summary>
    public PointD FarPoint() => new PointD(Point.X + Size.Width, Point.Y + Size.Height);
    public double MidX() => X + Width / 2;
    public double MidY() => Y + Height / 2;
    public PointD Center() => new PointD(MidX(), MidY());

    public static RectangleD Create(double x, double y, double width, double height)
    {
        return new RectangleD(new PointD(x, y), new SizeD(width, height));
    }

    internal RectangleD ForCoordSystem(CoordinateSystem coordSystem, double maxY)
    {
        return this with
        {
            Point = Point.ForCoordSystem(coordSystem, maxY),
        };
    }
}

public record struct ColorStop(float Frac, string HtmlColor);

public record struct LinearGradient(PointD Point0, PointD Point1, ColorStop[] Stops)
{
    internal LinearGradient ForCoordSystem(CoordinateSystem coordSystem, double maxY)
    {
        return this with
        {
            Point0 = Point0.ForCoordSystem(coordSystem, maxY),
            Point1 = Point1.ForCoordSystem(coordSystem, maxY),
        };
    }
}

public record struct RadialGradient(PointD Point0, double Radius0, PointD Point1, double Radius1, ColorStop[] Stops)
{
    internal RadialGradient ForCoordSystem(CoordinateSystem coordSystem, double maxY)
    {
        return this with
        {
            Point0 = Point0.ForCoordSystem(coordSystem, maxY),
            Point1 = Point1.ForCoordSystem(coordSystem, maxY),
        };
    }
}

public abstract record class Color
{
    private Color() { }
    public sealed record class Uniform(string HtmlColor) : Color { }
    public sealed record class Linear(LinearGradient Gradient) : Color { }
    public sealed record class Radial(RadialGradient Gradient) : Color { }
}

public enum TextAlign
{
    Left,
    Center,
    Right
}

/// <summary>
/// Represents a line of text to be drawn.
/// Labels with multiple lines will be represented by multiple <see cref="TextInfo"/> instances.
/// </summary>
/// <param name="Anchor">
/// The y-coordinate points to the baseline,
/// the x-coordinate points to the horizontal position relative to which the text should be
/// aligned according to the <see cref="Align"/> property.
/// </param>
/// <param name="Align">How the text should be aligned horizontally, relative to the given anchor point.</param>
/// <param name="Width">The estimated width of the text.</param>
/// <param name="Text"></param>
/// <param name="Font"></param>
public record struct TextInfo(PointD Anchor, TextAlign Align, double Width, string Text, Font Font, FontChar FontChar)
{
    /// <summary>
    /// Compute the bounding box of this text element given the necessary font information.
    /// </summary>
    /// <param name="font">Font used to draw the text</param>
    /// <param name="distanceBetweenBaselineAndDescender">Optional property of the font, to more accurately predict the bounding box.</param>
    public RectangleD TextBoundingBox(double? distanceBetweenBaselineAndDescender = null)
    {
        // FIXNOW: better estimate distanceBetweenBaselineAndDescender ?
        var size = TextSize();
        var descenderY = Anchor.Y - (distanceBetweenBaselineAndDescender ?? Font.Size / 5);
        var leftX = Align switch
        {
            TextAlign.Left => Anchor.X,
            TextAlign.Center => Anchor.X + size.Width / 2,
            TextAlign.Right => Anchor.X + size.Width,
            _ => throw new InvalidOperationException()
        };
        var bottomLeft = new PointD(leftX, descenderY);
        return new RectangleD(bottomLeft, size);
    }

    /// <summary>
    /// The width represents the estimated width of the text by GraphViz.
    /// The height represents the font size, which is usually the distance between the ascender and the descender
    /// of the font.
    /// </summary>
    public SizeD TextSize() => new SizeD(Width, Font.Size);

    internal TextInfo ForCoordSystem(CoordinateSystem coordSystem, double maxY)
    {
        // FIXNOW
        // While things like rectangles are anchored by the point closest to the origin,
        // the y-coordinate of a text object anchor always points to the baseline of the text.
        // This means we have to take extra care when transforming to the top-left coordinate system.
        return this with
        {
            Anchor = Anchor.ForCoordSystem(coordSystem, maxY),
        };
    }
}

public record struct ImageInfo(RectangleD Position, string Name)
{
    internal ImageInfo ForCoordSystem(CoordinateSystem coordSystem, double maxY)
    {
        return this with
        {
            Position = Position.ForCoordSystem(coordSystem, maxY),
        };
    }
}

/// <param name="Size">Font size in points</param>
/// <param name="Name">Font name</param>
public record struct Font(double Size, string Name)
{
    public static Font Default => new() { Size = 14, Name = "Times-Roman" };
}

[Flags]
public enum FontChar
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

internal static class PointDArrayExtension
{
    internal static PointD[] ForCoordSystem(this PointD[] self, CoordinateSystem coordSystem, double maxY)
    {
        return self.Select(a => a.ForCoordSystem(coordSystem, maxY)).ToArray();
    }
}

/// <summary>
/// See https://graphviz.org/docs/outputs/canon/#xdot for semantics.
/// 
/// Within the context of a single drawing attribute, e.g., draw, there is an implicit state for the
/// graphical attributes. That is, once a color or style is set, it remains valid for all relevant
/// drawing operations until the value is reset by another xdot cmd.
/// 
/// Note that the filled figures (ellipses, polygons and B-Splines) imply two operations: first,
/// drawing the filled figure with the current fill color; second, drawing an unfilled figure with
/// the current pen color, pen width and pen style.
/// 
/// The text operation is only used in the label attributes. Normally, the non-text operations are
/// only used in the non-label attributes. If, however, the decorate attribute is set on an edge,
/// its label attribute will also contain a polyline operation. In addition, if a label is a
/// complex, HTML-like label, it will also contain non-text operations.
/// 
/// NOTE: we've slightly trimmed down the number of cases w.r.t. the actual xdot operations.
/// All font related operations have been condensed into the text operations.
/// We only have a single Color type, which has three subtypes.
/// </summary>
public abstract record class XDotOp
{
    private XDotOp() { }

    public sealed record class FilledEllipse(RectangleD Value) : XDotOp { }
    public sealed record class UnfilledEllipse(RectangleD Value) : XDotOp { }
    public sealed record class FilledPolygon(PointD[] Points) : XDotOp { }
    public sealed record class UnfilledPolygon(PointD[] Points) : XDotOp { }
    public sealed record class PolyLine(PointD[] Points) : XDotOp { }
    public sealed record class FilledBezier(PointD[] Points) : XDotOp { }
    public sealed record class UnfilledBezier(PointD[] Points) : XDotOp { }
    public sealed record class Text(TextInfo Value) : XDotOp { }
    public sealed record class Image(ImageInfo Value) : XDotOp { }
    public sealed record class FillColor(Color Value) : XDotOp { }
    public sealed record class PenColor(Color Value) : XDotOp { }
    /// <summary>
    /// Style values which can be incorporated in the graphics model do not appear in xdot
    /// output. In particular, the style values filled, rounded, diagonals, and invis will not
    /// appear. Indeed, if style contains invis, there will not be any xdot output at all.
    /// For reference see https://graphviz.org/docs/attr-types/style/
    /// </summary>
    public sealed record class Style(string Value) : XDotOp { }
}
