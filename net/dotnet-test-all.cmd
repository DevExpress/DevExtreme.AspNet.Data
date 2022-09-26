@echo off

SqlLocalDB start

dotnet test --no-build "%~dp0DevExtreme.AspNet.Data.sln"
set dotnet_status=%errorlevel%

SqlLocalDB stop

exit /b %dotnet_status%
