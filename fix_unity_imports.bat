@echo off
echo ========================================
echo   Fixing Missing UnityEngine References
echo ========================================
echo.

set PROJECT_PATH="C:\CafeConnect3D"
cd /d %PROJECT_PATH%

echo Scanning for scripts missing UnityEngine imports...
echo.

REM Check if we're in the right directory
if not exist "Assets\_Project\Scripts" (
    echo ERROR: Cannot find Assets\_Project\Scripts directory
    echo Make sure you're running this from the Unity project root
    pause
    exit /b 1
)

echo Fixing scripts with Header attributes but missing UnityEngine import...

REM Create a temporary PowerShell script to fix the files
echo $files = Get-ChildItem -Path "Assets\_Project\Scripts" -Recurse -Filter "*.cs" > fix_unity_imports.ps1
echo foreach ($file in $files) { >> fix_unity_imports.ps1
echo     $content = Get-Content $file.FullName -Raw >> fix_unity_imports.ps1
echo     if ($content -match '\[Header' -and $content -notmatch 'using UnityEngine') { >> fix_unity_imports.ps1
echo         Write-Host "Fixing: $($file.Name)" >> fix_unity_imports.ps1
echo         $lines = Get-Content $file.FullName >> fix_unity_imports.ps1
echo         $newLines = @() >> fix_unity_imports.ps1
echo         $addedUnityEngine = $false >> fix_unity_imports.ps1
echo         foreach ($line in $lines) { >> fix_unity_imports.ps1
echo             if ($line -match '^using ' -and -not $addedUnityEngine) { >> fix_unity_imports.ps1
echo                 $newLines += $line >> fix_unity_imports.ps1
echo                 if ($line -notmatch 'using UnityEngine') { >> fix_unity_imports.ps1
echo                     $newLines += 'using UnityEngine;' >> fix_unity_imports.ps1
echo                     $addedUnityEngine = $true >> fix_unity_imports.ps1
echo                 } >> fix_unity_imports.ps1
echo             } else { >> fix_unity_imports.ps1
echo                 $newLines += $line >> fix_unity_imports.ps1
echo             } >> fix_unity_imports.ps1
echo         } >> fix_unity_imports.ps1
echo         $newLines ^| Set-Content $file.FullName >> fix_unity_imports.ps1
echo     } >> fix_unity_imports.ps1
echo } >> fix_unity_imports.ps1

echo Running PowerShell fix script...
powershell -ExecutionPolicy Bypass -File fix_unity_imports.ps1

echo Cleaning up...
del fix_unity_imports.ps1

echo.
echo ========================================
echo         UNITY IMPORTS FIXED
echo ========================================
echo.
echo ✅ Added missing "using UnityEngine;" directives
echo ✅ Header attributes should now work properly
echo ✅ Compilation errors should be resolved
echo.
echo Next steps:
echo 1. Open Unity project
echo 2. Wait for script compilation
echo 3. Check Console for any remaining errors
echo.
pause
