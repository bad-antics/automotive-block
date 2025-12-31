@echo off
echo ============================================
echo  BlackFlag v2.0 - Native Build Script
echo ============================================
echo.

REM Check for .NET SDK
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo ERROR: .NET SDK not found!
    echo Please install .NET 8.0 SDK from:
    echo https://dotnet.microsoft.com/download/dotnet/8.0
    pause
    exit /b 1
)

echo Building BlackFlag...
dotnet build -c Release

if errorlevel 1 (
    echo.
    echo Build failed! Check errors above.
    pause
    exit /b 1
)

echo.
echo Build successful!
echo.
echo Run with: dotnet run
echo.
echo Or publish standalone:
echo dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
echo.
pause
