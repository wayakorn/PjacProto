// Bmp2Prn.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <cassert>
#include <iostream>
#include <windows.h>

// Note: outBuffer needs to be deleted with the delete[] operator
void MyLoadBitmap(const wchar_t* bmpFilePath, SIZE* outImageSizeInPx, BYTE** outBuffer, int* outBufferBytes)
{
	std::wcout << L"Loading bitmap from " << bmpFilePath << L"..." << std::endl;

	HBITMAP hBitmap = (HBITMAP)LoadImage(NULL, bmpFilePath, IMAGE_BITMAP, 0, 0, LR_LOADFROMFILE);
	BITMAP bm;
	int objectBytes = GetObjectW(hBitmap, sizeof(BITMAP), (LPVOID)&bm);
	assert(objectBytes == sizeof(BITMAP));

	outImageSizeInPx->cx = bm.bmWidth;
	outImageSizeInPx->cy = bm.bmHeight;
	*outBufferBytes = GetBitmapBits(hBitmap, 0, nullptr);

	*outBuffer = new BYTE[*outBufferBytes];
	assert(*outBuffer);
	assert(::GetBitmapBits(hBitmap, *outBufferBytes, (LPVOID)*outBuffer) == *outBufferBytes);

	DeleteObject(hBitmap);
}

int main()
{
	SIZE imageSizeInPx;
	BYTE* buffer;
	int bufferSize;
	MyLoadBitmap(L"c:\\temp\\test.bmp", &imageSizeInPx, &buffer, &bufferSize);

    std::wcout << L"Bitmap loaded dimension in pixels: " << imageSizeInPx.cx <<
		", " << imageSizeInPx.cy << L" (" << bufferSize << L" bytes)." << std::endl;

}
