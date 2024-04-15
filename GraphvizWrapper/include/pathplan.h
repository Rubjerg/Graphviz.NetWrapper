/**
 * @file
 * @brief finds and smooths shortest paths
 * @ingroup public_apis
 *
 * **libpathplan** provides functions for creating spline paths in the plane
 * that are constrained by a polygonal boundary or obstacles to avoid.
 * All polygons must be simple, but need not be convex.
 *
 * [man 3 pathplan](https://graphviz.org/pdf/pathplan.3.pdf)
 *
 */

/*************************************************************************
 * Copyright (c) 2011 AT&T Intellectual Property 
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * which accompanies this distribution, and is available at
 * https://www.eclipse.org/legal/epl-v10.html
 *
 * Contributors: Details at https://graphviz.org
 *************************************************************************/

#pragma once

#include "pathgeom.h"
#include <stddef.h>

#ifdef __cplusplus
extern "C" {
#endif

#ifdef GVDLL
#ifdef PATHPLAN_EXPORTS
#define PATHPLAN_API __declspec(dllexport)
#else
#define PATHPLAN_API __declspec(dllimport)
#endif
#endif

#ifndef PATHPLAN_API
#define PATHPLAN_API /* nothing */
#endif

/* find shortest euclidean path within a simple polygon */
    PATHPLAN_API int Pshortestpath(Ppoly_t * boundary, Ppoint_t endpoints[2],
			     Ppolyline_t * output_route);

/* fit a spline to an input polyline, without touching barrier segments */
PATHPLAN_API int Proutespline(Pedge_t *barriers, size_t n_barriers,
                              Ppolyline_t input_route,
                              Pvector_t endpoint_slopes[2],
                              Ppolyline_t *output_route);

/* utility function to convert from a set of polygonal obstacles to barriers */
    PATHPLAN_API int Ppolybarriers(Ppoly_t ** polys, int npolys,
			     Pedge_t ** barriers, int *n_barriers);

/* function to convert a polyline into a spline representation */
    PATHPLAN_API void make_polyline(Ppolyline_t line, Ppolyline_t* sline);

#undef PATHPLAN_API

#ifdef __cplusplus
}
#endif
