@echo off
setlocal

:: Run the .NET application
dotnet run --project "_MPServerProvider\dotnet\src\MPServerProvider.App"

:: Check if the command failed (any exit code other than 0)
if %errorlevel% neq 0 (
    echo.
    echo [ERROR] Failed to run the application ^(Exit Code: %errorlevel%^).
    pause
    exit /b %errorlevel%
)

endlocal