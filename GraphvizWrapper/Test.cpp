#define _CRT_SECURE_NO_DEPRECATE
#include <objbase.h>
#include <iostream>
#include <fstream>
#include <sstream>
#include "GraphvizWrapper.h"

bool echobool(bool arg) { return arg; }
int echoint(int arg) { return arg; }
bool return_true() { return true; }
bool return_false() { return false; }
int return1() { return 1; }
int return_1() { return -1; }

char* readFile(const std::string& filename) {
    std::ifstream file(filename, std::ios::binary | std::ios::ate);

    if (!file) {
        std::cerr << "Failed to open file: " << filename << std::endl;
        return nullptr;
    }

    std::streamsize size = file.tellg();
    file.seekg(0, std::ios::beg);

    char* buffer = new char[size + 1];  // allocate one extra byte
    if (!file.read(buffer, size)) {
        std::cerr << "Failed to read file: " << filename << std::endl;
        delete[] buffer;
        return nullptr;
    }

    buffer[size] = '\0';  // null terminate the string
    return buffer;
}

int renderToSvg(char* dotString)
{
    // NOTE: the gvContext has to be called first
    // See https://gitlab.com/graphviz/graphviz/-/issues/2434
    auto gvc = gvContext();
    auto graph = agmemread(dotString);
    if (graph == NULL)
        return 1;
    gvLayout(gvc, graph, "dot");
    gvRenderFilename(gvc, graph, "svg", "test.svg");
    gvFreeLayout(gvc, graph);
    agclose(graph);
    return 0;
}

// This test fails only the first time. Rerunning it makes it work.
int missing_label_repro() {
    const std::string filename = "missing-label-repro.dot";
    char* dotString = readFile(filename);
    if (dotString == NULL)
        return 1;
    if (renderToSvg(dotString) > 0) return 2;

    char* svgText = readFile("test.svg");
    char* expected = ">OpenNode</text>";
    if (strstr(svgText, expected) == NULL)
        return 3;
    return 0;
}

int stackoverflow_repro() {

    const std::string filename = "stackoverflow-repro.dot";
    char* dotString = readFile(filename);
    if (dotString == NULL)
        return 1;
    return renderToSvg(dotString);
}


int test_agread() {
    char* filename = "missing-label-repro.dot";
    // Open the file for reading
    FILE* fp = fopen(filename, "r");
    if (fp == NULL)
        return 1;
    auto graph = agread(fp, NULL);
    if (graph == 0)
        return 2;
    fclose(fp);
    return 0;
}

int test_agmemread() {
    const std::string filename = "missing-label-repro.dot";
    char* dotString = readFile(filename);
    if (dotString == NULL)
        return 1;
    auto graph = agmemread(dotString);
    if (graph == 0)
        return 1;
    return 0;
}

int test_rj_agmemread() {
    const std::string filename = "missing-label-repro.dot";
    char* dotString = readFile(filename);
    if (dotString == NULL)
        return 1;
    auto graph = rj_agmemread(dotString);
    if (graph == NULL)
        return 2;
    return 0;
}


int test_xdot() {
    char* str = "c 9 -#fffffe00 C 7 -#ffffff P 4 0 0 0 72.25 136.5 72.25 136.5 0";
//    char* str = "c 7 -#000000 p 4 569.18 36.75 569.18 81.51 590.82 81.51 590.82 36.75 c 7 -#000000 L 2 569.18 70.32 581.12 70.32 c 7 -#000000 L 2 \
//569.18 59.13 581.12 59.13 c 7 -#000000 L 2 581.12 47.94 581.12 81.51 c 7 -#000000 L 2 581.12 70.32 590.82 70.32 c 7 -#000000 L 2 \
//581.12 59.13 590.82 59.13 c 7 -#000000 L 2 569.18 47.94 590.82 47.94 ";
    auto xdot = parseXDot(str);
    auto ops = get_ops(xdot);
    auto op = get_op_at_index(ops, 0);
    auto kind = get_kind(op);
    cout << kind << endl;
    return 0;
}
