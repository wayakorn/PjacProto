#include <iostream>
#include <windows.h>

#include "ImagePrinter.h"

LPCWSTR PRINTERNAME = L"Microsoft Print to PDF";
LPCWSTR FILEPATH = L"c:\\temp\\test.bmp";


#define CHECK(cond) \
  { if (!cond) { std::cout << GetLastError() << std::endl; DebugBreak(); abort(); } }

int main()
{
	if (SUCCEEDED(CoInitializeEx(nullptr, COINIT_APARTMENTTHREADED)))
	{
		ImagePrinter printer;
		CHECK(SUCCEEDED(printer.Initialize()));
		CHECK(SUCCEEDED(printer.DoPrint(PRINTERNAME, FILEPATH)));
		CoUninitialize();
		return 0;
	}
	return -1;
}
