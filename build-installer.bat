@echo off
setlocal
cd /d "%~dp0"

:: Builds "installer\Output\MPServerProvider Installer <version>.exe".
:: Usage:  build-installer.bat           (uses <Version> from MPServerProvider.App.csproj)
::         build-installer.bat 1.2.0     (builds that version instead)

set "PROJ=dotnet\src\MPServerProvider.App\MPServerProvider.App.csproj"
set "PUBLISH=installer\publish"

set "ISCC=%ProgramFiles(x86)%\Inno Setup 6\ISCC.exe"
if not exist "%ISCC%" set "ISCC=%ProgramFiles%\Inno Setup 6\ISCC.exe"
if not exist "%ISCC%" (
    echo [ERROR] Inno Setup 6 was not found. Install it from https://jrsoftware.org/isdl.php
    goto :fail
)

set "VERSION=%~1"
if "%VERSION%"=="" (
    for /f "usebackq delims=" %%v in (`dotnet msbuild "%PROJ%" -nologo -getProperty:Version`) do set "VERSION=%%v"
)
if "%VERSION%"=="" (
    echo [ERROR] Could not read the app version from %PROJ%.
    goto :fail
)

echo.
echo === Publishing MP Server Provider %VERSION% ===
if exist "%PUBLISH%" rmdir /s /q "%PUBLISH%"
dotnet publish "%PROJ%" -c Release -r win-x64 --self-contained true -p:Version=%VERSION% -o "%PUBLISH%" || goto :fail

echo.
echo === Building installer ===
"%ISCC%" /Q "/DAppVersion=%VERSION%" "installer\MPServerProvider.iss" || goto :fail

echo.
echo Done: %~dp0installer\Output\MPServerProvider Installer %VERSION%.exe
pause
exit /b 0

:fail
echo.
echo [ERROR] Building the installer failed.
pause
exit /b 1
