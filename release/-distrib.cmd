@echo off
cls
set VER=v0.0.4
set TEMP_PATH=D:\TEMP
set DIST_PATH="%TEMP_PATH%\mrl-dist-%VER%"

if not exist "%TEMP_PATH%" mkdir "%TEMP_PATH%"
if not exist "%DIST_PATH%" mkdir "%DIST_PATH%"

xcopy "..\dist" "%DIST_PATH%" /s /v /f /y
del "%DIST_PATH%\readme.txt"

xcopy ".\history.txt" "%DIST_PATH%" /s /v /f /y
xcopy ".\readme.txt" "%DIST_PATH%" /s /v /f /y
xcopy ".\start.cmd" "%DIST_PATH%" /s /v /f /y
xcopy ".\gpl.txt" "%DIST_PATH%" /s /v /f /y
xcopy ".\todo.txt" "%DIST_PATH%" /s /v /f /y

"c:\Program Files\7-zip\7z.exe" a -tzip -mx5 -scsWIN -r %TEMP_PATH%\mrl-dist-%VER%.zip %DIST_PATH% > %TEMP_PATH%\dist.log
