#define _CRT_SECURE_NO_DEPRECATE
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

TestEnum echo_enum(TestEnum e) { return e; }
TestEnum return_enum1() { return Val1; }
TestEnum return_enum2() { return Val2; }
TestEnum return_enum5() { return Val5; }

/// <param name="str">Does not take ownership</param>
/// <returns>Ownership of the function result is transferred to the caller</returns>
char* echo_string(char* str) {
    // We are not the owner of str, so we cannot return it as-is; the result
    // may already be freed when the consumer uses the string.
    // Instead, we have to duplicate the string.
    // Note that the caller has to free the returned string though, because we transfer ownership.
    return STRDUP(str);
}
/// <returns>Ownership is not returned to the caller</returns>
const char* return_empty_string() { return ""; }
/// <returns>Ownership is not returned to the caller</returns>
const char* return_hello() { return "hello"; }
/// <returns>Ownership is not returned to the caller</returns>
const char* return_copyright() { return "\xC2\xA9"; } // UTF-8 encoding for ©


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
    if (graph == nullptr)
        return 1;
    gvLayout(gvc, graph, "dot");
    gvRenderFilename(gvc, graph, "svg", "test.svg");
    gvFreeLayout(gvc, graph);
    agclose(graph);
    return 0;
}

int test_agread() {
    const char* filename = "missing-label-repro.dot";
    // Open the file for reading
    FILE* fp = fopen(filename, "r");
    if (fp == nullptr)
        return 1;
    auto graph = agread(fp, NULL);
    if (graph == nullptr)
        return 2;
    fclose(fp);
    return 0;
}

int test_agmemread() {
    const std::string filename = "missing-label-repro.dot";
    char* dotString = readFile(filename);
    if (dotString == nullptr)
        return 1;
    auto graph = agmemread(dotString);
    if (graph == nullptr)
        return 1;
    return 0;
}

int test_rj_agmemread() {
    const std::string filename = "missing-label-repro.dot";
    char* dotString = readFile(filename);
    if (dotString == nullptr)
        return 1;
    auto graph = rj_agmemread(dotString);
    if (graph == nullptr)
        return 2;
    return 0;
}

// This test fails only the first time. Rerunning it makes it work.
int missing_label_repro() {
    const std::string filename = "missing-label-repro.dot";
    char* dotString = readFile(filename);
    if (dotString == nullptr)
        return 1;
    if (renderToSvg(dotString) > 0) return 2;

    char* svgText = readFile("test.svg");
    const char* expected = ">OpenNode</text>";
    if (strstr(svgText, expected) == nullptr)
        return 3;
    return 0;
}

int stackoverflow_repro() {

    const std::string filename = "stackoverflow-repro.dot";
    char* dotString = readFile(filename);
    if (dotString == nullptr)
        return 1;
    return renderToSvg(dotString);
}
