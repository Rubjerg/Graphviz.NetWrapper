#pragma once

#define _CRT_SECURE_NO_DEPRECATE
#include <objbase.h>
#include <iostream>
#include <fstream>
#include <sstream>
#include "cgraph.h"
#include "gvc.h"
#include "types.h"
#include "xdot.h"

using namespace std;

extern "C" {

#pragma region "CGraph"
    // Some wrappers around cgraph macros
    __declspec(dllexport) Agedge_t* rj_agmkin(Agedge_t* e);
    __declspec(dllexport) Agedge_t* rj_agmkout(Agedge_t* e);
    __declspec(dllexport) Agnode_t* rj_aghead(Agedge_t* edge);
    __declspec(dllexport) Agnode_t* rj_agtail(Agedge_t* edge);
    __declspec(dllexport) int rj_ageqedge(Agedge_t* e, Agedge_t* f);

    // Some wrappers around existing cgraph functions to handle string marshaling
    __declspec(dllexport) const char* rj_agmemwrite(Agraph_t* g);
    __declspec(dllexport) Agraph_t* rj_agmemread(const char* s);
    __declspec(dllexport) const char* rj_agget(void* obj, char* name);
    __declspec(dllexport) const char* rj_agnameof(void* obj);
    __declspec(dllexport) Agraph_t* rj_agopen(char* name, int graphtype);
    __declspec(dllexport) const char* rj_sym_key(Agsym_t* sym);

    __declspec(dllexport) double node_x(Agnode_t* node);
    __declspec(dllexport) double node_y(Agnode_t* node);
    __declspec(dllexport) double node_width(Agnode_t* node);
    __declspec(dllexport) double node_height(Agnode_t* node);

    __declspec(dllexport) textlabel_t* node_label(Agnode_t* node);
    __declspec(dllexport) textlabel_t* edge_label(Agedge_t* edge);
    __declspec(dllexport) textlabel_t* graph_label(Agraph_t* graph);

    __declspec(dllexport) double label_x(textlabel_t* label);
    __declspec(dllexport) double label_y(textlabel_t* label);
    __declspec(dllexport) double label_width(textlabel_t* label);
    __declspec(dllexport) double label_height(textlabel_t* label);
    __declspec(dllexport) const char* label_text(textlabel_t* label);
    __declspec(dllexport) double label_fontsize(textlabel_t* label);
    __declspec(dllexport) const char* label_fontname(textlabel_t* label);

    __declspec(dllexport) void clone_attribute_declarations(Agraph_t* from, Agraph_t* to);
    __declspec(dllexport) void convert_to_undirected(Agraph_t* graph);
#pragma endregion

#pragma region "xdot"

    __declspec(dllexport) size_t get_cnt(xdot* xdot);
    __declspec(dllexport) xdot_op* get_ops(xdot* xdot);
    __declspec(dllexport) xdot_kind get_kind(xdot_op* op);
    __declspec(dllexport) xdot_rect* get_ellipse(xdot_op* op);
    __declspec(dllexport) xdot_polyline* get_polygon(xdot_op* op);
    __declspec(dllexport) xdot_polyline* get_polyline(xdot_op* op);
    __declspec(dllexport) xdot_polyline* get_bezier(xdot_op* op);
    __declspec(dllexport) xdot_text* get_text(xdot_op* op);
    __declspec(dllexport) xdot_image* get_image(xdot_op* op);
    __declspec(dllexport) char* get_color(xdot_op* op);
    __declspec(dllexport) xdot_color* get_grad_color(xdot_op* op);
    __declspec(dllexport) xdot_font* get_font(xdot_op* op);
    __declspec(dllexport) char* get_style(xdot_op* op);
    __declspec(dllexport) unsigned int get_fontchar(xdot_op* op);
    __declspec(dllexport) xdot_rect* get_pos(xdot_image* img);
    __declspec(dllexport) char* get_name_image(xdot_image* img);
    __declspec(dllexport) double get_size(xdot_font* font);
    __declspec(dllexport) char* get_name_font(xdot_font* font);
    __declspec(dllexport) xdot_grad_type get_type(xdot_color* clr);
    __declspec(dllexport) char* get_clr(xdot_color* clr);
    __declspec(dllexport) xdot_linear_grad* get_ling(xdot_color* clr);
    __declspec(dllexport) xdot_radial_grad* get_ring(xdot_color* clr);
    __declspec(dllexport) double get_x_text(xdot_text* txt);
    __declspec(dllexport) double get_y_text(xdot_text* txt);
    __declspec(dllexport) xdot_align get_align(xdot_text* txt);
    __declspec(dllexport) double get_width(xdot_text* txt);
    __declspec(dllexport) char* get_text_str(xdot_text* txt);
    __declspec(dllexport) double get_x0_ling(xdot_linear_grad* ling);
    __declspec(dllexport) double get_y0_ling(xdot_linear_grad* ling);
    __declspec(dllexport) double get_x1_ling(xdot_linear_grad* ling);
    __declspec(dllexport) double get_y1_ling(xdot_linear_grad* ling);
    __declspec(dllexport) int get_n_stops_ling(xdot_linear_grad* ling);
    __declspec(dllexport) xdot_color_stop* get_stops_ling(xdot_linear_grad* ling);
    __declspec(dllexport) double get_x0_ring(xdot_radial_grad* ring);
    __declspec(dllexport) double get_y0_ring(xdot_radial_grad* ring);
    __declspec(dllexport) double get_r0_ring(xdot_radial_grad* ring);
    __declspec(dllexport) double get_x1_ring(xdot_radial_grad* ring);
    __declspec(dllexport) double get_y1_ring(xdot_radial_grad* ring);
    __declspec(dllexport) double get_r1_ring(xdot_radial_grad* ring);
    __declspec(dllexport) int get_n_stops_ring(xdot_radial_grad* ring);
    __declspec(dllexport) xdot_color_stop* get_stops_ring(xdot_radial_grad* ring);
    __declspec(dllexport) float get_frac(xdot_color_stop* stop);
    __declspec(dllexport) char* get_color_stop(xdot_color_stop* stop);
    __declspec(dllexport) size_t get_cnt_polyline(xdot_polyline* polyline);
    __declspec(dllexport) xdot_point* get_pts_polyline(xdot_polyline* polyline);
    __declspec(dllexport) double get_x_point(xdot_point* point);
    __declspec(dllexport) double get_y_point(xdot_point* point);
    __declspec(dllexport) double get_z_point(xdot_point* point);
    __declspec(dllexport) double get_x_rect(xdot_rect* rect);
    __declspec(dllexport) double get_y_rect(xdot_rect* rect);
    __declspec(dllexport) double get_w_rect(xdot_rect* rect);
    __declspec(dllexport) double get_h_rect(xdot_rect* rect);
    __declspec(dllexport) xdot_color_stop* get_color_stop_at_index(xdot_color_stop* stops, int index);
    __declspec(dllexport) xdot_op* get_op_at_index(xdot_op* ops, int index);
    __declspec(dllexport) xdot_point* get_pt_at_index(xdot_point* pts, int index);
#pragma endregion

#pragma region "testing/debugging"
    __declspec(dllexport) bool echobool(bool arg);
    __declspec(dllexport) int echoint(int arg);
    __declspec(dllexport) bool return_true();
    __declspec(dllexport) bool return_false();
    __declspec(dllexport) int return1();
    __declspec(dllexport) int return_1();
    __declspec(dllexport) int stackoverflow_repro();
    __declspec(dllexport) int missing_label_repro();
    __declspec(dllexport) int test_agread();
    __declspec(dllexport) int test_agmemread();
    __declspec(dllexport) int test_rj_agmemread();
#pragma endregion
}



