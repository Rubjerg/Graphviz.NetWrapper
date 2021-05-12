#define _CRT_SECURE_NO_DEPRECATE
#include "cgraph.h"
#include "gvc.h"
#include "types.h"
#include <objbase.h>
#include <iostream>
#include <fstream>
#include <sstream>

using namespace std;

extern "C"
{

    // Some wrappers around existing cgraph functions to handle string marshaling
    //__declspec(dllexport) const char* imagmemwrite(Agraph_t * g);
    //__declspec(dllexport) Agraph_t* imagmemread(const char* s);
    __declspec(dllexport) void imagwrite(Agraph_t *g, char *filename);
    __declspec(dllexport) Agraph_t *imagread(char *filename);
    __declspec(dllexport) const char *imagget(void *obj, char *name);
    __declspec(dllexport) const char *imagnameof(void *obj);
    __declspec(dllexport) Agraph_t *imagopen(char *name, int graphtype);
    __declspec(dllexport) const char *imsym_key(Agsym_t *sym);

    __declspec(dllexport) double node_x(Agnode_t *node);
    __declspec(dllexport) double node_y(Agnode_t *node);
    __declspec(dllexport) double node_width(Agnode_t *node);
    __declspec(dllexport) double node_height(Agnode_t *node);

    __declspec(dllexport) textlabel_t *node_label(Agnode_t *node);
    __declspec(dllexport) textlabel_t *edge_label(Agedge_t *edge);
    __declspec(dllexport) textlabel_t *graph_label(Agraph_t *graph);

    __declspec(dllexport) double label_x(textlabel_t *label);
    __declspec(dllexport) double label_y(textlabel_t *label);
    __declspec(dllexport) double label_width(textlabel_t *label);
    __declspec(dllexport) double label_height(textlabel_t *label);
    __declspec(dllexport) const char *label_text(textlabel_t *label);
    __declspec(dllexport) double label_fontsize(textlabel_t *label);
    __declspec(dllexport) const char *label_fontname(textlabel_t *label);

    __declspec(dllexport) void clone_attribute_declarations(Agraph_t *from, Agraph_t *to);
    __declspec(dllexport) void convert_to_undirected(Agraph_t *graph);

    // Test and debug functions
    __declspec(dllexport) bool echobool(bool arg);
    __declspec(dllexport) int echoint(int arg);
    __declspec(dllexport) void imdebug();
}

char *marshalCString(const char *s)
{
    if (!s)
        return 0;
    int len = (int)strlen(s) + 1;
    char *ptr = (char *)CoTaskMemAlloc(len);
    strcpy_s(ptr, len, s);
    return ptr;
}

static int imafread(void *stream, char *buffer, int bufsize)
{
    istream *is = (istream *)stream;
    is->read(buffer, bufsize);
    int result = (int)is->gcount();
    return result;
}

static int imputstr(void *stream, const char *s)
{
    ostream *os = (ostream *)stream;
    (*os) << s;
    return 0;
}

static int imflush(void *stream)
{
    ostream *os = (ostream *)stream;
    os->flush();
    return 0;
}

//static Agiodisc_t memIoDisc = {imafread, imputstr, imflush};
//static Agdisc_t memDisc = {0, 0, &memIoDisc};

textlabel_t *node_label(Agnode_t *node) { return ND_label(node); }
textlabel_t *edge_label(Agedge_t *edge) { return ED_label(edge); }
textlabel_t *graph_label(Agraph_t *graph) { return GD_label(graph); }

// Center coords of the label
double label_x(textlabel_t *label) { return label->pos.x; }        // in points
double label_y(textlabel_t *label) { return label->pos.y; }        // in points
double label_width(textlabel_t *label) { return label->dimen.x; }  // in points
double label_height(textlabel_t *label) { return label->dimen.y; } // in points
const char *label_text(textlabel_t *label) { return marshalCString(label->text); }
double label_fontsize(textlabel_t *label) { return label->fontsize; } // in points
const char *label_fontname(textlabel_t *label) { return marshalCString(label->fontname); }

// Center coords of the node
double node_x(Agnode_t *node) { return ND_coord(node).x; }     // in points
double node_y(Agnode_t *node) { return ND_coord(node).y; }     // in points
double node_width(Agnode_t *node) { return ND_width(node); }   // in inches
double node_height(Agnode_t *node) { return ND_height(node); } // in inches

const char *imsym_key(Agsym_t *sym) { return marshalCString(sym->name); }

Agraph_t *imagopen(char *name, int graphtype)
{
    if (graphtype == 0)
        return agopen(name, Agdirected, 0);
    if (graphtype == 1)
        return agopen(name, Agstrictdirected, 0);
    if (graphtype == 2)
        return agopen(name, Agundirected, 0);
    if (graphtype == 3)
        return agopen(name, Agstrictundirected, 0);
    return 0;
}

//Agraph_t* imagmemread(const char* s)
//{
//    stringstream stream(s);
//    Agraph_t* g = agread(&stream, &memDisc);
//    return g;
//}

// Note: for this function to work, the graph has to be created with the default io disc
void imagwrite(Agraph_t *g, char *filename)
{
    FILE *file;
    file = fopen(filename, "w");
    agwrite(g, file);
    fclose(file);
}

// Note: for this function to work, the graph has to be created with the default io disc
Agraph_t *imagread(char *filename)
{
    FILE *file;
    file = fopen(filename, "r");
    auto result = agread(file, 0);
    fclose(file);
    return result;
}

// Note: for this function to work, the graph has to be created with the memDisc, e.g. using imagopen
//const char* imagmemwrite(Agraph_t * g)
//{
//    ostringstream os;
//    agwrite(g, &os);
//    return marshalCString(os.str().c_str());
//}

const char *imagget(void *obj, char *name)
{
    char *result = agget(obj, name);
    return marshalCString(result);
}

const char *imagnameof(void *obj)
{
    char *result = agnameof(obj);
    return marshalCString(result);
}

void clone_attribute_declarations(Agraph_t *from, Agraph_t *to)
{
    for (int kind = 0; kind < 3; kind++)
    {
        Agsym_t *current = agnxtattr(from, kind, NULL);
        while (current)
        {
            agattr(to, kind, current->name, current->defval);
            current = agnxtattr(from, kind, current);
        }
    }
}

void convert_to_undirected(Agraph_t *graph)
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

void imdebug()
{
    // Create reproduction dot file
    FILE *file;
    file = fopen("input.dot", "w");
    const char *inputstr = "digraph \"problem graph\" { node[fontname = \"Times-Roman\", fontsize = 7, margin = 0.01 ]; A[label = \"{20 VH|{1|2}}\", shape = record]; }";
    fprintf(file, inputstr);
    fclose(file);

    // Read reproduction graph
    file = fopen("input.dot", "r");
    auto root = agread(file, 0);
    fclose(file);

    // Compute layout using library calls
    system("bash -c 'echo Using lib calls: >> testout.txt '");
    auto gvc = gvContext();
    gvLayout(gvc, root, "dot");
    gvRender(gvc, root, "xdot", NULL);
    file = fopen("output.dot", "w");
    agwrite(root, file);
    fclose(file);
    system("bash -c 'cat output.dot >> testout.txt '");

    // Compute layout using dot.exe
    system("bash -c 'echo Using dot.exe: >> testout.txt '");
    system("bash -c 'Rubjerg.Graphviz\\dot.exe -Txdot input.dot >> testout.txt '");
}

int main()
{
    imdebug();
}
