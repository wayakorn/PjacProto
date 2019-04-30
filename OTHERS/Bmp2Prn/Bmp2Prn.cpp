const wchar_t* FILEPATH = L"c:\\temp\\test.bmp";
const wchar_t* PRINTERNAME = L"Microsoft Print to PDF";
const float PAGE_MARGIN_IN_DIPS = 96.0f; // 1 inch

// DirectX header files
#include <d2d1_1.h>
#include <d3d11.h>
#include <dwrite.h>
#include <wincodec.h>

// Print stuff
#include <documenttarget.h>

#include <Windows.h>
#include <WinUser.h>

#include <iostream>

#define CHECK(cond) \
  { if (!cond) { DebugBreak(); } abort; }

// Note: outBuffer needs to be deleted with the delete[] operator
HBITMAP MyLoadBitmap(const wchar_t* bmpFilePath, SIZE* outImageSizeInPx, BYTE** outBuffer, int* outBufferBytes)
{
	std::wcout << L"Loading bitmap from " << bmpFilePath << L"..." << std::endl;

	HBITMAP hBitmap = (HBITMAP)LoadImage(NULL, bmpFilePath, IMAGE_BITMAP, 0, 0, LR_LOADFROMFILE);
	CHECK(hBitmap);

	BITMAP bm;
	int objectBytes = GetObjectW(hBitmap, sizeof(BITMAP), (LPVOID)&bm);
	CHECK(objectBytes == sizeof(BITMAP));

	outImageSizeInPx->cx = bm.bmWidth;
	outImageSizeInPx->cy = bm.bmHeight;
	*outBufferBytes = GetBitmapBits(hBitmap, 0, nullptr);

	*outBuffer = new BYTE[*outBufferBytes];
	CHECK(*outBuffer);
	CHECK(::GetBitmapBits(hBitmap, *outBufferBytes, (LPVOID)*outBuffer) == *outBufferBytes);

	std::wcout << L"Bitmap loaded from \"" << bmpFilePath << L"\"" << 
		L", dimension in pixels: " << outImageSizeInPx->cx << ", " << outImageSizeInPx->cy <<
		L" (" << *outBufferBytes << L" bytes)." << std::endl;

	return hBitmap;
}

SIZE MyGetPageSize(const wchar_t* printerName)
{
	SIZE result;

	HANDLE hPrinter;
	CHECK(OpenPrinter((LPWSTR)printerName, &hPrinter, nullptr));

	DWORD printerInfoBufferBytes;
	CHECK(!GetPrinter(hPrinter, 8, nullptr, 0, &printerInfoBufferBytes) && GetLastError() == ERROR_INSUFFICIENT_BUFFER);

	BYTE * printerInfoBuffer = new BYTE[printerInfoBufferBytes];
	CHECK(GetPrinter(hPrinter, 8, printerInfoBuffer, printerInfoBufferBytes, &printerInfoBufferBytes));
	CHECK(printerInfoBufferBytes >= sizeof(PRINTER_INFO_8));

	PRINTER_INFO_8 * printerInfo = (PRINTER_INFO_8*)printerInfoBuffer;
	result.cx = printerInfo->pDevMode->dmPaperWidth;
	result.cy = printerInfo->pDevMode->dmPaperLength;

	std::wcout << L"Printer info: name=\"" << printerName << L"\"" <<
		L", width=" << result.cx <<
		L", length=" << result.cy << L"." << std::endl;

	delete[] printerInfoBuffer;
	ClosePrinter(hPrinter);

	return result;
}

HRESULT MyCreateDeviceContext(const wchar_t* printerName, SIZE paperSize, SIZE bitmapSizeInPx, HBITMAP hBitmap)
{
	HRESULT hr;

	// Create WIC factory
	IWICImagingFactory2* wicFactory;
	hr = CoCreateInstance(
		CLSID_WICImagingFactory,
		nullptr,
		CLSCTX_INPROC_SERVER,
		IID_PPV_ARGS(&wicFactory)
	);
	CHECK(SUCCEEDED(hr));
	CHECK(wicFactory);

	// Create D2d factory
	D2D1_FACTORY_OPTIONS options = {};
#if defined(_DEBUG)
	options.debugLevel = D2D1_DEBUG_LEVEL_INFORMATION;
#endif
	ID2D1Factory1* d2dFactory;
	hr = D2D1CreateFactory(
		D2D1_FACTORY_TYPE_SINGLE_THREADED,
		options,
		&d2dFactory
	);
	CHECK(SUCCEEDED(hr));
	CHECK(d2dFactory);

	// Create D3dDevice
	UINT createDeviceFlags = D3D11_CREATE_DEVICE_BGRA_SUPPORT;
#ifdef _DEBUG
	createDeviceFlags |= D3D11_CREATE_DEVICE_DEBUG;
#endif
	ID3D11Device* d3dDevice;
	hr = D3D11CreateDevice(
		nullptr,       // use default adapter
		D3D_DRIVER_TYPE_HARDWARE,
		nullptr,       // no external software rasterizer
		createDeviceFlags,
		nullptr,       // use default set of feature levels
		0,
		D3D11_SDK_VERSION,
		&d3dDevice,
		nullptr,       // do not care about what feature level is chosen
		nullptr        // do not retain D3D device context
	);
	CHECK(SUCCEEDED(hr));
	CHECK(d3dDevice);

	// Get a DXGI device interface from the D3D device
	IDXGIDevice* dxgiDevice;
	hr = d3dDevice->QueryInterface(&dxgiDevice);
	CHECK(SUCCEEDED(hr));
	CHECK(dxgiDevice);

	// Create a D2D device from the DXGI device
	ID2D1Device* d2dDevice;
	hr = d2dFactory->CreateDevice(
		dxgiDevice,
		&d2dDevice
	);
	CHECK(SUCCEEDED(hr));
	CHECK(d2dDevice);

	// Create a factory for document print job
	IPrintDocumentPackageTargetFactory* documentTargetFactory;
	hr = ::CoCreateInstance(
		__uuidof(PrintDocumentPackageTargetFactory),
		nullptr,
		CLSCTX_INPROC_SERVER,
		IID_PPV_ARGS(&documentTargetFactory)
	);
	CHECK(SUCCEEDED(hr));
	CHECK(documentTargetFactory);

	// Create the XPS document target
	IPrintDocumentPackageTarget* documentTarget;
	hr = documentTargetFactory->CreateDocumentPackageTargetForPrintJob(
		printerName,                                // printer name
		L"Direct2D desktop app printing sample",    // job name
		nullptr,                                    // job output stream; when nullptr, send to printer
		nullptr,                                    // job print ticket
		&documentTarget                             // result IPrintDocumentPackageTarget object
	);
	CHECK(SUCCEEDED(hr));
	CHECK(documentTarget);

	// Create the D2D print control
	ID2D1PrintControl* printControl;
	hr = d2dDevice->CreatePrintControl(
		wicFactory,
		documentTarget,
		nullptr,
		&printControl
	);
	CHECK(SUCCEEDED(hr));
	CHECK(printControl);

	// Create a D2D Device Context dedicated for the print job
	ID2D1DeviceContext* d2dContextForPrint;
	hr = d2dDevice->CreateDeviceContext(
		D2D1_DEVICE_CONTEXT_OPTIONS_NONE,
		&d2dContextForPrint
	);
	CHECK(SUCCEEDED(hr));
	CHECK(d2dContextForPrint);
	// Create D2D commandList
	ID2D1CommandList* commandList;
	hr = d2dContextForPrint->CreateCommandList(&commandList);
	CHECK(SUCCEEDED(hr));
	CHECK(commandList);

	// Set the target commandList, which will get played back later
	d2dContextForPrint->SetTarget(commandList);

	// Calculate the imagable area
	D2D1_SIZE_F imagableArea = { 0 };
	imagableArea.width = paperSize.cx - (2 * PAGE_MARGIN_IN_DIPS);
	imagableArea.height = paperSize.cy - (2 * PAGE_MARGIN_IN_DIPS);

	// Create IWICBitmap
	IWICBitmap* wicBitmap;
	hr = wicFactory->CreateBitmapFromHBITMAP(hBitmap, nullptr, WICBitmapUseAlpha, &wicBitmap);
	CHECK(SUCCEEDED(hr));
	CHECK(wicBitmap);

    auto renderTargetProperties = D2D1::RenderTargetProperties(
        D2D1_RENDER_TARGET_TYPE_SOFTWARE,
        D2D1::PixelFormat(DXGI_FORMAT_B8G8R8A8_UNORM),
        96.0f,
        96.0f,
        D2D1_RENDER_TARGET_USAGE_NONE,
        D2D1_FEATURE_LEVEL_DEFAULT);

	// Create the render target
	ID2D1RenderTarget* renderTarget;
	hr = d2dFactory->CreateWicBitmapRenderTarget(wicBitmap, &renderTargetProperties, &renderTarget);
	CHECK(SUCCEEDED(hr));
	CHECK(renderTarget);

	// Create D2d bitmap
	ID2D1Bitmap* d2dBitmap;
	hr = renderTarget->CreateBitmapFromWicBitmap(wicBitmap, &d2dBitmap);
	CHECK(SUCCEEDED(hr));
	CHECK(d2dBitmap);

	// Draw the page into the commandList
	d2dContextForPrint->BeginDraw();
	d2dContextForPrint->DrawBitmap(d2dBitmap, D2D1::RectF(0.0f, 0.0f, bitmapSizeInPx.cx, bitmapSizeInPx.cy));
	d2dContextForPrint->EndDraw();

	// Playback the commandList onto the page and finalize it
	hr = printControl->AddPage(commandList, D2D1::SizeF(paperSize.cx, paperSize.cy), nullptr);
	CHECK(SUCCEEDED(hr));
	hr = printControl->Close();
	CHECK(SUCCEEDED(hr));

	// Cleanup
	d2dBitmap->Release();
	renderTarget->Release();
	wicBitmap->Release();

	commandList->Release();
	d2dContextForPrint->Release();
	documentTargetFactory->Release();
	documentTarget->Release();
	printControl->Release();
	dxgiDevice->Release();
	d3dDevice->Release();
	d2dFactory->Release();
	wicFactory->Release();
	return hr;
}

int main()
{
	CHECK(SUCCEEDED(::CoInitialize(nullptr)));

	HBITMAP hBitmap;
	SIZE bitmapSizeInPx;
	BYTE* bmpBuffer;
	int bmpBufferBytes;
	hBitmap = MyLoadBitmap(FILEPATH, &bitmapSizeInPx, &bmpBuffer, &bmpBufferBytes);
	CHECK(hBitmap);

	SIZE paperSize = MyGetPageSize(PRINTERNAME);
	MyCreateDeviceContext(PRINTERNAME, paperSize, bitmapSizeInPx, hBitmap);

	delete[] bmpBuffer;
	DeleteObject(hBitmap);
}
