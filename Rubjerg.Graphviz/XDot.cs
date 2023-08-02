using System;

namespace Rubjerg.Graphviz
{
    public enum XDotGradType
    {
        None,
        Linear,
        Radial
    }

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

    public struct XDotColor
    {
        public XDotGradType Type { get; set; }
        public string Color { get; set; }
        public XDotLinearGrad LinearGrad { get; set; }
        public XDotRadialGrad RadialGrad { get; set; }
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

    public enum XDotKind
    {
        FilledEllipse, UnfilledEllipse,
        FilledPolygon, UnfilledPolygon,
        FilledBezier, UnfilledBezier,
        Polyline, Text,
        FillColor, PenColor, Font, Style, Image,
        GradFillColor, GradPenColor,
        FontChar
    }

    public struct XDotOp
    {
        public XDotKind Kind { get; set; }
        // We define a custom structure for each kind of operation
        // and a custom field in the union for each structure
        public XDotRect Ellipse { get; set; }      // FilledEllipse, UnfilledEllipse
        public XDotPolyline Polygon { get; set; }  // FilledPolygon, UnfilledPolygon
        public XDotPolyline Polyline { get; set; } // Polyline
        public XDotPolyline Bezier { get; set; }   // FilledBezier, UnfilledBezier
        public XDotText Text { get; set; }         // Text
        public XDotImage Image { get; set; }       // Image
        public string Color { get; set; }          // FillColor, PenColor
        public XDotColor GradColor { get; set; }   // GradFillColor, GradPenColor
        public XDotFont Font { get; set; }         // Font
        public string Style { get; set; }          // Style
        public uint FontChar { get; set; }         // FontChar
    }

    public struct XDot
    {
        public int Count { get; set; }     // Number of xdot ops
        public XDotOp[] Ops { get; set; }  // xdot operations
    }

    internal static class XDotTranslator
    {
        public static XDot TranslateXDot(IntPtr xdotPtr)
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

            return xdot;
        }

        private static XDotOp TranslateXDotOp(IntPtr xdotOpPtr)
        {
            if (xdotOpPtr == IntPtr.Zero)
                throw new ArgumentNullException(nameof(xdotOpPtr));

            XDotOp xdotOp = new XDotOp();
            xdotOp.Kind = XDotFFI.get_kind(xdotOpPtr);

            switch (xdotOp.Kind)
            {
                case XDotKind.FilledEllipse:
                case XDotKind.UnfilledEllipse:
                    xdotOp.Ellipse = TranslateEllipse(XDotFFI.get_ellipse(xdotOpPtr));
                    break;
                case XDotKind.FilledPolygon:
                case XDotKind.UnfilledPolygon:
                case XDotKind.FilledBezier:
                case XDotKind.UnfilledBezier:
                case XDotKind.Polyline:
                    xdotOp.Polyline = TranslatePolyline(XDotFFI.get_polyline(xdotOpPtr));
                    break;
                case XDotKind.Text:
                    xdotOp.Text = TranslateText(XDotFFI.get_text(xdotOpPtr));
                    break;
                case XDotKind.FillColor:
                case XDotKind.PenColor:
                    xdotOp.Color = XDotFFI.GetColor(xdotOpPtr);
                    break;
                case XDotKind.GradFillColor:
                case XDotKind.GradPenColor:
                    xdotOp.GradColor = TranslateGradColor(XDotFFI.get_grad_color(xdotOpPtr));
                    break;
                case XDotKind.Font:
                    xdotOp.Font = TranslateFont(XDotFFI.get_font(xdotOpPtr));
                    break;
                case XDotKind.Style:
                    xdotOp.Style = XDotFFI.GetStyle(xdotOpPtr);
                    break;
                case XDotKind.Image:
                    xdotOp.Image = TranslateImage(XDotFFI.get_image(xdotOpPtr));
                    break;
                case XDotKind.FontChar:
                    xdotOp.FontChar = XDotFFI.get_fontchar(xdotOpPtr);
                    break;
                default:
                    throw new ArgumentException($"Unexpected XDotOp.Kind: {xdotOp.Kind}");
            }

            return xdotOp;
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
            XDotColor color = new XDotColor();
            color.Type = XDotFFI.get_type(colorPtr);
            color.Color = XDotFFI.GetClr(colorPtr);

            switch (color.Type)
            {
                case XDotGradType.Linear:
                    color.LinearGrad = TranslateLinearGrad(XDotFFI.get_ling(colorPtr));
                    break;
                case XDotGradType.Radial:
                    color.RadialGrad = TranslateRadialGrad(XDotFFI.get_ring(colorPtr));
                    break;
                default:
                    throw new ArgumentException($"Unexpected XDotColor.Type: {color.Type}");
            }

            return color;
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
