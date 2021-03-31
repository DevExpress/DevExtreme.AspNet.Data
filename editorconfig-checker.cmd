@echo off

set tool_exe=ec-windows-amd64.exe
set tool_tmp=%temp%\editorconfig-checker

if not exist %tool_tmp%\bin\%tool_exe% (
    md %temp%\editorconfig-checker
    curl -L https://github.com/editorconfig-checker/editorconfig-checker/releases/download/2.3.5/%tool_exe%.tar.gz | tar xzf - -C %tool_tmp%
)

%tool_tmp%\bin\%tool_exe% -no-color
