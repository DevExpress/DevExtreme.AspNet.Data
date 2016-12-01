@echo off
cd %~dp0

if not exist nuget.exe (PowerShell -Command wget https://dist.nuget.org/win-x86-commandline/latest/nuget.exe -OutFile nuget.exe)

set NUGET_INSTALL=nuget install -ExcludeVersion -OutputDirectory .

%NUGET_INSTALL% OpenCover
%NUGET_INSTALL% ReportGenerator
