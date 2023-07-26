#include <iostream>
#include <conio.h>

using namespace std;

extern "C" {
    __declspec(dllimport) int stackoverflow_repro();
	__declspec(dllimport) int missing_label_repro();
	__declspec(dllimport) int test_agread();
	__declspec(dllimport) int test_agmemread();
	__declspec(dllimport) int test_rj_agmemread();
}

int main()
{
    cout << "Running tests!" << endl;

    cout << stackoverflow_repro() << endl;
    cout << missing_label_repro() << endl;
    //cout << test_agread() << endl;
    cout << test_agmemread() << endl;
    cout << test_rj_agmemread() << endl;

    cout << "Press key to exit..";
    auto c = _getch();
}
