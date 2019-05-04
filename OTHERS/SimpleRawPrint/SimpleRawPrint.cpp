// SimpleRawPrint.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include "pch.h"
#include <iostream>
#include <windows.h>


#if 0
// RawDataToPrinter - sends binary data directly to a printer 
//  
// szPrinterName: NULL-terminated string specifying printer name 
// lpData:        Pointer to raw data bytes 
// dwCount        Length of lpData in bytes 
//  
// Returns: TRUE for success, FALSE for failure. 
//  
BOOL RawDataToPrinter(LPWSTR szPrinterName, LPBYTE lpData, DWORD dwCount)
{
	BOOL     bStatus = FALSE;
	HANDLE     hPrinter = NULL;
	DOC_INFO_1 DocInfo;
	DWORD      dwJob = 0L;
	DWORD      dwBytesWritten = 0L;

	// Open a handle to the printer. 
	bStatus = OpenPrinterW((LPWSTR)szPrinterName, &hPrinter, NULL);  // question 1
	if (bStatus) {
		// Fill in the structure with info about this "document." 
		DocInfo.pDocName = (LPTSTR)L"SimpleRawPrint";  // question 2
		DocInfo.pOutputFile = NULL;                 // question 3
		DocInfo.pDatatype = (LPTSTR)L"RAW";   // question 4

		// Inform the spooler the document is beginning. 
		dwJob = StartDocPrinter(hPrinter, 1, (LPBYTE)&DocInfo);  // question 5
		if (dwJob > 0) {
			// Start a page. 
			bStatus = StartPagePrinter(hPrinter);
			if (bStatus) {
				// Send the data to the printer. 
				bStatus = WritePrinter(hPrinter, lpData, dwCount, &dwBytesWritten);
				EndPagePrinter(hPrinter);
			}
			// Inform the spooler that the document is ending. 
			EndDocPrinter(hPrinter);
		}
		// Close the printer handle. 
		ClosePrinter(hPrinter);
	}
	// Check to see if correct number of bytes were written. 
	if (!bStatus || (dwBytesWritten != dwCount)) {
		bStatus = FALSE;
	}
	else {
		bStatus = TRUE;
	}
	return bStatus;
}

int main()
{

	std::string buffer = "hello world!";

	if (RawDataToPrinter((LPWSTR)L"Microsoft Print to PDF", (LPBYTE)buffer.data(), buffer.length()))
	{
		std::cout << "Hello ok!\n";

	}
	else {
		std::cout << "Hello not ok!\n";
	}
}
#endif



void DoPrint()
{
	HWND hWnd = GetDesktopWindow();

	PRINTDLG pd = { 0 };
	pd.lStructSize = sizeof(pd);
	pd.hwndOwner = hWnd;
	pd.Flags = PD_RETURNDC;

	// Retrieves the printer DC
	if (PrintDlg(&pd))
	{
		DOCINFOW di = { 0 };
		di.cbSize = sizeof(DOCINFOW);
		di.lpszDocName = L"SimplePrint";

		HDC hdc = pd.hDC;
		StartDoc(hdc, &di);
		StartPage(hdc);

		// Drawing code begin
		//    
		RECT rc;
		rc.top = 100;
		rc.left = 100;
		rc.bottom = 300;
		rc.right = 300;

		HBRUSH greenBrush = CreateSolidBrush(RGB(0, 255, 0));
		FillRect(hdc, &rc, greenBrush);
		DeleteObject(greenBrush);
		//
		// Drawing code end

		EndPage(hdc);
		EndDoc(hdc);
		DeleteObject(hdc);
	}
}
int main()
{

	std::string buffer = "hello world!";

	DoPrint();
}
