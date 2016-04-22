where dnx > dnxpath.txt
set /p dnxpath=<dnxpath.txt

call dnu build ..\DevExtreme.AspNet.Data ..\DevExtreme.AspNet.Data.Tests

OpenCover\tools\OpenCover.Console ^
 "-target: %dnxpath%" ^
 "-targetargs: --lib %~dp0\..\DevExtreme.AspNet.Data\bin\Debug\net40 -p ..\DevExtreme.AspNet.Data.Tests test" ^
 "-filter:+[DevExtreme*]*" ^
 -hideskipped:All -output:coverage.xml -register:user

rd /s /q report
ReportGenerator\tools\ReportGenerator -reports:coverage.xml -targetdir:report
start report\index.htm
