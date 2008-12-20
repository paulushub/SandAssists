@echo off
setlocal

if exist Ax*.dll del Ax*.dll
if exist MSHelp*.dll del MSHelp*.dll

AxImp.exe /keyfile:..\Sandcastle.Register.snk "%CommonProgramFiles%\Microsoft Shared\help\hxvz.dll"

del Ax*.dll
del MSHelpControls.dll

pause