if not exist nuget.exe (PowerShell -Command wget https://dist.nuget.org/win-x86-commandline/latest/nuget.exe -OutFile nuget.exe)

nuget install OpenCover
nuget install ReportGenerator

move OpenCover.* OpenCover
move ReportGenerator.* ReportGenerator
