#include <iostream>
#include <filesystem>
#include <libgen.h>
#include <string.h>
#include <unistd.h>

int run();

int main() {
    char exePath[1024];   // Buffer to store the path of the executable
    ssize_t len;

    // Read the symbolic link for the current executable
    len = readlink("/proc/self/exe", exePath, sizeof(exePath) - 1);
    if (len == -1) {
        perror("Failed to read /proc/self/exe");
        return EXIT_FAILURE;
    }

    exePath[len] = '\0';  // Null-terminate the path

    // Get the directory name of the executable
    char *dirPath = dirname(exePath);

    // Construct the GVBINDIR path (dirPath + "/graphviz")
    char gvBinDir[1024];
    snprintf(gvBinDir, sizeof(gvBinDir), "%s/graphviz", dirPath);

    // Set LD_LIBRARY_PATH to the executable's directory
    if (setenv("LD_LIBRARY_PATH", dirPath, 1) != 0) {
        perror("Failed to set LD_LIBRARY_PATH");
        return EXIT_FAILURE;
    }

    // Set GVBINDIR to the "graphviz" subdirectory
    if (setenv("GVBINDIR", gvBinDir, 1) != 0) {
        perror("Failed to set GVBINDIR");
        return EXIT_FAILURE;
    }

    // Verify the environment variables
    printf("Executable Directory: %s\n", dirPath);
    printf("LD_LIBRARY_PATH = %s\n", getenv("LD_LIBRARY_PATH"));
    printf("GVBINDIR = %s\n", getenv("GVBINDIR"));

    return run();
}

#ifdef _WIN32
    #include <windows.h>
#else
    #include <dlfcn.h>
#endif

#include "../GraphvizWrapper/GraphvizWrapper.h"

using namespace std;

int run() {
    cout << "Make sure to set the current working directory to the repository root!" << endl;
    cout << "Running tests..." << endl;
    
    cout << test_agread() << endl;
    cout << test_agmemread() << endl;
    cout << test_rj_agmemread() << endl;
    cout << missing_label_repro() << endl;
    cout << stackoverflow_repro() << endl;
    
    cout << "Press Enter to exit...";
    cin.get(); // Portable replacement for _getch()
    
    return 0;
}

