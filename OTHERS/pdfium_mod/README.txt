1. first clone the real pdfium repo (snap to 4/30/2019)
2. copy these files over

set path=%path%;c:\temp\depot_tools
cd pdfium
REM optional: run gclient sync
REM see https://pdfium.googlesource.com/pdfium.git
REM Optional: generate out\Debug directory: gn args out/Debug
REM set DEPOT_TOOLS_WIN_TOOLCHAIN=0
REM gclient runhooks
REM -----
REM To build: ninja -C out\Debug pdfium_test
REM To run the test: pdfium_test.exe --render-oneshot --bmp c:\temp\test.pdf
REM BP: pdfium_test!WriteBmp
