//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

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

#include <wchar.h>
#include <math.h>
#include <Prntvpt.h>
#include <Strsafe.h>

#include "ImagePrinter.h"

static const FLOAT PAGE_WIDTH_IN_DIPS    = 8.5f * 96.0f;     // 8.5 inches
static const FLOAT PAGE_HEIGHT_IN_DIPS   = 11.0f * 96.0f;    // 11 inches
static const FLOAT PAGE_MARGIN_IN_DIPS   = 96.0f;            // 1 inch

// Initializes members.
ImagePrinter::ImagePrinter() :
    m_resourcesValid(false),
    m_d2dFactory(nullptr),
    m_wicFactory(nullptr),
    m_swapChain(nullptr),
    m_d2dDevice(nullptr),
    m_d2dContext(nullptr),
    m_customBitmap(nullptr),
    m_printControl(nullptr),
    m_documentTarget(nullptr),
    m_pageHeight(PAGE_HEIGHT_IN_DIPS),
    m_pageWidth(PAGE_WIDTH_IN_DIPS)
{
}

// Releases resources.
ImagePrinter::~ImagePrinter()
{
    // Release device-dependent resources.
    SafeRelease(&m_customBitmap);
    SafeRelease(&m_swapChain);
    SafeRelease(&m_d2dDevice);
    SafeRelease(&m_d2dContext);

    // Release printing-specific resources.
    SafeRelease(&m_printControl);
    SafeRelease(&m_documentTarget);

    // Release factories.
    SafeRelease(&m_d2dFactory);
    SafeRelease(&m_wicFactory);
}

// Creates the application window and initializes
// device-independent and device-dependent resources.
HRESULT ImagePrinter::Initialize()
{
    // Initialize device-indpendent resources, such
    // as the Direct2D factory.
    HRESULT hr = CreateDeviceIndependentResources();
    return hr;
}

// Calculates the size of the D2D (child) window.
D2D1_SIZE_U ImagePrinter::CalculateD2DWindowSize()
{
    D2D1_SIZE_U d2dWindowSize = {0};
    d2dWindowSize.width = 888;
    d2dWindowSize.height = 888;
    return d2dWindowSize;
}

// Creates resources which are not bound to any device.
// Their lifetimes effectively extend for the duration
// of the app.
HRESULT ImagePrinter::CreateDeviceIndependentResources()
{
    HRESULT hr = S_OK;

    ID2D1GeometrySink* sink = nullptr;

    if (SUCCEEDED(hr))
    {
        // Create a Direct2D factory.
        D2D1_FACTORY_OPTIONS options;
        ZeroMemory(&options, sizeof(D2D1_FACTORY_OPTIONS));

#if defined(_DEBUG)
        // If the project is in a debug build, enable Direct2D debugging via SDK Layers
        options.debugLevel = D2D1_DEBUG_LEVEL_INFORMATION;
#endif

        hr = D2D1CreateFactory(
            D2D1_FACTORY_TYPE_SINGLE_THREADED,
            options,
            &m_d2dFactory
            );
    }
    if (SUCCEEDED(hr))
    {
        // Create a WIC factory.
        hr = CoCreateInstance(
            CLSID_WICImagingFactory,
            nullptr,
            CLSCTX_INPROC_SERVER,
            IID_PPV_ARGS(&m_wicFactory)
            );
    }

    return hr;
}

// Create D2D context for display (Direct3D) device
HRESULT ImagePrinter::CreateDeviceContext()
{
    HRESULT hr = S_OK;

    // Get the size of the child window.
    D2D1_SIZE_U size = CalculateD2DWindowSize();

    // Create a D3D device and a swap chain sized to the child window.
    UINT createDeviceFlags = D3D11_CREATE_DEVICE_BGRA_SUPPORT;

#ifdef _DEBUG
    createDeviceFlags |= D3D11_CREATE_DEVICE_DEBUG;
#endif

    D3D_DRIVER_TYPE driverTypes[] =
    {
        D3D_DRIVER_TYPE_HARDWARE,
        D3D_DRIVER_TYPE_WARP,
    };
    UINT countOfDriverTypes = ARRAYSIZE(driverTypes);

    DXGI_SWAP_CHAIN_DESC swapDescription;
    ZeroMemory(&swapDescription, sizeof(swapDescription));
    swapDescription.BufferDesc.Width = size.width;
    swapDescription.BufferDesc.Height = size.height;
    swapDescription.BufferDesc.Format = DXGI_FORMAT_B8G8R8A8_UNORM;
    swapDescription.BufferDesc.RefreshRate.Numerator = 60;
    swapDescription.BufferDesc.RefreshRate.Denominator = 1;
    swapDescription.SampleDesc.Count = 1;
    swapDescription.SampleDesc.Quality = 0;
    swapDescription.BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT;
    swapDescription.BufferCount = 1;
    swapDescription.OutputWindow = GetDesktopWindow();
    swapDescription.Windowed = TRUE;

    ID3D11Device* d3dDevice = nullptr;
    for (UINT driverTypeIndex = 0; driverTypeIndex < countOfDriverTypes; driverTypeIndex++)
    {
        hr = D3D11CreateDeviceAndSwapChain(
            nullptr,       // use default adapter
            driverTypes[driverTypeIndex],
            nullptr,       // no external software rasterizer
            createDeviceFlags,
            nullptr,       // use default set of feature levels
            0,
            D3D11_SDK_VERSION,
            &swapDescription,
            &m_swapChain,
            &d3dDevice,
            nullptr,       // do not care about what feature level is chosen
            nullptr        // do not retain D3D device context
            );

        if (SUCCEEDED(hr))
        {
            break;
        }
    }

    IDXGIDevice* dxgiDevice = nullptr;
    if (SUCCEEDED(hr))
    {
        // Get a DXGI device interface from the D3D device.
        hr = d3dDevice->QueryInterface(&dxgiDevice);
    }
    if (SUCCEEDED(hr))
    {
        // Create a D2D device from the DXGI device.
        hr = m_d2dFactory->CreateDevice(
            dxgiDevice,
            &m_d2dDevice
            );
    }
    if (SUCCEEDED(hr))
    {
        // Create a device context from the D2D device.
        hr = m_d2dDevice->CreateDeviceContext(
            D2D1_DEVICE_CONTEXT_OPTIONS_NONE,
            &m_d2dContext
            );
    }

    SafeRelease(&dxgiDevice);
    SafeRelease(&d3dDevice);
    return hr;
}

// This method creates resources which are bound to a particular
// Direct3D device. It's all centralized here, in case the resources
// need to be recreated in case of Direct3D device loss (e.g. display
// change, remoting, removal of video card, etc). The resources created
// here can be used by multiple Direct2D device contexts (in this
// sample, one for display and another for print) which are created
// from the same Direct2D device.
HRESULT ImagePrinter::CreateDeviceResources(LPCWSTR imageFilePath)
{
    HRESULT hr = S_OK;

    if (!m_resourcesValid)
    {
        hr = CreateDeviceContext();

        IDXGISurface* surface = nullptr;
        if (SUCCEEDED(hr))
        {
            // Get a surface from the swap chain.
            hr = m_swapChain->GetBuffer(
                0,
                IID_PPV_ARGS(&surface)
                );
        }
        ID2D1Bitmap1* bitmap = nullptr;
        if (SUCCEEDED(hr))
        {
            FLOAT dpiX, dpiY;
#pragma warning(push)
#pragma warning(disable : 4996) // GetDesktopDpi is deprecated.
            m_d2dFactory->GetDesktopDpi(&dpiX, &dpiY);
#pragma warning(pop)

            // Create a bitmap pointing to the surface.
            D2D1_BITMAP_PROPERTIES1 properties = D2D1::BitmapProperties1(
                D2D1_BITMAP_OPTIONS_TARGET | D2D1_BITMAP_OPTIONS_CANNOT_DRAW,
                D2D1::PixelFormat(
                    DXGI_FORMAT_B8G8R8A8_UNORM,
                    D2D1_ALPHA_MODE_IGNORE
                    ),
                dpiX,
                dpiY
                );

            hr = m_d2dContext->CreateBitmapFromDxgiSurface(
                surface,
                &properties,
                &bitmap
                );
        }
        if (SUCCEEDED(hr))
        {
            // Set the bitmap as the target of our device context.
            m_d2dContext->SetTarget(bitmap);
        }
        if (SUCCEEDED(hr))
        {
            // Create a bitmap by loading it from a file.
            hr = LoadBitmapFromFile(
                m_d2dContext,
                m_wicFactory,
				imageFilePath,
                &m_customBitmap
                );
        }

        SafeRelease(&bitmap);
        SafeRelease(&surface);
    }

    if (FAILED(hr))
    {
        DiscardDeviceResources();
    }
    else
    {
        m_resourcesValid = true;
    }

    return hr;
}

// Discards device-specific resources which need to be recreated
// when a Direct3D device is lost.
void ImagePrinter::DiscardDeviceResources()
{
    SafeRelease(&m_swapChain);
    SafeRelease(&m_d2dDevice);
    SafeRelease(&m_d2dContext);

    SafeRelease(&m_customBitmap);

    m_resourcesValid = false;
}

// Draws the scene to a rendering device context or a printing device context.
// If the "printing" parameter is set, this function will add margins to
// the target and render the text across two pages. Otherwise, it fits the
// content to the target and renders the text in one block.
HRESULT ImagePrinter::DrawToContext(
    _In_ ID2D1DeviceContext* d2dContext,
    UINT pageNumber)
{
    HRESULT hr = S_OK;

    // Get the size of the displayed window.
    D2D1_SIZE_U windowSize = CalculateD2DWindowSize();

    // Compute the margin sizes.
    FLOAT margin = PAGE_MARGIN_IN_DIPS;

    // Get the size of the destination context ("page").
    D2D1_SIZE_F targetSize = {0};
    targetSize.width = m_pageWidth - 2 * margin;
    targetSize.height = m_pageHeight - 2 * margin;

    // Compute the translation matrix that simulates printing.
	D2D1_MATRIX_3X2_F scrollTransform = D2D1::Matrix3x2F::Translation(margin, margin);

    d2dContext->BeginDraw();

    d2dContext->SetTransform(scrollTransform);

    d2dContext->Clear(D2D1::ColorF(D2D1::ColorF::White));

    // Draw a bitmap in the upper-left corner of the window.
    D2D1_SIZE_F bitmapSize = m_customBitmap->GetSize();
    d2dContext->DrawBitmap(
        m_customBitmap,
        D2D1::RectF(0.0f, 0.0f, bitmapSize.width, bitmapSize.height)
        );

    hr = d2dContext->EndDraw();

    return hr;
}

// Called whenever the application begins a print job. Initializes
// the printing subsystem, draws the scene to a printing device
// context, and commits the job to the printing subsystem.
HRESULT ImagePrinter::DoPrint(LPCWSTR printerName, LPCWSTR imageFilePath)
{
    HRESULT hr = S_OK;

    if (!m_resourcesValid)
    {
        hr = CreateDeviceResources(imageFilePath);
    }

    if (SUCCEEDED(hr))
    {
        // Initialize printing-specific resources and prepare the
        // printing subsystem for a job.
        hr = InitializePrintJob(printerName);
    }

    ID2D1DeviceContext* d2dContextForPrint = nullptr;
    if (SUCCEEDED(hr))
    {
        // Create a D2D Device Context dedicated for the print job.
        hr = m_d2dDevice->CreateDeviceContext(
            D2D1_DEVICE_CONTEXT_OPTIONS_NONE,
            &d2dContextForPrint
            );
    }

    if (SUCCEEDED(hr))
    {
        ID2D1CommandList* commandList = nullptr;

		{
            hr = d2dContextForPrint->CreateCommandList(&commandList);

            // Create, draw, and add a Direct2D Command List for each page.
            if (SUCCEEDED(hr))
            {
                d2dContextForPrint->SetTarget(commandList);
				hr = DrawToContext(d2dContextForPrint, 1);
                commandList->Close();
            }

            if (SUCCEEDED(hr))
            {
                hr = m_printControl->AddPage(commandList, D2D1::SizeF(m_pageWidth, m_pageHeight), nullptr);
            }

            SafeRelease(&commandList);
        }

        // Release the print device context.
        SafeRelease(&d2dContextForPrint);

        // Send the job to the printing subsystem and discard
        // printing-specific resources.
        HRESULT hrFinal = FinalizePrintJob();

        if (SUCCEEDED(hr))
        {
            hr = hrFinal;
        }
    }

    if (hr == D2DERR_RECREATE_TARGET)
    {
        DiscardDeviceResources();
    }

    return hr;
}

// Brings up a Print Dialog to collect user print
// settings, then creates and initializes a print
// control object properly for a new print job.
HRESULT ImagePrinter::InitializePrintJob(LPCWSTR printerName)
{
    HRESULT hr = S_OK;

    // Create a factory for document print job.
    IPrintDocumentPackageTargetFactory* documentTargetFactory = nullptr;
    if (SUCCEEDED(hr))
    {
		hr = ::CoCreateInstance(
			__uuidof(PrintDocumentPackageTargetFactory),
			nullptr,
			CLSCTX_INPROC_SERVER,
			IID_PPV_ARGS(&documentTargetFactory)
		);

    }

    // Initialize the print subsystem and get a package target.
    if (SUCCEEDED(hr))
    {
        hr = documentTargetFactory->CreateDocumentPackageTargetForPrintJob(
			printerName,                                // printer name
            L"Direct2D desktop app printing sample",    // job name
            nullptr,                                    // job output stream; when nullptr, send to printer
            nullptr,                     // job print ticket
            &m_documentTarget                           // result IPrintDocumentPackageTarget object
            );
    }

    // Create a new print control linked to the package target.
    if (SUCCEEDED(hr))
    {
        hr = m_d2dDevice->CreatePrintControl(
            m_wicFactory,
            m_documentTarget,
            nullptr,
            &m_printControl
            );
    }

    SafeRelease(&documentTargetFactory);

    return hr;
}


// Commits the current print job to the printing subystem by
// closing the print control, and releases all printing-
// specific resources.
HRESULT ImagePrinter::FinalizePrintJob()
{
    // Send the print job to the print subsystem. (When this call
    // returns, we are safe to release printing resources.)
    HRESULT hr = m_printControl->Close();

    SafeRelease(&m_printControl);

    return hr;
}

// Creates a Direct2D bitmap from the specified file name.
HRESULT ImagePrinter::LoadBitmapFromFile(
    _In_ ID2D1DeviceContext* d2dContext,
    _In_ IWICImagingFactory* wicFactory,
    _In_ PCWSTR uri,
    _Outptr_ ID2D1Bitmap** bitmap
    )
{
    IWICBitmapDecoder* bitmapDecoder = nullptr;
    HRESULT hr = wicFactory->CreateDecoderFromFilename(
        uri,
        nullptr,
        GENERIC_READ,
        WICDecodeMetadataCacheOnLoad,
        &bitmapDecoder
        );

    IWICBitmapFrameDecode* frameDecode = nullptr;
    if (SUCCEEDED(hr))
    {
        // Create the initial frame.
        hr = bitmapDecoder->GetFrame(0, &frameDecode);
    }

    IWICFormatConverter* formatConverter = nullptr;
    if (SUCCEEDED(hr))
    {
        // Convert the image format to 32bppPBGRA
        // (DXGI_FORMAT_B8G8R8A8_UNORM + D2D1_ALPHA_MODE_PREMULTIPLIED).
        hr = wicFactory->CreateFormatConverter(&formatConverter);
    }

    if (SUCCEEDED(hr))
    {
        hr = formatConverter->Initialize(
            frameDecode,
            GUID_WICPixelFormat32bppPBGRA,
            WICBitmapDitherTypeNone,
            nullptr,
            0.0f,
            WICBitmapPaletteTypeMedianCut
            );
    }

    if (SUCCEEDED(hr))
    {
        // Create a Direct2D bitmap from the WIC bitmap.
        hr = d2dContext->CreateBitmapFromWicBitmap(
            formatConverter,
            nullptr,
            bitmap
            );
    }

    SafeRelease(&bitmapDecoder);
    SafeRelease(&frameDecode);
    SafeRelease(&formatConverter);

    return hr;
}
