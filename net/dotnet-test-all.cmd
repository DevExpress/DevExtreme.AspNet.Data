dotnet test --no-build "%~dp0DevExtreme.AspNet.Data.sln" || exit /b 1
SqlLocalDB stop
