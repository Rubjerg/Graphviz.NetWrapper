#include <iostream>
#include <string.h>

#ifdef _WIN32
    #include <windows.h>
#else
    #include <dlfcn.h>
#endif

#include "../GraphvizWrapper/GraphvizWrapper.h"

using namespace std;

int main() {
    cout << "Make sure to set the current working directory to the repository root!" << endl;
    cout << "Running tests..." << endl;
    
    cout << test_agread() << endl;
    cout << test_agmemread() << endl;
    cout << test_rj_agmemread() << endl;
    cout << missing_label_repro() << endl;
    cout << stackoverflow_repro() << endl;
    cout << agclose_repro() << endl;
    
    cout << "Press Enter to exit...";
    cin.get(); // Portable replacement for _getch()
    
    return 0;
}

