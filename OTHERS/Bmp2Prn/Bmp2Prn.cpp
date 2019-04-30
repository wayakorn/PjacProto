const wchar_t* FILEPATH = L"c:\\temp\\test.bmp";
const wchar_t* PRINTERNAME = L"Microsoft Print to PDF";
const float PAGE_MARGIN_IN_DIPS = 96.0f; // 1 inch
const float DEFAULT_DPI = 96.0f;
const float FRAME_HEIGHT_IN_DIPS = 400.0f;           // 400 DIPs

// DirectX header files
#include <d2d1_1.h>
#include <d3d11.h>
#include <dwrite.h>
#include <wincodec.h>

// Print stuff
#include <documenttarget.h>

#include <Windows.h>
#include <WinUser.h>

#include <atlcomcli.h>

#include <iostream>

#define CHECK(cond) \
  { if (!cond) { std::cout << GetLastError() << std::endl; DebugBreak(); } abort; }


SIZE MyGetPageSize(const wchar_t* printerName)
{
	SIZE result;

	HANDLE hPrinter;
	CHECK(OpenPrinter((LPWSTR)printerName, &hPrinter, nullptr));

	DWORD printerInfoBufferBytes;
	CHECK(!GetPrinter(hPrinter, 8, nullptr, 0, &printerInfoBufferBytes) && GetLastError() == ERROR_INSUFFICIENT_BUFFER);

	BYTE * printerInfoBuffer = new BYTE[printerInfoBufferBytes];
	CHECK(GetPrinter(hPrinter, 8, printerInfoBuffer, printerInfoBufferBytes, &printerInfoBufferBytes));
	CHECK((size_t)printerInfoBufferBytes >= sizeof(PRINTER_INFO_8));

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

void DrawToPrintContext(
	_In_ ID2D1DeviceContext* d2dContext,
	UINT pageNumber,
	SIZE pageSize,
	SIZE bitmapSize,
	ID2D1Bitmap* d2dBitmap
)
{
	HRESULT hr = S_OK;

	// Get the size of the displayed window.
	D2D1_SIZE_U windowSize = { 0 };
	windowSize.width = pageSize.cx;
	windowSize.height = pageSize.cy;

	// Compute the margin sizes.
	FLOAT margin = PAGE_MARGIN_IN_DIPS;

	// Get the size of the destination context ("page").
	D2D1_SIZE_F targetSize = { 0 };
	targetSize.width = pageSize.cx - 2 * margin;
	targetSize.height = pageSize.cy - 2 * margin;

	// Compute the size of the gridded background rectangle.
	D2D1_SIZE_F frameSize = D2D1::SizeF(
		targetSize.width,
		FRAME_HEIGHT_IN_DIPS
	);

	// Compute the translation matrix that simulates scrolling or printing.
	D2D1_MATRIX_3X2_F scrollTransform = D2D1::Matrix3x2F::Translation(margin, margin);

	d2dContext->BeginDraw();

	d2dContext->SetTransform(scrollTransform);

	d2dContext->Clear(D2D1::ColorF(D2D1::ColorF::White));

	if (pageNumber == 1)
	{
		/*
		// Display geometries and text on screen. In printing case, display only on page 1.

		// Paint a grid background
		d2dContext->FillRectangle(
			D2D1::RectF(0.0f, 0.0f, frameSize.width, frameSize.height),
			m_gridPatternBrush
		);
		*/

		// Draw a bitmap in the upper-left corner of the window.
		D2D1_SIZE_F bitmapD2dSize;
		bitmapD2dSize.width = bitmapSize.cx;
		bitmapD2dSize.height = bitmapSize.cy;
		d2dContext->DrawBitmap(
			d2dBitmap,
			D2D1::RectF(0.0f, 0.0f, bitmapD2dSize.width, bitmapD2dSize.height)
		);
	}

	hr = d2dContext->EndDraw();
	CHECK(SUCCEEDED(hr));
}

void PrintBitmap(const wchar_t* printerName, const wchar_t* bmpFilePath)
{
	HRESULT hr;

	SIZE paperSize = MyGetPageSize(PRINTERNAME);

	SIZE bitmapSize = { 0 };
	{
		HBITMAP hBitmap = (HBITMAP)LoadImage(NULL, bmpFilePath, IMAGE_BITMAP, 0, 0, LR_LOADFROMFILE);
		CHECK(hBitmap);

		BITMAP bm;
		int objectBytes = GetObjectW(hBitmap, sizeof(BITMAP), (LPVOID)&bm);
		CHECK((size_t)objectBytes == sizeof(BITMAP));

		bitmapSize.cx = bm.bmWidth;
		bitmapSize.cy = bm.bmHeight;
		DeleteObject(hBitmap);
	}

	// Create WIC factory
	CComPtr<IWICImagingFactory> wicFactory;
	hr = CoCreateInstance(
		CLSID_WICImagingFactory,
		nullptr,
		CLSCTX_INPROC_SERVER,
		IID_PPV_ARGS(&wicFactory)
	);
	CHECK(SUCCEEDED(hr));
	CHECK(wicFactory);

	CComPtr<IWICBitmapDecoder> decoder;
	hr = wicFactory->CreateDecoderFromFilename(
		bmpFilePath,                     // Image to be decoded
		nullptr,                         // Do not prefer a particular vendor
		GENERIC_READ,                    // Desired read access to the file
		WICDecodeMetadataCacheOnDemand,  // Cache metadata when needed
		&decoder                         // Pointer to the decoder
	);
	CHECK(SUCCEEDED(hr));
	CHECK(decoder);

	CComPtr<IWICBitmapFrameDecode> frame;
	hr = decoder->GetFrame(0, &frame);
	CHECK(SUCCEEDED(hr));
	CHECK(frame);

	CComPtr<IWICFormatConverter> convertedSourceBitmap;
	hr = wicFactory->CreateFormatConverter(&convertedSourceBitmap);
	CHECK(SUCCEEDED(hr));
	CHECK(convertedSourceBitmap);

	hr = convertedSourceBitmap->Initialize(
		frame,                           // Input bitmap to convert
		GUID_WICPixelFormat32bppPBGRA,   // Destination pixel format
		WICBitmapDitherTypeNone,         // Specified dither pattern
		nullptr,                         // Specify a particular palette 
		0.f,                             // Alpha threshold
		WICBitmapPaletteTypeCustom       // Palette translation type
	);
	CHECK(SUCCEEDED(hr));

	CComPtr<ID2D1Device> d2dDevice;
	{
		D2D1_FACTORY_OPTIONS options = {};
#if defined(_DEBUG)
		options.debugLevel = D2D1_DEBUG_LEVEL_INFORMATION;
#endif
		CComPtr<ID2D1Factory1> d2dFactory;
		hr = D2D1CreateFactory(
			D2D1_FACTORY_TYPE_SINGLE_THREADED,
			options,
			&d2dFactory
		);
		CHECK(SUCCEEDED(hr));
		CHECK(d2dFactory);

		UINT createDeviceFlags = D3D11_CREATE_DEVICE_BGRA_SUPPORT;
#ifdef _DEBUG
		createDeviceFlags |= D3D11_CREATE_DEVICE_DEBUG;
#endif

		CComPtr <ID3D11Device> d3dDevice;
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
		CComPtr <IDXGIDevice> dxgiDevice;
		hr = d3dDevice->QueryInterface(&dxgiDevice);
		CHECK(SUCCEEDED(hr));
		CHECK(dxgiDevice);

		// Create a D2D device from the DXGI device
		hr = d2dFactory->CreateDevice(
			dxgiDevice,
			&d2dDevice
		);
		CHECK(SUCCEEDED(hr));
		CHECK(d2dDevice);
	}

	// Print stuff in here
	CComPtr<ID2D1PrintControl> printControl;
	{
		// Create a factory for document print job
		CComPtr<IPrintDocumentPackageTargetFactory> documentTargetFactory;
		hr = ::CoCreateInstance(
			__uuidof(PrintDocumentPackageTargetFactory),
			nullptr,
			CLSCTX_INPROC_SERVER,
			IID_PPV_ARGS(&documentTargetFactory)
		);
		CHECK(SUCCEEDED(hr));
		CHECK(documentTargetFactory);

		// Create the XPS document target
		CComPtr <IPrintDocumentPackageTarget> documentTarget;
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
		hr = d2dDevice->CreatePrintControl(
			wicFactory,
			documentTarget,
			nullptr,
			&printControl
		);
		CHECK(SUCCEEDED(hr));
		CHECK(printControl);
	}

	// Create a D2D Device Context dedicated for the print job
	CComPtr<ID2D1DeviceContext> d2dContextForPrint;
	hr = d2dDevice->CreateDeviceContext(
		D2D1_DEVICE_CONTEXT_OPTIONS_NONE,
		&d2dContextForPrint
	);
	CHECK(SUCCEEDED(hr));
	CHECK(d2dContextForPrint);

	// Create D2D commandList
	CComPtr <ID2D1CommandList> commandList;
	hr = d2dContextForPrint->CreateCommandList(&commandList);
	CHECK(SUCCEEDED(hr));
	CHECK(commandList);

	// Set the target commandList, which will get played back later
	d2dContextForPrint->SetTarget(commandList);

	// Create D2D bitmap
	CComPtr<ID2D1Bitmap> d2dBitmap;
	hr = d2dContextForPrint->CreateBitmapFromWicBitmap(convertedSourceBitmap, &d2dBitmap);
	CHECK(SUCCEEDED(hr));
	CHECK(d2dBitmap);

	// Calculate the margin
///	const float margin = (2 * PAGE_MARGIN_IN_DIPS);

	DrawToPrintContext(d2dContextForPrint, 1, paperSize, bitmapSize, d2dBitmap);

	/*

	// Draw the page into the commandList
	d2dContextForPrint->BeginDraw();
	d2dContextForPrint->DrawBitmap(d2dBitmap, D2D1::RectF(margin, margin, bitmapSize.cx, bitmapSize.cy));
	d2dContextForPrint->EndDraw();
	*/

	// Playback the commandList onto the page and finalize it
	hr = printControl->AddPage(commandList, D2D1::SizeF(paperSize.cx, paperSize.cy), nullptr);
	CHECK(SUCCEEDED(hr));

	hr = printControl->Close();
	CHECK(SUCCEEDED(hr));
}

#if 0

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

HRESULT MyCreateDeviceContext_YIKES(const wchar_t* printerName, SIZE paperSize, SIZE bitmapSizeInPx, HBITMAP hBitmap)
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

	/*
	// Create the bitmap decoder
	IWICBitmapDecoder* wicBitmapDecoder;
	hr = wicFactory->CreateDecoder(GUID_ContainerFormatBmp, &GUID_VendorMicrosoft, &wicBitmapDecoder);
	CHECK(SUCCEEDED(hr));
	CHECK(wicBitmapDecoder);

	// Retrieve the first frame of the image from the decoder
	IWICBitmapFrameDecode* pFrame;
	hr = wicBitmapDecoder->GetFrame(0, &pFrame);
	CHECK(SUCCEEDED(hr));
	CHECK(pFrame);

	// Convert the frame to 32bppPBGRA
	IWICFormatConverter* formatConverter;
	hr = wicFactory->CreateFormatConverter(&formatConverter);
	CHECK(SUCCEEDED(hr));
	CHECK(formatConverter);

	// Initialize the formatConverter
	hr = formatConverter->Initialize(
		pFrame,                          // Input bitmap to convert
		GUID_WICPixelFormat32bppPBGRA,   // Destination pixel format
		WICBitmapDitherTypeNone,         // Specified dither pattern
		nullptr,                         // Specify a particular palette 
		0.f,                             // Alpha threshold
		WICBitmapPaletteTypeCustom       // Palette translation type
	);
	CHECK(SUCCEEDED(hr));


///	hr = m_pRT->CreateBitmapFromWicBitmap(m_pConvertedSourceBitmap, nullptr, &m_pD2DBitmap);
*/


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


	/*
	// Create the surface to render to
	IDXGISurface* surface;
	DXGI_SURFACE_DESC surfaceDesc = { 0 };
	surfaceDesc.Format = DXGI_FORMAT_B8G8R8A8_UNORM;
	surfaceDesc.Width = paperSize.cx;
	surfaceDesc.Height = paperSize.cy;
	hr = dxgiDevice->CreateSurface(&surfaceDesc, 1, DXGI_USAGE_RENDER_TARGET_OUTPUT, nullptr, &surface);
	CHECK(SUCCEEDED(hr));
	CHECK(surface);

	// Create the render target: https://docs.microsoft.com/en-us/windows/desktop/Direct2D/direct2d-and-direct3d-interoperation-overview#writing-to-a-direct3d-surface-with-a-dxgi-surface-render-target
	ID2D1RenderTarget* renderTarget;
    auto renderTargetProperties = D2D1::RenderTargetProperties(
        D2D1_RENDER_TARGET_TYPE_SOFTWARE,
        D2D1::PixelFormat(DXGI_FORMAT_B8G8R8A8_UNORM),
        96.0f,
        96.0f,
        D2D1_RENDER_TARGET_USAGE_NONE,
        D2D1_FEATURE_LEVEL_DEFAULT);
	hr = d2dFactory->CreateDxgiSurfaceRenderTarget(surface, renderTargetProperties, &renderTarget);
	CHECK(SUCCEEDED(hr));
	CHECK(renderTarget);

	// Create IWICBitmap
	IWICBitmap* wicBitmap;
	hr = wicFactory->CreateBitmapFromHBITMAP(hBitmap, nullptr, WICBitmapUseAlpha, &wicBitmap);
	CHECK(SUCCEEDED(hr));
	CHECK(wicBitmap);

	// Create D2d bitmap
	ID2D1Bitmap* d2dBitmap;
	hr = renderTarget->CreateBitmapFromWicBitmap(wicBitmap, &d2dBitmap);
	CHECK(SUCCEEDED(hr));
	CHECK(d2dBitmap);
	*/

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

	/*
	formatConverter->Release();
	pFrame->Release();
	wicBitmapDecoder->Release();
	*/

	surface->Release();

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
#endif

int main()
{
	CHECK(SUCCEEDED(::CoInitialize(nullptr)));
	PrintBitmap(PRINTERNAME, FILEPATH);
}
