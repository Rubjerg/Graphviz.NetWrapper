/// @file
/// @ingroup public_apis
/*************************************************************************
 * Copyright (c) 2011 AT&T Intellectual Property 
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * which accompanies this distribution, and is available at
 * https://www.eclipse.org/legal/epl-v10.html
 *
 * Contributors: Details at https://graphviz.org
 *************************************************************************/

/* geometric functions (e.g. on points and boxes) with application to, but
 * no specific dependence on graphs */

#pragma once

/* for sincos */
#ifndef _GNU_SOURCE
#define _GNU_SOURCE 1
#endif

#include <limits.h>
#include <math.h>

#ifdef __cplusplus
extern "C" {
#endif

#ifdef MIN
#undef MIN
#endif
#define MIN(a,b)	((a)<(b)?(a):(b))

#ifdef MAX
#undef MAX
#endif
#define MAX(a,b)	((a)>(b)?(a):(b))

#ifndef MAXDOUBLE
#define MAXDOUBLE	1.7976931348623157e+308
#endif

#ifdef BETWEEN
#undef BETWEEN
#endif
#define BETWEEN(a,b,c)	(((a) <= (b)) && ((b) <= (c)))

#ifndef M_PI
#define M_PI		3.14159265358979323846
#endif

#ifndef SQRT2
#define SQRT2		1.41421356237309504880
#endif

#define ROUND(f)        ((f>=0)?(int)(f + .5):(int)(f - .5))
#define RADIANS(deg)	((deg)/180.0 * M_PI)
#define DEGREES(rad)	((rad)/M_PI * 180.0)

#define SQR(a) ((a) * (a))

#ifdef HAVE_SINCOS
    extern void sincos(double x, double *s, double *c);
#else
# define sincos(x,s,c) *s = sin(x); *c = cos(x)
#endif

#ifdef __cplusplus
}
#endif
