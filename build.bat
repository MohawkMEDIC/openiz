@echo off
set version=%1

set msbuild="";
IF EXIST "C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe" (
set msbuild="C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe";
) ELSE IF EXIST "c:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\MSBuild.exe" (
set msbuild="c:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\MSBuild.exe"
) ELSE (
echo "No MSBUILD"
exit
)

echo Using %msbuild%
%msbuild% openiz.sln /t:Rebuild /p:Configuration=Release

echo Building Windows Installer
cd installer
call .\build.bat %version%
cd ..
exit /b