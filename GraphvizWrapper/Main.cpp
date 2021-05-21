#define _CRT_SECURE_NO_DEPRECATE
#include "cgraph.h"
#include "gvc.h"
#include "types.h"
#include <objbase.h>
#include <iostream>
#include <fstream>
#include <sstream>
#include <conio.h>

using namespace std;

extern "C" {

	// Some wrappers around existing cgraph functions to handle string marshaling
    __declspec(dllexport) const char* rj_agmemwrite(Agraph_t * g);
    __declspec(dllexport) Agraph_t* rj_agmemread(const char* s);
    __declspec(dllexport) const char* rj_agget(void* obj, char* name);
    __declspec(dllexport) const char* rj_agnameof(void* obj);
    __declspec(dllexport) Agraph_t* rj_agopen(char* name, int graphtype);
    __declspec(dllexport) const char *rj_sym_key(Agsym_t *sym);

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
    __declspec(dllexport) void convert_to_undirected(Agraph_t *graph);


    // Test and debug functions
    __declspec(dllexport) bool echobool(bool arg);
    __declspec(dllexport) int echoint(int arg);
    __declspec(dllexport) void rj_debug();
}


char* marshalCString(const char* s)
{
    if (!s) return 0;
    int len = (int)strlen(s) + 1;
    char* ptr = (char*) CoTaskMemAlloc(len);
    strcpy_s(ptr, len, s);
    return ptr;
}

static int rj_afread(void* stream, char* buffer, int bufsize)
{
    istringstream* is = (istringstream*) stream;
    is->read(buffer, bufsize);
    int result = (int) is->gcount();
    return result;
}

static int rj_putstr(void* stream, const char *s)
{
    ostringstream* os = (ostringstream*) stream;
    (*os) << s;
    return 0;
}

static int rj_flush(void* stream)
{
    ostringstream* os = (ostringstream*) stream;
    os->flush();
    return 0;
}


static Agiodisc_t memIoDisc = {rj_afread, rj_putstr, rj_flush};
static Agdisc_t memDisc = {0, 0, &memIoDisc};

textlabel_t* node_label(Agnode_t* node) { return ND_label(node); }
textlabel_t* edge_label(Agedge_t* edge) { return ED_label(edge); }
textlabel_t* graph_label(Agraph_t* graph) { return GD_label(graph); }

// Center coords of the label
double label_x(textlabel_t* label) { return label->pos.x; } // in points
double label_y(textlabel_t* label) { return label->pos.y; } // in points
double label_width(textlabel_t* label) { return label->dimen.x; } // in points
double label_height(textlabel_t* label) { return label->dimen.y; } // in points
const char* label_text(textlabel_t* label) { return marshalCString(label->text); }
double label_fontsize(textlabel_t* label) { return label->fontsize; } // in points
const char* label_fontname(textlabel_t* label) { return marshalCString(label->fontname); }

// Center coords of the node
double node_x(Agnode_t* node) { return ND_coord(node).x; } // in points
double node_y(Agnode_t* node) { return ND_coord(node).y; } // in points
double node_width(Agnode_t* node) { return ND_width(node); } // in inches
double node_height(Agnode_t* node) { return ND_height(node); } // in inches

const char *rj_sym_key(Agsym_t *sym) { return marshalCString(sym->name); }

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
void rj_agwrite(Agraph_t * g, const char* filename)
{
    ostringstream os;
    agwrite(g, &os);
    ofstream out(filename);
    out << os.str();
    out.close();
}

// Note: for this function to work, the graph has to be created with the memDisc, e.g. using rj_agopen
const char* rj_agmemwrite(Agraph_t * g)
{
    ostringstream os;
    agwrite(g, &os);
    return marshalCString(os.str().c_str());
}

const char* rj_agget(void* obj, char* name)
{
    char* result = agget(obj, name);
    return marshalCString(result);
}

const char* rj_agnameof(void* obj)
{
    char* result = agnameof(obj);
    return marshalCString(result);
}

Agdisc_t* getdisc()
{
    return &memDisc;
}

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

bool echobool(bool arg)
{
    return arg;
}

int echoint(int arg)
{
    return arg;
}

void rj_debug()
{
}

int main()
{
    rj_debug();
}
