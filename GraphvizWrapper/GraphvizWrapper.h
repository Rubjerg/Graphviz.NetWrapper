#pragma once

#define _CRT_SECURE_NO_DEPRECATE
#include <iostream>
#include <fstream>
#include <sstream>

#ifdef _WIN32
#define GVDLL 1
#endif

#ifdef _WIN32
    #define STRDUP _strdup
#else
    #define STRDUP strdup
#endif

#include "cgraph.h"
#include "gvc.h"
#include "types.h"
#include "xdot.h"


using namespace std;


static int rj_afread(void* stream, char* buffer, int bufsize)
{
    istringstream* is = (istringstream*)stream;
    is->read(buffer, bufsize);
    int result = (int)is->gcount();
    return result;
}

static int rj_putstr(void* stream, const char* s)
{
    ostringstream* os = (ostringstream*)stream;
    (*os) << s;
    return 0;
}

static int rj_flush(void* stream)
{
    ostringstream* os = (ostringstream*)stream;
    os->flush();
    return 0;
}

static Agiodisc_t ioDisc = { rj_afread, rj_putstr, rj_flush };
static Agdisc_t disc = { 0, &ioDisc };

extern "C" {

    CGRAPH_API void free_str(char* str);

#pragma region "CGraph"
    // Some wrappers around cgraph macros
    CGRAPH_API Agedge_t* rj_agmkin(Agedge_t* e);
    CGRAPH_API Agedge_t* rj_agmkout(Agedge_t* e);
    CGRAPH_API Agnode_t* rj_aghead(Agedge_t* edge);
    CGRAPH_API Agnode_t* rj_agtail(Agedge_t* edge);
    CGRAPH_API int rj_ageqedge(Agedge_t* e, Agedge_t* f);

    // Some wrappers around existing cgraph functions to handle string marshaling
    CGRAPH_API const char* rj_agmemwrite(Agraph_t* g);
    CGRAPH_API Agraph_t* rj_agmemread(const char* s);
    CGRAPH_API Agraph_t* rj_agopen(char* name, int graphtype);
    CGRAPH_API const char* rj_sym_key(Agsym_t* sym);

    CGRAPH_API double node_x(Agnode_t* node);
    CGRAPH_API double node_y(Agnode_t* node);
    CGRAPH_API double node_width(Agnode_t* node);
    CGRAPH_API double node_height(Agnode_t* node);

    CGRAPH_API textlabel_t* node_label(Agnode_t* node);
    CGRAPH_API textlabel_t* edge_label(Agedge_t* edge);
    CGRAPH_API textlabel_t* graph_label(Agraph_t* graph);

    CGRAPH_API double label_x(textlabel_t* label);
    CGRAPH_API double label_y(textlabel_t* label);
    CGRAPH_API double label_width(textlabel_t* label);
    CGRAPH_API double label_height(textlabel_t* label);
    CGRAPH_API const char* label_text(textlabel_t* label);
    CGRAPH_API double label_fontsize(textlabel_t* label);
    CGRAPH_API const char* label_fontname(textlabel_t* label);

    CGRAPH_API void clone_attribute_declarations(Agraph_t* from, Agraph_t* to);
    CGRAPH_API void convert_to_undirected(Agraph_t* graph);
#pragma endregion

#pragma region "xdot"

    CGRAPH_API size_t get_cnt(xdot* xdot);
    CGRAPH_API xdot_op* get_ops(xdot* xdot);
    CGRAPH_API xdot_kind get_kind(xdot_op* op);
    CGRAPH_API xdot_rect* get_ellipse(xdot_op* op);
    CGRAPH_API xdot_polyline* get_polygon(xdot_op* op);
    CGRAPH_API xdot_polyline* get_polyline(xdot_op* op);
    CGRAPH_API xdot_polyline* get_bezier(xdot_op* op);
    CGRAPH_API xdot_text* get_text(xdot_op* op);
    CGRAPH_API xdot_image* get_image(xdot_op* op);
    CGRAPH_API char* get_color(xdot_op* op);
    CGRAPH_API xdot_color* get_grad_color(xdot_op* op);
    CGRAPH_API xdot_font* get_font(xdot_op* op);
    CGRAPH_API char* get_style(xdot_op* op);
    CGRAPH_API unsigned int get_fontchar(xdot_op* op);
    CGRAPH_API xdot_rect* get_pos(xdot_image* img);
    CGRAPH_API char* get_name_image(xdot_image* img);
    CGRAPH_API double get_size(xdot_font* font);
    CGRAPH_API char* get_name_font(xdot_font* font);
    CGRAPH_API xdot_grad_type get_type(xdot_color* clr);
    CGRAPH_API char* get_clr(xdot_color* clr);
    CGRAPH_API xdot_linear_grad* get_ling(xdot_color* clr);
    CGRAPH_API xdot_radial_grad* get_ring(xdot_color* clr);
    CGRAPH_API double get_x_text(xdot_text* txt);
    CGRAPH_API double get_y_text(xdot_text* txt);
    CGRAPH_API xdot_align get_align(xdot_text* txt);
    CGRAPH_API double get_width(xdot_text* txt);
    CGRAPH_API char* get_text_str(xdot_text* txt);
    CGRAPH_API double get_x0_ling(xdot_linear_grad* ling);
    CGRAPH_API double get_y0_ling(xdot_linear_grad* ling);
    CGRAPH_API double get_x1_ling(xdot_linear_grad* ling);
    CGRAPH_API double get_y1_ling(xdot_linear_grad* ling);
    CGRAPH_API int get_n_stops_ling(xdot_linear_grad* ling);
    CGRAPH_API xdot_color_stop* get_stops_ling(xdot_linear_grad* ling);
    CGRAPH_API double get_x0_ring(xdot_radial_grad* ring);
    CGRAPH_API double get_y0_ring(xdot_radial_grad* ring);
    CGRAPH_API double get_r0_ring(xdot_radial_grad* ring);
    CGRAPH_API double get_x1_ring(xdot_radial_grad* ring);
    CGRAPH_API double get_y1_ring(xdot_radial_grad* ring);
    CGRAPH_API double get_r1_ring(xdot_radial_grad* ring);
    CGRAPH_API int get_n_stops_ring(xdot_radial_grad* ring);
    CGRAPH_API xdot_color_stop* get_stops_ring(xdot_radial_grad* ring);
    CGRAPH_API float get_frac(xdot_color_stop* stop);
    CGRAPH_API char* get_color_stop(xdot_color_stop* stop);
    CGRAPH_API size_t get_cnt_polyline(xdot_polyline* polyline);
    CGRAPH_API xdot_point* get_pts_polyline(xdot_polyline* polyline);
    CGRAPH_API double get_x_point(xdot_point* point);
    CGRAPH_API double get_y_point(xdot_point* point);
    CGRAPH_API double get_z_point(xdot_point* point);
    CGRAPH_API double get_x_rect(xdot_rect* rect);
    CGRAPH_API double get_y_rect(xdot_rect* rect);
    CGRAPH_API double get_w_rect(xdot_rect* rect);
    CGRAPH_API double get_h_rect(xdot_rect* rect);
    CGRAPH_API xdot_color_stop* get_color_stop_at_index(xdot_color_stop* stops, int index);
    CGRAPH_API xdot_op* get_op_at_index(xdot_op* ops, int index);
    CGRAPH_API xdot_point* get_pt_at_index(xdot_point* pts, int index);
#pragma endregion

#pragma region "testing/debugging"
    CGRAPH_API bool echobool(bool arg);
    CGRAPH_API int echoint(int arg);
    CGRAPH_API bool return_true();
    CGRAPH_API bool return_false();
    CGRAPH_API int return1();
    CGRAPH_API int return_1();

    typedef enum {
        Val1, Val2, Val3, Val4, Val5
    } TestEnum;

    CGRAPH_API TestEnum echo_enum(TestEnum e);
    CGRAPH_API TestEnum return_enum1();
    CGRAPH_API TestEnum return_enum2();
    CGRAPH_API TestEnum return_enum5();
    CGRAPH_API char* echo_string(char* str);
    CGRAPH_API const char* return_empty_string();
    CGRAPH_API const char* return_hello();
    CGRAPH_API const char* return_copyright();
    CGRAPH_API int stackoverflow_repro();
    CGRAPH_API int missing_label_repro();
    CGRAPH_API int test_agread();
    CGRAPH_API int test_agmemread();
    CGRAPH_API int test_rj_agmemread();
#pragma endregion
}

