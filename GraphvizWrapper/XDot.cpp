#include "GraphvizWrapper.h"


// Accessors for xdot
size_t get_cnt(xdot* xdot) { return xdot->cnt; }
xdot_op* get_ops(xdot* xdot) { return xdot->ops; }

// Accessors for xdot_op
xdot_kind get_kind(xdot_op* op) { return op->kind; }
xdot_rect* get_ellipse(xdot_op* op) { return &op->u.ellipse; }
xdot_polyline* get_polygon(xdot_op* op) { return &op->u.polygon; }
xdot_polyline* get_polyline(xdot_op* op) { return &op->u.polyline; }
xdot_polyline* get_bezier(xdot_op* op) { return &op->u.bezier; }
xdot_text* get_text(xdot_op* op) { return &op->u.text; }
xdot_image* get_image(xdot_op* op) { return &op->u.image; }
char* get_color(xdot_op* op) { return op->u.color; }
xdot_color* get_grad_color(xdot_op* op) { return &op->u.grad_color; }
xdot_font* get_font(xdot_op* op) { return &op->u.font; }
char* get_style(xdot_op* op) { return op->u.style; }
unsigned int get_fontchar(xdot_op* op) { return op->u.fontchar; }

// Accessors for xdot_image
xdot_rect* get_pos(xdot_image* img) { return &img->pos; }
char* get_name_image(xdot_image* img) { return img->name; }

// Accessors for xdot_font
double get_size(xdot_font* font) { return font->size; }
char* get_name_font(xdot_font* font) { return font->name; }

// Accessors for xdot_color
xdot_grad_type get_type(xdot_color* clr) { return clr->type; }
char* get_clr(xdot_color* clr) { return clr->u.clr; }
xdot_linear_grad* get_ling(xdot_color* clr) { return &clr->u.ling; }
xdot_radial_grad* get_ring(xdot_color* clr) { return &clr->u.ring; }

// Accessors for xdot_text
double get_x_text(xdot_text* txt) { return txt->x; }
double get_y_text(xdot_text* txt) { return txt->y; }
xdot_align get_align(xdot_text* txt) { return txt->align; }
double get_width(xdot_text* txt) { return txt->width; }
char* get_text_str(xdot_text* txt) { return txt->text; }

// Accessors for xdot_linear_grad
double get_x0_ling(xdot_linear_grad* ling) { return ling->x0; }
double get_y0_ling(xdot_linear_grad* ling) { return ling->y0; }
double get_x1_ling(xdot_linear_grad* ling) { return ling->x1; }
double get_y1_ling(xdot_linear_grad* ling) { return ling->y1; }
int get_n_stops_ling(xdot_linear_grad* ling) { return ling->n_stops; }
xdot_color_stop* get_stops_ling(xdot_linear_grad* ling) { return ling->stops; }

// Accessors for xdot_radial_grad
double get_x0_ring(xdot_radial_grad* ring) { return ring->x0; }
double get_y0_ring(xdot_radial_grad* ring) { return ring->y0; }
double get_r0_ring(xdot_radial_grad* ring) { return ring->r0; }
double get_x1_ring(xdot_radial_grad* ring) { return ring->x1; }
double get_y1_ring(xdot_radial_grad* ring) { return ring->y1; }
double get_r1_ring(xdot_radial_grad* ring) { return ring->r1; }
int get_n_stops_ring(xdot_radial_grad* ring) { return ring->n_stops; }
xdot_color_stop* get_stops_ring(xdot_radial_grad* ring) { return ring->stops; }

// Accessors for xdot_color_stop
float get_frac(xdot_color_stop* stop) { return stop->frac; }
char* get_color_stop(xdot_color_stop* stop) { return stop->color; }

// Accessors for xdot_polyline
size_t get_cnt_polyline(xdot_polyline* polyline) { return polyline->cnt; }
xdot_point* get_pts_polyline(xdot_polyline* polyline) { return polyline->pts; }

// Accessors for xdot_point
double get_x_point(xdot_point* point) { return point->x; }
double get_y_point(xdot_point* point) { return point->y; }
double get_z_point(xdot_point* point) { return point->z; }

// Accessors for xdot_rect
double get_x_rect(xdot_rect* rect) { return rect->x; }
double get_y_rect(xdot_rect* rect) { return rect->y; }
double get_w_rect(xdot_rect* rect) { return rect->w; }
double get_h_rect(xdot_rect* rect) { return rect->h; }

// Index function for xdot_color_stop array
xdot_color_stop* get_color_stop_at_index(xdot_color_stop* stops, int index) { return &stops[index]; }

// Index function for xdot_op array
xdot_op* get_op_at_index(xdot_op* ops, int index) { return &ops[index]; }

// Index function for xdot_point array
xdot_point* get_pt_at_index(xdot_point* pts, int index) { return &pts[index]; }

