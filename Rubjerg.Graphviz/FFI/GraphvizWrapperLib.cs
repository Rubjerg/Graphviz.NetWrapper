using System;
using System.Runtime.InteropServices;

namespace Rubjerg.Graphviz.FFI;

using static Marshaling;
using static Constants;

internal static class GraphvizWrapperLib
{
    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void clone_attribute_declarations(IntPtr graphfrom, IntPtr graphto);

    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void convert_to_undirected(IntPtr graph);

    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr edge_label(IntPtr node);
    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr graph_label(IntPtr node);
    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr label_fontname(IntPtr label);
    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern double label_fontsize(IntPtr label);
    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern double label_height(IntPtr label);
    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr label_text(IntPtr label);
    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern double label_width(IntPtr label);

    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern double label_x(IntPtr label);
    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern double label_y(IntPtr label);
    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern double node_height(IntPtr node);
    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr node_label(IntPtr node);
    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern double node_width(IntPtr node);

    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern double node_x(IntPtr node);
    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern double node_y(IntPtr node);

    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.U1)]
    internal static extern bool rj_ageqedge(IntPtr edge1, IntPtr edge2);
    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr rj_aghead(IntPtr node);
    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr rj_agmemread(IntPtr input);
    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr rj_agmemwrite(IntPtr graph);
    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr rj_agmkin(IntPtr edge);
    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr rj_agmkout(IntPtr edge);
    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr rj_agtail(IntPtr node);
    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr rj_sym_key(IntPtr sym);
    
    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr rj_agopen(IntPtr name, int graphtype);
    
    // Accessors for xdot
    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern UIntPtr get_cnt(IntPtr xdot);

    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr get_ops(IntPtr xdot);

    // Accessors for xdot_image
    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr get_name_image(IntPtr img);
    public static string? GetNameImage(IntPtr img) => MarshalFromUtf8(get_name_image(img), false);

    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr get_pos(IntPtr img);

    // Accessors for xdot_font
    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern double get_size(IntPtr font);

    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr get_name_font(IntPtr font);
    public static string? GetNameFont(IntPtr img) => MarshalFromUtf8(get_name_font(img), false);

    // Accessors for xdot_op
    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern XDotKind get_kind(IntPtr op);

    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr get_ellipse(IntPtr op);

    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr get_polygon(IntPtr op);

    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr get_polyline(IntPtr op);

    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr get_bezier(IntPtr op);

    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr get_text(IntPtr op);

    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr get_image(IntPtr op);

    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr get_color(IntPtr op);
    public static string? GetColor(IntPtr op) => MarshalFromUtf8(get_color(op), false);

    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr get_grad_color(IntPtr op);

    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr get_font(IntPtr op);

    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr get_style(IntPtr op);
    public static string? GetStyle(IntPtr op) => MarshalFromUtf8(get_style(op), false);

    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern uint get_fontchar(IntPtr op);

    // Accessors for xdot_color
    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern XDotGradType get_type(IntPtr clr);

    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr get_clr(IntPtr clr);
    public static string? GetClr(IntPtr clr) => MarshalFromUtf8(get_clr(clr), false);

    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr get_ling(IntPtr clr);

    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr get_ring(IntPtr clr);

    // Accessors for xdot_text
    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern double get_x_text(IntPtr txt);

    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern double get_y_text(IntPtr txt);

    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern TextAlign get_align(IntPtr txt);

    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern double get_width(IntPtr txt);

    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr get_text_str(IntPtr txt);
    public static string? GetTextStr(IntPtr txt) => MarshalFromUtf8(get_text_str(txt), false);

    // Accessors for xdot_linear_grad
    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern double get_x0_ling(IntPtr ling);

    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern double get_y0_ling(IntPtr ling);

    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern double get_x1_ling(IntPtr ling);

    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern double get_y1_ling(IntPtr ling);

    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern int get_n_stops_ling(IntPtr ling);

    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr get_stops_ling(IntPtr ling);

    // Accessors for xdot_radial_grad
    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern double get_x0_ring(IntPtr ring);

    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern double get_y0_ring(IntPtr ring);

    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern double get_r0_ring(IntPtr ring);

    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern double get_x1_ring(IntPtr ring);

    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern double get_y1_ring(IntPtr ring);

    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern double get_r1_ring(IntPtr ring);

    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern int get_n_stops_ring(IntPtr ring);

    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr get_stops_ring(IntPtr ring);

    // Accessors for xdot_color_stop
    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern float get_frac(IntPtr stop);

    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr get_color_stop(IntPtr stop);
    public static string? GetColorStop(IntPtr stop) => MarshalFromUtf8(get_color_stop(stop), false);

    // Accessors for xdot_polyline
    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern UIntPtr get_cnt_polyline(IntPtr polyline);

    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr get_pts_polyline(IntPtr polyline);

    // Accessors for xdot_point
    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern double get_x_point(IntPtr point);

    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern double get_y_point(IntPtr point);

    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern double get_z_point(IntPtr point);

    // Accessors for xdot_rect
    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern double get_x_rect(IntPtr rect);

    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern double get_y_rect(IntPtr rect);

    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern double get_w_rect(IntPtr rect);

    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern double get_h_rect(IntPtr rect);

    // Index function for xdot_color_stop array
    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr get_color_stop_at_index(IntPtr stops, int index);

    // Index function for xdot_op array
    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr get_op_at_index(IntPtr ops, int index);

    // Index function for xdot_pt array
    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr get_pt_at_index(IntPtr pts, int index);
}