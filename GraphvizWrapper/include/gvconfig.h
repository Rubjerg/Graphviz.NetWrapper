/*************************************************************************
 * Copyright (c) 2011 AT&T Intellectual Property 
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * which accompanies this distribution, and is available at
 * http://www.eclipse.org/legal/epl-v10.html
 *
 * Contributors: Details at https://graphviz.org
 *************************************************************************/

/* Header used by plugins */

#ifndef GVCONFIG_H
#define GVCONFIG_H

#include "gvplugin.h"

#ifdef __cplusplus
extern "C" {
#endif

extern void gvconfig_plugin_install_from_library(GVC_t * gvc, char *path, gvplugin_library_t *library);

#ifdef __cplusplus
}
#endif
#endif				/* GVCONFIG_H */
