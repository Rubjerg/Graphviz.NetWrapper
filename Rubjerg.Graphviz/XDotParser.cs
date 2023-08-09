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
    public static List<XDotOp> ParseXDot(string xdotString)
    {
        IntPtr xdot = XDotFFI.parseXDot(xdotString);
        try
        {
            return TranslateXDot(xdot);
        }
        finally
        {
            if (xdot != IntPtr.Zero)
            {
                XDotFFI.freeXDot(xdot);
            }
        }
    }

    internal static List<XDotOp> TranslateXDot(IntPtr xdotPtr)
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
        for (int i = 0; i < count; ++i)
        {
            IntPtr xdotOpPtr = XDotFFI.get_op_at_index(opsPtr, i);
            xdot.Ops[i] = TranslateXDotOp(xdotOpPtr);
        }

        return xdot.Ops.ToList();
    }

    private static XDotOp TranslateXDotOp(IntPtr xdotOpPtr)
    {
        if (xdotOpPtr == IntPtr.Zero)
            throw new ArgumentNullException(nameof(xdotOpPtr));

        var kind = XDotFFI.get_kind(xdotOpPtr);
        switch (kind)
        {
            case XDotKind.FilledEllipse:
                return new XDotOp.FilledEllipse()
                {
                    Value = TranslateEllipse(XDotFFI.get_ellipse(xdotOpPtr))
                };
            case XDotKind.UnfilledEllipse:
                return new XDotOp.UnfilledEllipse()
                {
                    Value = TranslateEllipse(XDotFFI.get_ellipse(xdotOpPtr))
                };
            case XDotKind.FilledPolygon:
                return new XDotOp.FilledPolygon()
                {
                    Value = TranslatePolyline(XDotFFI.get_polyline(xdotOpPtr))
                };
            case XDotKind.UnfilledPolygon:
                return new XDotOp.FilledPolygon()
                {
                    Value = TranslatePolyline(XDotFFI.get_polyline(xdotOpPtr))
                };
            case XDotKind.FilledBezier:
                return new XDotOp.FilledBezier()
                {
                    Value = TranslatePolyline(XDotFFI.get_polyline(xdotOpPtr))
                };
            case XDotKind.UnfilledBezier:
                return new XDotOp.UnfilledBezier()
                {
                    Value = TranslatePolyline(XDotFFI.get_polyline(xdotOpPtr))
                };
            case XDotKind.Polyline:
                return new XDotOp.PolyLine()
                {
                    Value = TranslatePolyline(XDotFFI.get_polyline(xdotOpPtr))
                };
            case XDotKind.Text:
                return new XDotOp.Text()
                {
                    Value = TranslateText(XDotFFI.get_text(xdotOpPtr))
                };
            case XDotKind.FillColor:
                return new XDotOp.FillColor()
                {
                    Value = XDotFFI.GetColor(xdotOpPtr)
                };
            case XDotKind.PenColor:
                return new XDotOp.PenColor()
                {
                    Value = XDotFFI.GetColor(xdotOpPtr)
                };
            case XDotKind.GradFillColor:
                return new XDotOp.GradFillColor()
                {
                    Value = TranslateGradColor(XDotFFI.get_grad_color(xdotOpPtr))
                };
            case XDotKind.GradPenColor:
                return new XDotOp.GradPenColor()
                {
                    Value = TranslateGradColor(XDotFFI.get_grad_color(xdotOpPtr))
                };
            case XDotKind.Font:
                return new XDotOp.Font()
                {
                    Value = TranslateFont(XDotFFI.get_font(xdotOpPtr))
                };
            case XDotKind.Style:
                return new XDotOp.Style()
                {
                    Value = XDotFFI.GetStyle(xdotOpPtr)
                };
            case XDotKind.Image:
                return new XDotOp.Image()
                {
                    Value = TranslateImage(XDotFFI.get_image(xdotOpPtr))
                };
            case XDotKind.FontChar:
                return new XDotOp.FontChar()
                {
                    Value = TranslateFontChar(XDotFFI.get_fontchar(xdotOpPtr))
                };
            default:
                throw new ArgumentException($"Unexpected XDotOp.Kind: {kind}");
        }
    }

    private static XDotFontChar TranslateFontChar(uint value)
    {
        return (XDotFontChar)(int)value;
    }
    private static XDotImage TranslateImage(IntPtr imagePtr)
    {
        XDotImage image = new XDotImage
        {
            Pos = TranslateRect(XDotFFI.get_pos(imagePtr)),
            Name = XDotFFI.GetNameImage(imagePtr)
        };

        return image;
    }

    private static XDotFont TranslateFont(IntPtr fontPtr)
    {
        XDotFont font = new XDotFont
        {
            Size = XDotFFI.get_size(fontPtr),
            Name = XDotFFI.GetNameFont(fontPtr)
        };

        return font;
    }

    private static XDotRect TranslateEllipse(IntPtr ellipsePtr)
    {
        XDotRect ellipse = new XDotRect
        {
            X = XDotFFI.get_x_rect(ellipsePtr),
            Y = XDotFFI.get_y_rect(ellipsePtr),
            Width = XDotFFI.get_w_rect(ellipsePtr),
            Height = XDotFFI.get_h_rect(ellipsePtr)
        };

        return ellipse;
    }

    private static XDotGradColor TranslateGradColor(IntPtr colorPtr)
    {
        var type = XDotFFI.get_type(colorPtr);
        switch (type)
        {
            case XDotGradType.None:
                return new XDotGradColor.Uniform()
                {
                    Color = XDotFFI.GetClr(colorPtr)
                };
            case XDotGradType.Linear:
                return new XDotGradColor.LinearGradient()
                {
                    LinearGrad = TranslateLinearGrad(XDotFFI.get_ling(colorPtr))
                };
            case XDotGradType.Radial:
                return new XDotGradColor.RadialGradient()
                {
                    RadialGrad = TranslateRadialGrad(XDotFFI.get_ring(colorPtr))
                };
            default:
                throw new ArgumentException($"Unexpected XDotColor.Type: {type}");
        }
    }

    private static XDotLinearGrad TranslateLinearGrad(IntPtr lingPtr)
    {
        int count = XDotFFI.get_n_stops_ling(lingPtr);
        XDotLinearGrad linearGrad = new XDotLinearGrad
        {
            X0 = XDotFFI.get_x0_ling(lingPtr),
            Y0 = XDotFFI.get_y0_ling(lingPtr),
            X1 = XDotFFI.get_x1_ling(lingPtr),
            Y1 = XDotFFI.get_y1_ling(lingPtr),
            NStops = count,
            Stops = new XDotColorStop[count]
        };

        // Translate the array of ColorStops
        var stopsPtr = XDotFFI.get_stops_ling(lingPtr);
        for (int i = 0; i < count; ++i)
        {
            IntPtr colorStopPtr = XDotFFI.get_color_stop_at_index(stopsPtr, i);
            linearGrad.Stops[i] = TranslateColorStop(colorStopPtr);
        }

        return linearGrad;
    }

    private static XDotRadialGrad TranslateRadialGrad(IntPtr ringPtr)
    {
        int count = XDotFFI.get_n_stops_ring(ringPtr);
        XDotRadialGrad radialGrad = new XDotRadialGrad
        {
            X0 = XDotFFI.get_x0_ring(ringPtr),
            Y0 = XDotFFI.get_y0_ring(ringPtr),
            R0 = XDotFFI.get_r0_ring(ringPtr),
            X1 = XDotFFI.get_x1_ring(ringPtr),
            Y1 = XDotFFI.get_y1_ring(ringPtr),
            R1 = XDotFFI.get_r1_ring(ringPtr),
            NStops = count,
            Stops = new XDotColorStop[count]
        };

        // Translate the array of ColorStops
        var stopsPtr = XDotFFI.get_stops_ring(ringPtr);
        for (int i = 0; i < count; ++i)
        {
            IntPtr colorStopPtr = XDotFFI.get_color_stop_at_index(stopsPtr, i);
            radialGrad.Stops[i] = TranslateColorStop(colorStopPtr);
        }

        return radialGrad;
    }

    private static XDotColorStop TranslateColorStop(IntPtr stopPtr)
    {
        XDotColorStop colorStop = new XDotColorStop
        {
            Frac = XDotFFI.get_frac(stopPtr),
            Color = XDotFFI.GetColorStop(stopPtr)
        };

        return colorStop;
    }

    private static XDotPolyline TranslatePolyline(IntPtr polylinePtr)
    {
        int count = (int)XDotFFI.get_cnt_polyline(polylinePtr);
        XDotPolyline polyline = new XDotPolyline
        {
            Count = count,
            Points = new XDotPoint[count]
        };

        // Translate the array of Points
        var pointsPtr = XDotFFI.get_pts_polyline(polylinePtr);
        for (int i = 0; i < count; ++i)
        {
            IntPtr pointPtr = XDotFFI.get_pt_at_index(pointsPtr, i);
            polyline.Points[i] = TranslatePoint(pointPtr);
        }

        return polyline;
    }

    private static XDotPoint TranslatePoint(IntPtr pointPtr)
    {
        XDotPoint point = new XDotPoint
        {
            X = XDotFFI.get_x_point(pointPtr),
            Y = XDotFFI.get_y_point(pointPtr),
            Z = XDotFFI.get_z_point(pointPtr)
        };

        return point;
    }

    private static XDotRect TranslateRect(IntPtr rectPtr)
    {
        XDotRect rect = new XDotRect
        {
            X = XDotFFI.get_x_rect(rectPtr),
            Y = XDotFFI.get_y_rect(rectPtr),
            Width = XDotFFI.get_w_rect(rectPtr),
            Height = XDotFFI.get_h_rect(rectPtr)
        };

        return rect;
    }

    private static XDotText TranslateText(IntPtr txtPtr)
    {
        XDotText text = new XDotText
        {
            X = XDotFFI.get_x_text(txtPtr),
            Y = XDotFFI.get_y_text(txtPtr),
            Align = XDotFFI.get_align(txtPtr),
            Width = XDotFFI.get_width(txtPtr),
            Text = XDotFFI.GetTextStr(txtPtr)
        };

        return text;
    }
}
