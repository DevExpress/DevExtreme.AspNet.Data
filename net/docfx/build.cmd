@echo off
cd %~dp0

set PATH=%PATH%;build
if not exist build md build

where nuget || (
    if not exist build/nuget.exe (PowerShell -Command wget https://dist.nuget.org/win-x86-commandline/latest/nuget.exe -O build/nuget.exe)
)

nuget install -ExcludeVersion -OutputDirectory build -Verbosity q docfx.console -Version 2.27.0
nuget install -ExcludeVersion -OutputDirectory build -Verbosity q msdn.4.5.2 -Pre

build\docfx.console\tools\docfx
