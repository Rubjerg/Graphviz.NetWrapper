using System;
using System.Runtime.InteropServices;

namespace Rubjerg.Graphviz;

using static Marshaling;
using static Constants;

/// <summary>
/// See https://graphviz.org/docs/outputs/canon/#xdot
/// </summary>
internal static class XDotFFI
{
    [DllImport(XDotLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr parseXDot(IntPtr xdotString);
    public static IntPtr ParseXDot(string xdotString) => MarshalToUtf8(xdotString, parseXDot);

    [DllImport(XDotLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern void freeXDot(IntPtr xdotptr);

    // Accessors for xdot
    [DllImport(GraphvizWrapperLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern UIntPtr get_cnt(IntPtr xdot);

    [DllImport(GraphvizWrapperLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr get_ops(IntPtr xdot);

    // Accessors for xdot_image
    [DllImport(GraphvizWrapperLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr get_name_image(IntPtr img);
    public static string? GetNameImage(IntPtr img) => MarshalFromUtf8(get_name_image(img), false);

    [DllImport(GraphvizWrapperLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr get_pos(IntPtr img);

    // Accessors for xdot_font
    [DllImport(GraphvizWrapperLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern double get_size(IntPtr font);

    [DllImport(GraphvizWrapperLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr get_name_font(IntPtr font);
    public static string? GetNameFont(IntPtr img) => MarshalFromUtf8(get_name_font(img), false);

    // Accessors for xdot_op
    [DllImport(GraphvizWrapperLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern XDotKind get_kind(IntPtr op);

    [DllImport(GraphvizWrapperLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr get_ellipse(IntPtr op);

    [DllImport(GraphvizWrapperLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr get_polygon(IntPtr op);

    [DllImport(GraphvizWrapperLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr get_polyline(IntPtr op);

    [DllImport(GraphvizWrapperLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr get_bezier(IntPtr op);

    [DllImport(GraphvizWrapperLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr get_text(IntPtr op);

    [DllImport(GraphvizWrapperLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr get_image(IntPtr op);

    [DllImport(GraphvizWrapperLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr get_color(IntPtr op);
    public static string? GetColor(IntPtr op) => MarshalFromUtf8(get_color(op), false);

    [DllImport(GraphvizWrapperLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr get_grad_color(IntPtr op);

    [DllImport(GraphvizWrapperLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr get_font(IntPtr op);

    [DllImport(GraphvizWrapperLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr get_style(IntPtr op);
    public static string? GetStyle(IntPtr op) => MarshalFromUtf8(get_style(op), false);

    [DllImport(GraphvizWrapperLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern uint get_fontchar(IntPtr op);

    // Accessors for xdot_color
    [DllImport(GraphvizWrapperLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern XDotGradType get_type(IntPtr clr);

    [DllImport(GraphvizWrapperLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr get_clr(IntPtr clr);
    public static string? GetClr(IntPtr clr) => MarshalFromUtf8(get_clr(clr), false);

    [DllImport(GraphvizWrapperLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr get_ling(IntPtr clr);

    [DllImport(GraphvizWrapperLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr get_ring(IntPtr clr);

    // Accessors for xdot_text
    [DllImport(GraphvizWrapperLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern double get_x_text(IntPtr txt);

    [DllImport(GraphvizWrapperLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern double get_y_text(IntPtr txt);

    [DllImport(GraphvizWrapperLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern TextAlign get_align(IntPtr txt);

    [DllImport(GraphvizWrapperLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern double get_width(IntPtr txt);

    [DllImport(GraphvizWrapperLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr get_text_str(IntPtr txt);
    public static string? GetTextStr(IntPtr txt) => MarshalFromUtf8(get_text_str(txt), false);

    // Accessors for xdot_linear_grad
    [DllImport(GraphvizWrapperLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern double get_x0_ling(IntPtr ling);

    [DllImport(GraphvizWrapperLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern double get_y0_ling(IntPtr ling);

    [DllImport(GraphvizWrapperLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern double get_x1_ling(IntPtr ling);

    [DllImport(GraphvizWrapperLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern double get_y1_ling(IntPtr ling);

    [DllImport(GraphvizWrapperLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern int get_n_stops_ling(IntPtr ling);

    [DllImport(GraphvizWrapperLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr get_stops_ling(IntPtr ling);

    // Accessors for xdot_radial_grad
    [DllImport(GraphvizWrapperLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern double get_x0_ring(IntPtr ring);

    [DllImport(GraphvizWrapperLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern double get_y0_ring(IntPtr ring);

    [DllImport(GraphvizWrapperLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern double get_r0_ring(IntPtr ring);

    [DllImport(GraphvizWrapperLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern double get_x1_ring(IntPtr ring);

    [DllImport(GraphvizWrapperLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern double get_y1_ring(IntPtr ring);

    [DllImport(GraphvizWrapperLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern double get_r1_ring(IntPtr ring);

    [DllImport(GraphvizWrapperLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern int get_n_stops_ring(IntPtr ring);

    [DllImport(GraphvizWrapperLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr get_stops_ring(IntPtr ring);

    // Accessors for xdot_color_stop
    [DllImport(GraphvizWrapperLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern float get_frac(IntPtr stop);

    [DllImport(GraphvizWrapperLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr get_color_stop(IntPtr stop);
    public static string? GetColorStop(IntPtr stop) => MarshalFromUtf8(get_color_stop(stop), false);

    // Accessors for xdot_polyline
    [DllImport(GraphvizWrapperLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern UIntPtr get_cnt_polyline(IntPtr polyline);

    [DllImport(GraphvizWrapperLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr get_pts_polyline(IntPtr polyline);

    // Accessors for xdot_point
    [DllImport(GraphvizWrapperLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern double get_x_point(IntPtr point);

    [DllImport(GraphvizWrapperLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern double get_y_point(IntPtr point);

    [DllImport(GraphvizWrapperLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern double get_z_point(IntPtr point);

    // Accessors for xdot_rect
    [DllImport(GraphvizWrapperLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern double get_x_rect(IntPtr rect);

    [DllImport(GraphvizWrapperLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern double get_y_rect(IntPtr rect);

    [DllImport(GraphvizWrapperLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern double get_w_rect(IntPtr rect);

    [DllImport(GraphvizWrapperLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern double get_h_rect(IntPtr rect);

    // Index function for xdot_color_stop array
    [DllImport(GraphvizWrapperLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr get_color_stop_at_index(IntPtr stops, int index);

    // Index function for xdot_op array
    [DllImport(GraphvizWrapperLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr get_op_at_index(IntPtr ops, int index);

    // Index function for xdot_pt array
    [DllImport(GraphvizWrapperLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr get_pt_at_index(IntPtr pts, int index);

}
