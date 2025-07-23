@echo off
echo ======================================
echo    CafeConnect3D Build Script
echo ======================================
echo.

REM Set Unity installation path (adjust if different)
set UNITY_PATH="C:\Program Files\Unity\Hub\Editor\2022.3.45f1\Editor\Unity.exe"

REM Set project path
set PROJECT_PATH="C:\CafeConnect3D"

REM Check if Unity exists
if not exist %UNITY_PATH% (
    echo ERROR: Unity not found at %UNITY_PATH%
    echo Please update UNITY_PATH in this script
    pause
    exit /b 1
)

REM Check if project exists
if not exist %PROJECT_PATH% (
    echo ERROR: Project not found at %PROJECT_PATH%
    echo Please update PROJECT_PATH in this script
    pause
    exit /b 1
)

echo Unity found: %UNITY_PATH%
echo Project path: %PROJECT_PATH%
echo.

:MENU
echo Choose build option:
echo 1. Build Client Only
echo 2. Build Server Only
echo 3. Build Both (Client + Server)
echo 4. Exit
echo.
set /p choice="Enter your choice (1-4): "

if "%choice%"=="1" goto BUILD_CLIENT
if "%choice%"=="2" goto BUILD_SERVER
if "%choice%"=="3" goto BUILD_ALL
if "%choice%"=="4" goto EXIT
echo Invalid choice. Please try again.
goto MENU

:BUILD_CLIENT
echo.
echo Building Client...
%UNITY_PATH% -projectPath %PROJECT_PATH% -executeMethod CafeConnect3D.Editor.BuildScript.BuildClient -quit -batchmode -logFile build_client.log
echo Client build complete! Check build_client.log for details.
echo Output: %PROJECT_PATH%\Build\Client\CafeConnect3D.exe
goto MENU

:BUILD_SERVER
echo.
echo Building Server...
%UNITY_PATH% -projectPath %PROJECT_PATH% -executeMethod CafeConnect3D.Editor.BuildScript.BuildServer -quit -batchmode -logFile build_server.log
echo Server build complete! Check build_server.log for details.
echo Output: %PROJECT_PATH%\Build\Server\CafeConnect3D_Server.exe
goto MENU

:BUILD_ALL
echo.
echo Building Client and Server...
%UNITY_PATH% -projectPath %PROJECT_PATH% -executeMethod CafeConnect3D.Editor.BuildScript.BuildAll -quit -batchmode -logFile build_all.log
echo All builds complete! Check build_all.log for details.
echo Client: %PROJECT_PATH%\Build\Client\CafeConnect3D.exe
echo Server: %PROJECT_PATH%\Build\Server\CafeConnect3D_Server.exe
goto MENU

:EXIT
echo.
echo Exiting build script...
pause
exit /b 0
