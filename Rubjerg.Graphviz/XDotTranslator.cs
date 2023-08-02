using System;
using System.Collections.Generic;
using System.Linq;

namespace Rubjerg.Graphviz
{
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

    internal static class XDotTranslator
    {
        public static List<XDotOp> TranslateXDot(IntPtr xdotPtr)
        {
            if (xdotPtr == IntPtr.Zero)
                throw new ArgumentNullException(nameof(xdotPtr));

            XDot xdot = new XDot();
            xdot.Count = (int)XDotFFI.get_cnt(xdotPtr);

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
                        Ellipse = TranslateEllipse(XDotFFI.get_ellipse(xdotOpPtr))
                    };
                case XDotKind.UnfilledEllipse:
                    return new XDotOp.UnfilledEllipse()
                    {
                        Ellipse = TranslateEllipse(XDotFFI.get_ellipse(xdotOpPtr))
                    };
                case XDotKind.FilledPolygon:
                    return new XDotOp.FilledPolygon()
                    {
                        Polygon = TranslatePolyline(XDotFFI.get_polyline(xdotOpPtr))
                    };
                case XDotKind.UnfilledPolygon:
                    return new XDotOp.FilledPolygon()
                    {
                        Polygon = TranslatePolyline(XDotFFI.get_polyline(xdotOpPtr))
                    };
                case XDotKind.FilledBezier:
                    return new XDotOp.FilledBezier()
                    {
                        Bezier = TranslatePolyline(XDotFFI.get_polyline(xdotOpPtr))
                    };
                case XDotKind.UnfilledBezier:
                    return new XDotOp.UnfilledBezier()
                    {
                        Bezier = TranslatePolyline(XDotFFI.get_polyline(xdotOpPtr))
                    };
                case XDotKind.Polyline:
                    return new XDotOp.PolyLine()
                    {
                        Polyline = TranslatePolyline(XDotFFI.get_polyline(xdotOpPtr))
                    };
                case XDotKind.Text:
                    return new XDotOp.Text()
                    {
                        Value = TranslateText(XDotFFI.get_text(xdotOpPtr))
                    };
                case XDotKind.FillColor:
                    return new XDotOp.FillColor()
                    {
                        Color = XDotFFI.GetColor(xdotOpPtr)
                    };
                case XDotKind.PenColor:
                    return new XDotOp.PenColor()
                    {
                        Color = XDotFFI.GetColor(xdotOpPtr)
                    };
                case XDotKind.GradFillColor:
                    return new XDotOp.GradFillColor()
                    {
                        GradColor = TranslateGradColor(XDotFFI.get_grad_color(xdotOpPtr))
                    };
                case XDotKind.GradPenColor:
                    return new XDotOp.GradPenColor()
                    {
                        GradColor = TranslateGradColor(XDotFFI.get_grad_color(xdotOpPtr))
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
                        Value = XDotFFI.get_fontchar(xdotOpPtr)
                    };
                default:
                    throw new ArgumentException($"Unexpected XDotOp.Kind: {kind}");
            }
        }

        private static XDotImage TranslateImage(IntPtr imagePtr)
        {
            XDotImage image = new XDotImage();
            image.Pos = TranslateRect(XDotFFI.get_pos(imagePtr));
            image.Name = XDotFFI.GetNameImage(imagePtr);

            return image;
        }

        private static XDotFont TranslateFont(IntPtr fontPtr)
        {
            XDotFont font = new XDotFont();
            font.Size = XDotFFI.get_size(fontPtr);
            font.Name = XDotFFI.GetNameFont(fontPtr);

            return font;
        }

        private static XDotRect TranslateEllipse(IntPtr ellipsePtr)
        {
            XDotRect ellipse = new XDotRect();
            ellipse.X = XDotFFI.get_x_rect(ellipsePtr);
            ellipse.Y = XDotFFI.get_y_rect(ellipsePtr);
            ellipse.Width = XDotFFI.get_w_rect(ellipsePtr);
            ellipse.Height = XDotFFI.get_h_rect(ellipsePtr);

            return ellipse;
        }

        private static XDotColor TranslateGradColor(IntPtr colorPtr)
        {
            var type = XDotFFI.get_type(colorPtr);
            switch (type)
            {
                case XDotGradType.None:
                    return new XDotColor.Uniform()
                    {
                        Color = XDotFFI.GetClr(colorPtr)
                    };
                case XDotGradType.Linear:
                    return new XDotColor.LinearGradient()
                    {
                        LinearGrad = TranslateLinearGrad(XDotFFI.get_ling(colorPtr))
                    };
                case XDotGradType.Radial:
                    return new XDotColor.RadialGradient()
                    {
                        RadialGrad = TranslateRadialGrad(XDotFFI.get_ring(colorPtr))
                    };
                default:
                    throw new ArgumentException($"Unexpected XDotColor.Type: {type}");
            }
        }

        private static XDotLinearGrad TranslateLinearGrad(IntPtr lingPtr)
        {
            XDotLinearGrad linearGrad = new XDotLinearGrad();
            linearGrad.X0 = XDotFFI.get_x0_ling(lingPtr);
            linearGrad.Y0 = XDotFFI.get_y0_ling(lingPtr);
            linearGrad.X1 = XDotFFI.get_x1_ling(lingPtr);
            linearGrad.Y1 = XDotFFI.get_y1_ling(lingPtr);
            linearGrad.NStops = XDotFFI.get_n_stops_ling(lingPtr);

            // Translate the array of ColorStops
            int count = linearGrad.NStops;
            linearGrad.Stops = new XDotColorStop[count];
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
            XDotRadialGrad radialGrad = new XDotRadialGrad();
            radialGrad.X0 = XDotFFI.get_x0_ring(ringPtr);
            radialGrad.Y0 = XDotFFI.get_y0_ring(ringPtr);
            radialGrad.R0 = XDotFFI.get_r0_ring(ringPtr);
            radialGrad.X1 = XDotFFI.get_x1_ring(ringPtr);
            radialGrad.Y1 = XDotFFI.get_y1_ring(ringPtr);
            radialGrad.R1 = XDotFFI.get_r1_ring(ringPtr);
            radialGrad.NStops = XDotFFI.get_n_stops_ring(ringPtr);

            // Translate the array of ColorStops
            int count = radialGrad.NStops;
            radialGrad.Stops = new XDotColorStop[count];
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
            XDotColorStop colorStop = new XDotColorStop();
            colorStop.Frac = XDotFFI.get_frac(stopPtr);
            colorStop.Color = XDotFFI.GetColorStop(stopPtr);

            return colorStop;
        }

        private static XDotPolyline TranslatePolyline(IntPtr polylinePtr)
        {
            XDotPolyline polyline = new XDotPolyline();
            polyline.Count = (int)XDotFFI.get_cnt_polyline(polylinePtr);

            // Translate the array of Points
            int count = polyline.Count;
            polyline.Points = new XDotPoint[count];
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
            XDotPoint point = new XDotPoint();
            point.X = XDotFFI.get_x_point(pointPtr);
            point.Y = XDotFFI.get_y_point(pointPtr);
            point.Z = XDotFFI.get_z_point(pointPtr);

            return point;
        }

        private static XDotRect TranslateRect(IntPtr rectPtr)
        {
            XDotRect rect = new XDotRect();
            rect.X = XDotFFI.get_x_rect(rectPtr);
            rect.Y = XDotFFI.get_y_rect(rectPtr);
            rect.Width = XDotFFI.get_w_rect(rectPtr);
            rect.Height = XDotFFI.get_h_rect(rectPtr);

            return rect;
        }

        private static XDotText TranslateText(IntPtr txtPtr)
        {
            XDotText text = new XDotText();
            text.X = XDotFFI.get_x_text(txtPtr);
            text.Y = XDotFFI.get_y_text(txtPtr);
            text.Align = XDotFFI.get_align(txtPtr);
            text.Width = XDotFFI.get_width(txtPtr);
            text.Text = XDotFFI.GetTextStr(txtPtr);

            return text;
        }
    }
}
