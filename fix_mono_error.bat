@echo off
echo ========================================
echo   MonoManager NULL Error - Auto Fix
echo ========================================
echo.

REM Check if running as administrator
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo This script needs to run as Administrator
    echo Right-click and select "Run as Administrator"
    pause
    exit /b 1
)

set PROJECT_PATH="C:\CafeConnect3D"
set BACKUP_PATH="C:\CafeConnect3D_Backup"

echo Project Path: %PROJECT_PATH%
echo.

REM Check if project exists
if not exist %PROJECT_PATH% (
    echo ERROR: Project not found at %PROJECT_PATH%
    echo Please update PROJECT_PATH in this script
    pause
    exit /b 1
)

echo Step 1: Closing Unity processes...
taskkill /f /im Unity.exe 2>nul
taskkill /f /im "Unity Hub.exe" 2>nul
taskkill /f /im UnityShaderCompiler.exe 2>nul
taskkill /f /im UnityHelper.exe 2>nul
echo Waiting for processes to close...
timeout /t 5 >nul

echo.
echo Step 2: Creating backup of game scripts...
if not exist %BACKUP_PATH% mkdir %BACKUP_PATH%
if exist %PROJECT_PATH%\Assets\_Project (
    echo Backing up Assets\_Project...
    xcopy %PROJECT_PATH%\Assets\_Project %BACKUP_PATH%\Assets\_Project /e /i /h /y >nul
    echo ✓ Backup created at %BACKUP_PATH%
)

echo.
echo Step 3: Cleaning corrupted cache folders...
cd /d %PROJECT_PATH%

if exist "Library" (
    echo Removing Library folder...
    rmdir /s /q "Library"
    echo ✓ Library folder removed
)

if exist "Temp" (
    echo Removing Temp folder...
    rmdir /s /q "Temp"
    echo ✓ Temp folder removed
)

if exist "Logs" (
    echo Removing Logs folder...
    rmdir /s /q "Logs"
    echo ✓ Logs folder removed
)

if exist "UserSettings" (
    echo Removing UserSettings folder...
    rmdir /s /q "UserSettings"
    echo ✓ UserSettings folder removed
)

if exist "obj" (
    echo Removing obj folder...
    rmdir /s /q "obj"
    echo ✓ obj folder removed
)

if exist ".vs" (
    echo Removing .vs folder...
    rmdir /s /q ".vs"
    echo ✓ .vs folder removed
)

echo.
echo Step 4: Creating clean cache directories...
mkdir "Library" 2>nul
mkdir "Temp" 2>nul
mkdir "Logs" 2>nul
mkdir "UserSettings" 2>nul
echo ✓ Clean directories created

echo.
echo Step 5: Checking essential Unity files...
if exist "ProjectSettings\ProjectVersion.txt" (
    echo ✓ ProjectVersion.txt found
) else (
    echo ⚠ ProjectVersion.txt missing - recreating...
    echo m_EditorVersion: 2022.3.45f1 > "ProjectSettings\ProjectVersion.txt"
    echo m_EditorVersionWithRevision: 2022.3.45f1 (c74f0e6da61f) >> "ProjectSettings\ProjectVersion.txt"
)

if exist "Assets" (
    echo ✓ Assets folder found
) else (
    echo ⚠ Assets folder missing - recreating...
    mkdir "Assets"
)

if exist "Packages\manifest.json" (
    echo ✓ Package manifest found
) else (
    echo ⚠ Package manifest missing - recreating...
    if not exist "Packages" mkdir "Packages"
    echo { > "Packages\manifest.json"
    echo   "dependencies": { >> "Packages\manifest.json"
    echo     "com.unity.collab-proxy": "2.0.7", >> "Packages\manifest.json"
    echo     "com.unity.feature.development": "1.0.1", >> "Packages\manifest.json"
    echo     "com.unity.textmeshpro": "3.0.6", >> "Packages\manifest.json"
    echo     "com.unity.timeline": "1.7.4", >> "Packages\manifest.json"
    echo     "com.unity.ugui": "1.0.0" >> "Packages\manifest.json"
    echo   } >> "Packages\manifest.json"
    echo } >> "Packages\manifest.json"
)

echo.
echo ========================================
echo           FIX COMPLETE!
echo ========================================
echo.
echo ✅ Unity cache cleared
echo ✅ Corrupted files removed
echo ✅ Essential files verified
echo ✅ Game scripts backed up to: %BACKUP_PATH%
echo.
echo NEXT STEPS:
echo 1. Open Unity Hub
echo 2. Add project: %PROJECT_PATH%
echo 3. Open with Unity 2022.3.45f1
echo 4. Wait for reimport to complete
echo 5. Install Mirror Networking if needed
echo.
echo If the error persists, check MONO_MANAGER_FIX.md for advanced solutions.
echo.
pause
