#define _CRT_SECURE_NO_DEPRECATE
#include <objbase.h>
#include <iostream>
#include <fstream>
#include <sstream>
#include "GraphvizWrapper.h"

using namespace std;

void free_str(char* str) {
    free(str);
}

Agraph_t* rj_agopen(char* name, int graphtype)
{
    if (graphtype == 0)
        return agopen(name, Agdirected, &memDisc);
    if (graphtype == 1)
        return agopen(name, Agstrictdirected, &memDisc);
    if (graphtype == 2)
        return agopen(name, Agundirected, &memDisc);
    if (graphtype == 3)
        return agopen(name, Agstrictundirected, &memDisc);
    return 0;
}

Agraph_t* rj_agmemread(const char* s)
{
    stringstream stream;
    stream << s;
    Agraph_t* g = agread(&stream, &memDisc);
    return g;
}

// Note: for this function to work, the graph has to be created with the memDisc, e.g. using rj_agopen
// This function transfers ownership of the string result.
const char* rj_agmemwrite(Agraph_t* g)
{
    ostringstream os;
    agwrite(g, &os);
    // Note that os and os.str() will go out of scope when this function returns.
    // Therefore we need to strdup the underlying c string
    return _strdup(os.str().c_str());
}


// Expose removed cgraph functions https://gitlab.com/graphviz/graphviz/-/issues/2433
Agnode_t* rj_aghead(Agedge_t* edge)
{
    return AGHEAD(edge);
}
Agnode_t* rj_agtail(Agedge_t* edge)
{
    return AGTAIL(edge);
}
int rj_ageqedge(Agedge_t* e, Agedge_t* f)
{
    return AGEQEDGE(e, f);
}
Agedge_t* rj_agmkin(Agedge_t* e)
{
    return AGMKIN(e);
}
Agedge_t* rj_agmkout(Agedge_t* e)
{
    return AGMKOUT(e);
}

textlabel_t* node_label(Agnode_t* node) { return ND_label(node); }
textlabel_t* edge_label(Agedge_t* edge) { return ED_label(edge); }
textlabel_t* graph_label(Agraph_t* graph) { return GD_label(graph); }

// Center coords of the label
double label_x(textlabel_t* label) { return label->pos.x; } // in points
double label_y(textlabel_t* label) { return label->pos.y; } // in points
double label_width(textlabel_t* label) { return label->dimen.x; } // in points
double label_height(textlabel_t* label) { return label->dimen.y; } // in points
const char* label_text(textlabel_t* label) { return label->text; }
double label_fontsize(textlabel_t* label) { return label->fontsize; } // in points
const char* label_fontname(textlabel_t* label) { return label->fontname; }

// Center coords of the node
double node_x(Agnode_t* node) { return ND_coord(node).x; } // in points
double node_y(Agnode_t* node) { return ND_coord(node).y; } // in points
double node_width(Agnode_t* node) { return ND_width(node); } // in inches
double node_height(Agnode_t* node) { return ND_height(node); } // in inches

const char* rj_sym_key(Agsym_t* sym) { return sym->name; }

void clone_attribute_declarations(Agraph_t* from, Agraph_t* to)
{
    for (int kind = 0; kind < 3; kind++)
    {
        Agsym_t* current = agnxtattr(from, kind, NULL);
        while (current)
        {
            agattr(to, kind, current->name, current->defval);
            current = agnxtattr(from, kind, current);
        }
    }
}

void convert_to_undirected(Agraph_t* graph)
{
    graph->desc.directed = 0;
}
