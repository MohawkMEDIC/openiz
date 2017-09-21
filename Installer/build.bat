set version = %1
"c:\Program Files (x86)\Inno Setup 5\ISCC.exe" /dBUNDLED "/o.\dist" ".\OpenIZInstall.iss" /dMyAppVersion=%version%
"c:\Program Files (x86)\Inno Setup 5\ISCC.exe" /dx64 /dBUNDLED "/o.\dist" ".\OpenIZInstall.iss" /dMyAppVersion=%version%
"c:\Program Files (x86)\Inno Setup 5\ISCC.exe" "/o.\dist" ".\OpenIZInstall.iss" /dMyAppVersion=%version%