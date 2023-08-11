using System;
using System.Collections.Generic;
using System.Linq;

namespace Rubjerg.Graphviz;

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
        IntPtr xdot = XDotFFI.parseXDot(xdotString);
        try
        {
            return TranslateXDot(xdot, coordinateSystem, maxY);
        }
        finally
        {
            if (xdot != IntPtr.Zero)
            {
                XDotFFI.freeXDot(xdot);
            }
        }
    }

    internal static List<XDotOp> TranslateXDot(IntPtr xdotPtr, CoordinateSystem coordinateSystem, double maxY)
    {
        if (xdotPtr == IntPtr.Zero)
            throw new ArgumentNullException(nameof(xdotPtr));

        XDot xdot = new XDot
        {
            Count = (int)XDotFFI.get_cnt(xdotPtr)
        };

        // Translate the array of XDotOps
        int count = xdot.Count;
        xdot.Ops = new XDotOp[count];
        var opsPtr = XDotFFI.get_ops(xdotPtr);

        var activeFont = Font.Default;
        var activeFontChar = FontChar.None;
        for (int i = 0; i < count; ++i)
        {
            IntPtr xdotOpPtr = XDotFFI.get_op_at_index(opsPtr, i);
            var kind = XDotFFI.get_kind(xdotOpPtr);
            switch (kind)
            {
                case XDotKind.FilledEllipse:
                    xdot.Ops[i] = new XDotOp.FilledEllipse(TranslateEllipse(XDotFFI.get_ellipse(xdotOpPtr))
                        .ForCoordSystem(coordinateSystem, maxY));
                    break;
                case XDotKind.UnfilledEllipse:
                    xdot.Ops[i] = new XDotOp.UnfilledEllipse(TranslateEllipse(XDotFFI.get_ellipse(xdotOpPtr))
                        .ForCoordSystem(coordinateSystem, maxY));
                    break;
                case XDotKind.FilledPolygon:
                    xdot.Ops[i] = new XDotOp.FilledPolygon(TranslatePolyline(XDotFFI.get_polyline(xdotOpPtr))
                        .ForCoordSystem(coordinateSystem, maxY));
                    break;
                case XDotKind.UnfilledPolygon:
                    xdot.Ops[i] = new XDotOp.FilledPolygon(TranslatePolyline(XDotFFI.get_polyline(xdotOpPtr))
                        .ForCoordSystem(coordinateSystem, maxY));
                    break;
                case XDotKind.FilledBezier:
                    xdot.Ops[i] = new XDotOp.FilledBezier(TranslatePolyline(XDotFFI.get_polyline(xdotOpPtr))
                        .ForCoordSystem(coordinateSystem, maxY));
                    break;
                case XDotKind.UnfilledBezier:
                    xdot.Ops[i] = new XDotOp.UnfilledBezier(TranslatePolyline(XDotFFI.get_polyline(xdotOpPtr))
                        .ForCoordSystem(coordinateSystem, maxY));
                    break;
                case XDotKind.Polyline:
                    xdot.Ops[i] = new XDotOp.PolyLine(TranslatePolyline(XDotFFI.get_polyline(xdotOpPtr))
                        .ForCoordSystem(coordinateSystem, maxY));
                    break;
                case XDotKind.Text:
                    xdot.Ops[i] = new XDotOp.Text(TranslateText(XDotFFI.get_text(xdotOpPtr), activeFont, activeFontChar)
                        .ForCoordSystem(coordinateSystem, maxY));
                    break;
                case XDotKind.FillColor:
                    xdot.Ops[i] = new XDotOp.FillColor(new Color.Uniform(XDotFFI.GetColor(xdotOpPtr)));
                    break;
                case XDotKind.PenColor:
                    xdot.Ops[i] = new XDotOp.PenColor(new Color.Uniform(XDotFFI.GetColor(xdotOpPtr)));
                    break;
                case XDotKind.GradFillColor:
                    xdot.Ops[i] = new XDotOp.FillColor(TranslateGradColor(XDotFFI.get_grad_color(xdotOpPtr)));
                    break;
                case XDotKind.GradPenColor:
                    xdot.Ops[i] = new XDotOp.PenColor(TranslateGradColor(XDotFFI.get_grad_color(xdotOpPtr)));
                    break;
                case XDotKind.Font:
                    activeFont = TranslateFont(XDotFFI.get_font(xdotOpPtr));
                    break;
                case XDotKind.Style:
                    xdot.Ops[i] = new XDotOp.Style(XDotFFI.GetStyle(xdotOpPtr));
                    break;
                case XDotKind.Image:
                    xdot.Ops[i] = new XDotOp.Image(TranslateImage(XDotFFI.get_image(xdotOpPtr))
                        .ForCoordSystem(coordinateSystem, maxY));
                    break;
                case XDotKind.FontChar:
                    activeFontChar = TranslateFontChar(XDotFFI.get_fontchar(xdotOpPtr));
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
            Position: TranslateRect(XDotFFI.get_pos(imagePtr)),
            Name: XDotFFI.GetNameImage(imagePtr)
        );

        return image;
    }

    private static Font TranslateFont(IntPtr fontPtr)
    {
        Font font = new Font
        (
            Size: XDotFFI.get_size(fontPtr),
            Name: XDotFFI.GetNameFont(fontPtr)
        );

        return font;
    }

    private static RectangleD TranslateEllipse(IntPtr ellipsePtr)
    {
        RectangleD ellipse = RectangleD.Create
        (
            XDotFFI.get_x_rect(ellipsePtr),
            XDotFFI.get_y_rect(ellipsePtr),
            XDotFFI.get_w_rect(ellipsePtr),
            XDotFFI.get_h_rect(ellipsePtr)
        );

        return ellipse;
    }

    private static Color TranslateGradColor(IntPtr colorPtr)
    {
        var type = XDotFFI.get_type(colorPtr);
        switch (type)
        {
            case XDotGradType.None:
                return new Color.Uniform(XDotFFI.GetClr(colorPtr));
            case XDotGradType.Linear:
                return new Color.Linear(TranslateLinearGrad(XDotFFI.get_ling(colorPtr)));
            case XDotGradType.Radial:
                return new Color.Radial(TranslateRadialGrad(XDotFFI.get_ring(colorPtr)));
            default:
                throw new ArgumentException($"Unexpected XDotColor.Type: {type}");
        }
    }

    private static LinearGradient TranslateLinearGrad(IntPtr lingPtr)
    {
        int count = XDotFFI.get_n_stops_ling(lingPtr);
        LinearGradient linearGrad = new LinearGradient
        (
            Point0: new PointD(XDotFFI.get_x0_ling(lingPtr), XDotFFI.get_y0_ling(lingPtr)),
            Point1: new PointD(XDotFFI.get_x1_ling(lingPtr), XDotFFI.get_y1_ling(lingPtr)),
            Stops: new ColorStop[count]
        );

        // Translate the array of ColorStops
        var stopsPtr = XDotFFI.get_stops_ling(lingPtr);
        for (int i = 0; i < count; ++i)
        {
            IntPtr colorStopPtr = XDotFFI.get_color_stop_at_index(stopsPtr, i);
            linearGrad.Stops[i] = TranslateColorStop(colorStopPtr);
        }

        return linearGrad;
    }

    private static RadialGradient TranslateRadialGrad(IntPtr ringPtr)
    {
        int count = XDotFFI.get_n_stops_ring(ringPtr);
        RadialGradient radialGrad = new RadialGradient
        (
            Point0: new PointD(XDotFFI.get_x0_ring(ringPtr), XDotFFI.get_y0_ring(ringPtr)),
            Point1: new PointD(XDotFFI.get_x1_ring(ringPtr), XDotFFI.get_y1_ring(ringPtr)),
            Radius0: XDotFFI.get_r0_ring(ringPtr),
            Radius1: XDotFFI.get_r1_ring(ringPtr),
            Stops: new ColorStop[count]
        );

        // Translate the array of ColorStops
        var stopsPtr = XDotFFI.get_stops_ring(ringPtr);
        for (int i = 0; i < count; ++i)
        {
            IntPtr colorStopPtr = XDotFFI.get_color_stop_at_index(stopsPtr, i);
            radialGrad.Stops[i] = TranslateColorStop(colorStopPtr);
        }

        return radialGrad;
    }

    private static ColorStop TranslateColorStop(IntPtr stopPtr)
    {
        ColorStop colorStop = new ColorStop
        (
            Frac: XDotFFI.get_frac(stopPtr),
            HtmlColor: XDotFFI.GetColorStop(stopPtr)
        );

        return colorStop;
    }

    private static PointD[] TranslatePolyline(IntPtr polylinePtr)
    {
        int count = (int)XDotFFI.get_cnt_polyline(polylinePtr);
        var points = new PointD[count];

        // Translate the array of Points
        var pointsPtr = XDotFFI.get_pts_polyline(polylinePtr);
        for (int i = 0; i < count; ++i)
        {
            IntPtr pointPtr = XDotFFI.get_pt_at_index(pointsPtr, i);
            points[i] = TranslatePoint(pointPtr);
        }

        return points;
    }

    private static PointD TranslatePoint(IntPtr pointPtr)
    {
        var point = new PointD
        (
            X: XDotFFI.get_x_point(pointPtr),
            Y: XDotFFI.get_y_point(pointPtr)
        );

        return point;
    }

    private static RectangleD TranslateRect(IntPtr rectPtr)
    {
        var rect = RectangleD.Create
        (
            x: XDotFFI.get_x_rect(rectPtr),
            y: XDotFFI.get_y_rect(rectPtr),
            width: XDotFFI.get_w_rect(rectPtr),
            height: XDotFFI.get_h_rect(rectPtr)
        );

        return rect;
    }

    private static TextInfo TranslateText(IntPtr txtPtr, Font activeFont, FontChar activeFontChar)
    {
        TextInfo text = new TextInfo
        (
            new PointD(XDotFFI.get_x_text(txtPtr), XDotFFI.get_y_text(txtPtr)),
            XDotFFI.get_align(txtPtr),
            XDotFFI.get_width(txtPtr),
            XDotFFI.GetTextStr(txtPtr),
            activeFont,
            activeFontChar,
            CoordinateSystem.BottomLeft
        );

        return text;
    }
}
