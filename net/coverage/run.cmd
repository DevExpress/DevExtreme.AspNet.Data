@echo off
cd %~dp0

call dotnet build ..\DevExtreme.AspNet.Data ..\DevExtreme.AspNet.Data.Tests

OpenCover\tools\OpenCover.Console ^
 "-target: %programfiles%\dotnet\dotnet.exe" ^
 "-targetargs: test -f netcoreapp1.0 %~dp0\..\DevExtreme.AspNet.Data.Tests" ^
 "-filter:+[DevExtreme*]* -[DevExtreme.AspNet.Data.Tests]*" ^
 -hideskipped:All -output:coverage.xml -register:user -oldstyle

rd /s /q report
ReportGenerator\tools\ReportGenerator -reports:coverage.xml -targetdir:report
start report\index.htm
