#include <iostream>
#include <conio.h>
#include "GraphvizWrapper.h"

using namespace std;

int main()
{
    cout << "Make sure to set the current working directory to the repository root!" << endl;
    cout << "Running tests..." << endl;

    cout << test_agread() << endl;
    cout << test_agmemread() << endl;
    cout << test_rj_agmemread() << endl;
    cout << missing_label_repro() << endl;
    cout << stackoverflow_repro() << endl;

    cout << "Press key to exit..";
    auto c = _getch();
}
