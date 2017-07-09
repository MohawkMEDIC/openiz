@echo off
setlocal enabledelayedexpansion
for %%d in (*.dll) do (
		echo %%d
               copy ..\..\medicsvccore\bin\debug\%%d .\
)
for %%d in (*.pdb) do (
		echo %%d
               copy ..\..\openiz\bin\debug\%%d .\
)
endlocal
