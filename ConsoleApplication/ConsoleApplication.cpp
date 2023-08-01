#include <iostream>
#include <conio.h>

using namespace std;

extern "C" {
    __declspec(dllimport) int stackoverflow_repro();
    __declspec(dllimport) int missing_label_repro();
    __declspec(dllimport) int test_agread();
    __declspec(dllimport) int test_agmemread();
    __declspec(dllimport) int test_rj_agmemread();
    __declspec(dllimport) int test_xdot();
}

int main()
{
    cout << "Make sure to set the current working directory to the repository root!" << endl;
    cout << "Running tests..." << endl;

    cout << test_xdot() << endl;
    cout << test_agread() << endl;
    cout << test_agmemread() << endl;
    cout << test_rj_agmemread() << endl;
    cout << missing_label_repro() << endl;
    cout << stackoverflow_repro() << endl;

    cout << "Press key to exit..";
    auto c = _getch();
}
