using System;
using System.Collections.Generic;
using System.Linq;

namespace Rubjerg.Graphviz.FFI;

// These internal types are only used for marshaling
// We replace them with more idiomatic types
internal enum XDotGradType
{
    None,
    Linear,
    Radial
}

internal enum XDotKind
{
    FilledEllipse, UnfilledEllipse,
    FilledPolygon, UnfilledPolygon,
    FilledBezier, UnfilledBezier,
    Polyline, Text,
    FillColor, PenColor, Font, Style, Image,
    GradFillColor, GradPenColor,
    FontChar
}

internal struct XDot
{
    public int Count { get; set; }     // Number of xdot ops
    public XDotOp[] Ops { get; set; }  // xdot operations
}

internal static class XDotParser
{
    public static List<XDotOp> ParseXDot(string xdotString, CoordinateSystem coordinateSystem, double maxY)
    {
        IntPtr xdot = XDotFFI.ParseXDot(xdotString);
        try
        {
            return TranslateXDot(xdot, coordinateSystem, maxY);
        }
        finally
        {
            if (xdot != IntPtr.Zero)
            {
                XDotFFI.FreeXDot(xdot);
            }
        }
    }

    internal static List<XDotOp> TranslateXDot(IntPtr xdotPtr, CoordinateSystem coordinateSystem, double maxY)
    {
        if (xdotPtr == IntPtr.Zero)
            throw new ArgumentNullException(nameof(xdotPtr));

        XDot xdot = new XDot
        {
            Count = (int)GraphvizWrapperLib.get_cnt(xdotPtr)
        };

        // Translate the array of XDotOps
        int count = xdot.Count;
        xdot.Ops = new XDotOp[count];
        var opsPtr = GraphvizWrapperLib.get_ops(xdotPtr);

        var activeFont = Font.Default;
        var activeFontChar = FontChar.None;
        for (int i = 0; i < count; ++i)
        {
            IntPtr xdotOpPtr = GraphvizWrapperLib.get_op_at_index(opsPtr, i);
            var kind = GraphvizWrapperLib.get_kind(xdotOpPtr);
            switch (kind)
            {
                case XDotKind.FilledEllipse:
                    xdot.Ops[i] = new XDotOp.FilledEllipse(TranslateEllipse(GraphvizWrapperLib.get_ellipse(xdotOpPtr))
                        .ForCoordSystem(coordinateSystem, maxY));
                    break;
                case XDotKind.UnfilledEllipse:
                    xdot.Ops[i] = new XDotOp.UnfilledEllipse(TranslateEllipse(GraphvizWrapperLib.get_ellipse(xdotOpPtr))
                        .ForCoordSystem(coordinateSystem, maxY));
                    break;
                case XDotKind.FilledPolygon:
                    xdot.Ops[i] = new XDotOp.FilledPolygon(TranslatePolyline(GraphvizWrapperLib.get_polyline(xdotOpPtr))
                        .ForCoordSystem(coordinateSystem, maxY));
                    break;
                case XDotKind.UnfilledPolygon:
                    xdot.Ops[i] = new XDotOp.FilledPolygon(TranslatePolyline(GraphvizWrapperLib.get_polyline(xdotOpPtr))
                        .ForCoordSystem(coordinateSystem, maxY));
                    break;
                case XDotKind.FilledBezier:
                    xdot.Ops[i] = new XDotOp.FilledBezier(TranslatePolyline(GraphvizWrapperLib.get_polyline(xdotOpPtr))
                        .ForCoordSystem(coordinateSystem, maxY));
                    break;
                case XDotKind.UnfilledBezier:
                    xdot.Ops[i] = new XDotOp.UnfilledBezier(TranslatePolyline(GraphvizWrapperLib.get_polyline(xdotOpPtr))
                        .ForCoordSystem(coordinateSystem, maxY));
                    break;
                case XDotKind.Polyline:
                    xdot.Ops[i] = new XDotOp.PolyLine(TranslatePolyline(GraphvizWrapperLib.get_polyline(xdotOpPtr))
                        .ForCoordSystem(coordinateSystem, maxY));
                    break;
                case XDotKind.Text:
                    xdot.Ops[i] = new XDotOp.Text(TranslateText(GraphvizWrapperLib.get_text(xdotOpPtr), activeFont, activeFontChar)
                        .ForCoordSystem(coordinateSystem, maxY));
                    break;
                case XDotKind.FillColor:
                    xdot.Ops[i] = new XDotOp.FillColor(new Color.Uniform(GraphvizWrapperLib.GetColor(xdotOpPtr)!));
                    break;
                case XDotKind.PenColor:
                    xdot.Ops[i] = new XDotOp.PenColor(new Color.Uniform(GraphvizWrapperLib.GetColor(xdotOpPtr)!));
                    break;
                case XDotKind.GradFillColor:
                    xdot.Ops[i] = new XDotOp.FillColor(TranslateGradColor(GraphvizWrapperLib.get_grad_color(xdotOpPtr)));
                    break;
                case XDotKind.GradPenColor:
                    xdot.Ops[i] = new XDotOp.PenColor(TranslateGradColor(GraphvizWrapperLib.get_grad_color(xdotOpPtr)));
                    break;
                case XDotKind.Font:
                    activeFont = TranslateFont(GraphvizWrapperLib.get_font(xdotOpPtr));
                    break;
                case XDotKind.Style:
                    xdot.Ops[i] = new XDotOp.Style(GraphvizWrapperLib.GetStyle(xdotOpPtr)!);
                    break;
                case XDotKind.Image:
                    xdot.Ops[i] = new XDotOp.Image(TranslateImage(GraphvizWrapperLib.get_image(xdotOpPtr))
                        .ForCoordSystem(coordinateSystem, maxY));
                    break;
                case XDotKind.FontChar:
                    activeFontChar = TranslateFontChar(GraphvizWrapperLib.get_fontchar(xdotOpPtr));
                    break;
                default:
                    throw new ArgumentException($"Unexpected XDotOp.Kind: {kind}");
            }
        }

        return xdot.Ops.ToList();
    }

    private static FontChar TranslateFontChar(uint value)
    {
        return (FontChar)(int)value;
    }

    private static ImageInfo TranslateImage(IntPtr imagePtr)
    {
        ImageInfo image = new ImageInfo
        (
            Position: TranslateRect(GraphvizWrapperLib.get_pos(imagePtr)),
            Name: GraphvizWrapperLib.GetNameImage(imagePtr)
        );

        return image;
    }

    private static Font TranslateFont(IntPtr fontPtr)
    {
        Font font = new Font
        (
            Size: GraphvizWrapperLib.get_size(fontPtr),
            Name: GraphvizWrapperLib.GetNameFont(fontPtr)!
        );

        return font;
    }

    private static RectangleD TranslateEllipse(IntPtr ellipsePtr)
    {
        RectangleD ellipse = RectangleD.Create
        (
            GraphvizWrapperLib.get_x_rect(ellipsePtr),
            GraphvizWrapperLib.get_y_rect(ellipsePtr),
            GraphvizWrapperLib.get_w_rect(ellipsePtr),
            GraphvizWrapperLib.get_h_rect(ellipsePtr)
        );

        return ellipse;
    }

    private static Color TranslateGradColor(IntPtr colorPtr)
    {
        var type = GraphvizWrapperLib.get_type(colorPtr);
        switch (type)
        {
            case XDotGradType.None:
                return new Color.Uniform(GraphvizWrapperLib.GetClr(colorPtr)!);
            case XDotGradType.Linear:
                return new Color.Linear(TranslateLinearGrad(GraphvizWrapperLib.get_ling(colorPtr)));
            case XDotGradType.Radial:
                return new Color.Radial(TranslateRadialGrad(GraphvizWrapperLib.get_ring(colorPtr)));
            default:
                throw new ArgumentException($"Unexpected XDotColor.Type: {type}");
        }
    }

    private static LinearGradient TranslateLinearGrad(IntPtr lingPtr)
    {
        int count = GraphvizWrapperLib.get_n_stops_ling(lingPtr);
        LinearGradient linearGrad = new LinearGradient
        (
            Point0: new PointD(GraphvizWrapperLib.get_x0_ling(lingPtr), GraphvizWrapperLib.get_y0_ling(lingPtr)),
            Point1: new PointD(GraphvizWrapperLib.get_x1_ling(lingPtr), GraphvizWrapperLib.get_y1_ling(lingPtr)),
            Stops: new ColorStop[count]
        );

        // Translate the array of ColorStops
        var stopsPtr = GraphvizWrapperLib.get_stops_ling(lingPtr);
        for (int i = 0; i < count; ++i)
        {
            IntPtr colorStopPtr = GraphvizWrapperLib.get_color_stop_at_index(stopsPtr, i);
            linearGrad.Stops[i] = TranslateColorStop(colorStopPtr);
        }

        return linearGrad;
    }

    private static RadialGradient TranslateRadialGrad(IntPtr ringPtr)
    {
        int count = GraphvizWrapperLib.get_n_stops_ring(ringPtr);
        RadialGradient radialGrad = new RadialGradient
        (
            Point0: new PointD(GraphvizWrapperLib.get_x0_ring(ringPtr), GraphvizWrapperLib.get_y0_ring(ringPtr)),
            Point1: new PointD(GraphvizWrapperLib.get_x1_ring(ringPtr), GraphvizWrapperLib.get_y1_ring(ringPtr)),
            Radius0: GraphvizWrapperLib.get_r0_ring(ringPtr),
            Radius1: GraphvizWrapperLib.get_r1_ring(ringPtr),
            Stops: new ColorStop[count]
        );

        // Translate the array of ColorStops
        var stopsPtr = GraphvizWrapperLib.get_stops_ring(ringPtr);
        for (int i = 0; i < count; ++i)
        {
            IntPtr colorStopPtr = GraphvizWrapperLib.get_color_stop_at_index(stopsPtr, i);
            radialGrad.Stops[i] = TranslateColorStop(colorStopPtr);
        }

        return radialGrad;
    }

    private static ColorStop TranslateColorStop(IntPtr stopPtr)
    {
        ColorStop colorStop = new ColorStop
        (
            Frac: GraphvizWrapperLib.get_frac(stopPtr),
            HtmlColor: GraphvizWrapperLib.GetColorStop(stopPtr)!
        );

        return colorStop;
    }

    private static PointD[] TranslatePolyline(IntPtr polylinePtr)
    {
        int count = (int)GraphvizWrapperLib.get_cnt_polyline(polylinePtr);
        var points = new PointD[count];

        // Translate the array of Points
        var pointsPtr = GraphvizWrapperLib.get_pts_polyline(polylinePtr);
        for (int i = 0; i < count; ++i)
        {
            IntPtr pointPtr = GraphvizWrapperLib.get_pt_at_index(pointsPtr, i);
            points[i] = TranslatePoint(pointPtr);
        }

        return points;
    }

    private static PointD TranslatePoint(IntPtr pointPtr)
    {
        var point = new PointD
        (
            X: GraphvizWrapperLib.get_x_point(pointPtr),
            Y: GraphvizWrapperLib.get_y_point(pointPtr)
        );

        return point;
    }

    private static RectangleD TranslateRect(IntPtr rectPtr)
    {
        var rect = RectangleD.Create
        (
            x: GraphvizWrapperLib.get_x_rect(rectPtr),
            y: GraphvizWrapperLib.get_y_rect(rectPtr),
            width: GraphvizWrapperLib.get_w_rect(rectPtr),
            height: GraphvizWrapperLib.get_h_rect(rectPtr)
        );

        return rect;
    }

    private static TextInfo TranslateText(IntPtr txtPtr, Font activeFont, FontChar activeFontChar)
    {
        TextInfo text = new TextInfo
        (
            new PointD(GraphvizWrapperLib.get_x_text(txtPtr), GraphvizWrapperLib.get_y_text(txtPtr)),
            GraphvizWrapperLib.get_align(txtPtr),
            GraphvizWrapperLib.get_width(txtPtr),
            GraphvizWrapperLib.GetTextStr(txtPtr)!,
            activeFont,
            activeFontChar,
            CoordinateSystem.BottomLeft
        );

        return text;
    }
}
