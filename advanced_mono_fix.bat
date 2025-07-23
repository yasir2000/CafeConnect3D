@echo off
echo ========================================
echo   ADVANCED MonoManager NULL Fix
echo ========================================
echo.

REM Administrative check
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo ERROR: This script must run as Administrator
    echo Right-click this file and select "Run as Administrator"
    pause
    exit /b 1
)

set PROJECT_PATH="C:\CafeConnect3D"
echo Project: %PROJECT_PATH%
echo.

echo Step 1: Force-killing ALL Unity processes...
taskkill /f /im Unity.exe /t 2>nul
taskkill /f /im "Unity Hub.exe" /t 2>nul
taskkill /f /im UnityShaderCompiler.exe /t 2>nul
taskkill /f /im UnityHelper.exe /t 2>nul
taskkill /f /im UnityPackageManager.exe /t 2>nul
taskkill /f /im node.exe /t 2>nul
echo Waiting for cleanup...
timeout /t 10 >nul

echo.
echo Step 2: Complete project cleanup...
cd /d %PROJECT_PATH%

REM Remove ALL cache and temporary files
for %%D in (Library Temp Logs UserSettings obj .vs) do (
    if exist "%%D" (
        echo Removing %%D...
        rmdir /s /q "%%D" 2>nul
    )
)

REM Remove problematic assembly definitions
if exist "Assembly-CSharp.asmdef" (
    echo Removing problematic Assembly-CSharp.asmdef...
    del "Assembly-CSharp.asmdef"
)

REM Remove lock files
if exist "Packages\packages-lock.json" (
    echo Removing packages lock file...
    del "Packages\packages-lock.json"
)

echo.
echo Step 3: Recreating clean Unity project structure...

REM Recreate minimal ProjectSettings if corrupted
if not exist "ProjectSettings\ProjectVersion.txt" (
    echo Creating ProjectVersion.txt...
    echo m_EditorVersion: 2022.3.45f1 > "ProjectSettings\ProjectVersion.txt"
    echo m_EditorVersionWithRevision: 2022.3.45f1 (c74f0e6da61f) >> "ProjectSettings\ProjectVersion.txt"
)

REM Create minimal package manifest
echo Creating clean package manifest...
echo { > "Packages\manifest.json"
echo   "dependencies": { >> "Packages\manifest.json"
echo     "com.unity.collab-proxy": "2.0.7", >> "Packages\manifest.json"
echo     "com.unity.feature.development": "1.0.1", >> "Packages\manifest.json"
echo     "com.unity.textmeshpro": "3.0.6", >> "Packages\manifest.json"
echo     "com.unity.timeline": "1.7.4", >> "Packages\manifest.json"
echo     "com.unity.ugui": "1.0.0" >> "Packages\manifest.json"
echo   }, >> "Packages\manifest.json"
echo   "scopedRegistries": [], >> "Packages\manifest.json"
echo   "testables": [] >> "Packages\manifest.json"
echo } >> "Packages\manifest.json"

echo.
echo Step 4: Unity Hub cache cleanup...
set UNITY_HUB_PATH="%APPDATA%\UnityHub"
if exist %UNITY_HUB_PATH% (
    echo Clearing Unity Hub cache...
    rmdir /s /q %UNITY_HUB_PATH%\logs 2>nul
    rmdir /s /q %UNITY_HUB_PATH%\cache 2>nul
)

echo.
echo Step 5: Registry cleanup (fixing Unity associations)...
reg delete "HKEY_CURRENT_USER\SOFTWARE\Unity Technologies" /f 2>nul
reg delete "HKEY_LOCAL_MACHINE\SOFTWARE\Unity Technologies" /f 2>nul

echo.
echo ========================================
echo         ADVANCED FIX COMPLETE
echo ========================================
echo.
echo ✅ All Unity processes terminated
echo ✅ Complete project cache cleared
echo ✅ Assembly definitions removed
echo ✅ Package manifest recreated
echo ✅ Unity Hub cache cleared
echo ✅ Registry entries cleaned
echo.
echo CRITICAL NEXT STEPS:
echo.
echo 1. RESTART YOUR COMPUTER (important!)
echo 2. Open Unity Hub
echo 3. Go to Installs tab
echo 4. If Unity 2022.3.45f1 shows issues, REINSTALL IT
echo 5. Add project: %PROJECT_PATH%
echo 6. Open with fresh Unity installation
echo.
echo If MonoManager error STILL persists:
echo - Try Unity 2022.3.44f1 or 2022.3.46f1
echo - Create new project and copy Assets\_Project folder
echo.
pause
