@echo off
setlocal enabledelayedexpansion
for %%d in (*.dll) do (
		echo %%d
               copy ..\..\servicecore\bin\release\%%d .\
)
for %%d in (*.pdb) do (
		echo %%d
               copy ..\..\openiz\bin\release\%%d .\
)
endlocal
