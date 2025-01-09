#pragma once

#define _CRT_SECURE_NO_DEPRECATE
#include <iostream>
#include <fstream>
#include <sstream>

#ifdef _WIN32
    #define STRDUP _strdup
    #define GVDLL 1
    #define API __declspec(dllexport)
#else
    #define STRDUP strdup
    #define API 
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

    API void free_str(char* str);

#pragma region "CGraph"
    // Some wrappers around cgraph macros
    API Agedge_t* rj_agmkin(Agedge_t* e);
    API Agedge_t* rj_agmkout(Agedge_t* e);
    API Agnode_t* rj_aghead(Agedge_t* edge);
    API Agnode_t* rj_agtail(Agedge_t* edge);
    API int rj_ageqedge(Agedge_t* e, Agedge_t* f);

    // Some wrappers around existing cgraph functions to handle string marshaling
    API const char* rj_agmemwrite(Agraph_t* g);
    API Agraph_t* rj_agmemread(const char* s);
    API Agraph_t* rj_agopen(char* name, int graphtype);
    API const char* rj_sym_key(Agsym_t* sym);

    API double node_x(Agnode_t* node);
    API double node_y(Agnode_t* node);
    API double node_width(Agnode_t* node);
    API double node_height(Agnode_t* node);

    API textlabel_t* node_label(Agnode_t* node);
    API textlabel_t* edge_label(Agedge_t* edge);
    API textlabel_t* graph_label(Agraph_t* graph);

    API double label_x(textlabel_t* label);
    API double label_y(textlabel_t* label);
    API double label_width(textlabel_t* label);
    API double label_height(textlabel_t* label);
    API const char* label_text(textlabel_t* label);
    API double label_fontsize(textlabel_t* label);
    API const char* label_fontname(textlabel_t* label);

    API void clone_attribute_declarations(Agraph_t* from, Agraph_t* to);
    API void convert_to_undirected(Agraph_t* graph);
#pragma endregion

#pragma region "xdot"

    API size_t get_cnt(xdot* xdot);
    API xdot_op* get_ops(xdot* xdot);
    API xdot_kind get_kind(xdot_op* op);
    API xdot_rect* get_ellipse(xdot_op* op);
    API xdot_polyline* get_polygon(xdot_op* op);
    API xdot_polyline* get_polyline(xdot_op* op);
    API xdot_polyline* get_bezier(xdot_op* op);
    API xdot_text* get_text(xdot_op* op);
    API xdot_image* get_image(xdot_op* op);
    API char* get_color(xdot_op* op);
    API xdot_color* get_grad_color(xdot_op* op);
    API xdot_font* get_font(xdot_op* op);
    API char* get_style(xdot_op* op);
    API unsigned int get_fontchar(xdot_op* op);
    API xdot_rect* get_pos(xdot_image* img);
    API char* get_name_image(xdot_image* img);
    API double get_size(xdot_font* font);
    API char* get_name_font(xdot_font* font);
    API xdot_grad_type get_type(xdot_color* clr);
    API char* get_clr(xdot_color* clr);
    API xdot_linear_grad* get_ling(xdot_color* clr);
    API xdot_radial_grad* get_ring(xdot_color* clr);
    API double get_x_text(xdot_text* txt);
    API double get_y_text(xdot_text* txt);
    API xdot_align get_align(xdot_text* txt);
    API double get_width(xdot_text* txt);
    API char* get_text_str(xdot_text* txt);
    API double get_x0_ling(xdot_linear_grad* ling);
    API double get_y0_ling(xdot_linear_grad* ling);
    API double get_x1_ling(xdot_linear_grad* ling);
    API double get_y1_ling(xdot_linear_grad* ling);
    API int get_n_stops_ling(xdot_linear_grad* ling);
    API xdot_color_stop* get_stops_ling(xdot_linear_grad* ling);
    API double get_x0_ring(xdot_radial_grad* ring);
    API double get_y0_ring(xdot_radial_grad* ring);
    API double get_r0_ring(xdot_radial_grad* ring);
    API double get_x1_ring(xdot_radial_grad* ring);
    API double get_y1_ring(xdot_radial_grad* ring);
    API double get_r1_ring(xdot_radial_grad* ring);
    API int get_n_stops_ring(xdot_radial_grad* ring);
    API xdot_color_stop* get_stops_ring(xdot_radial_grad* ring);
    API float get_frac(xdot_color_stop* stop);
    API char* get_color_stop(xdot_color_stop* stop);
    API size_t get_cnt_polyline(xdot_polyline* polyline);
    API xdot_point* get_pts_polyline(xdot_polyline* polyline);
    API double get_x_point(xdot_point* point);
    API double get_y_point(xdot_point* point);
    API double get_z_point(xdot_point* point);
    API double get_x_rect(xdot_rect* rect);
    API double get_y_rect(xdot_rect* rect);
    API double get_w_rect(xdot_rect* rect);
    API double get_h_rect(xdot_rect* rect);
    API xdot_color_stop* get_color_stop_at_index(xdot_color_stop* stops, int index);
    API xdot_op* get_op_at_index(xdot_op* ops, int index);
    API xdot_point* get_pt_at_index(xdot_point* pts, int index);
#pragma endregion

#pragma region "testing/debugging"
    API bool echobool(bool arg);
    API int echoint(int arg);
    API bool return_true();
    API bool return_false();
    API int return1();
    API int return_1();

    typedef enum {
        Val1, Val2, Val3, Val4, Val5
    } TestEnum;

    API TestEnum echo_enum(TestEnum e);
    API TestEnum return_enum1();
    API TestEnum return_enum2();
    API TestEnum return_enum5();
    API char* echo_string(char* str);
    API const char* return_empty_string();
    API const char* return_hello();
    API const char* return_copyright();
    API int stackoverflow_repro();
    API int missing_label_repro();
    API int test_agread();
    API int test_agmemread();
    API int test_rj_agmemread();
#pragma endregion
}

