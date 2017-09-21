@echo off
set version=%1

"C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe" openiz.sln /t:Rebuild /p:Configuration=Release
echo Building Windows Installer
cd installer
.\build.bat %version%