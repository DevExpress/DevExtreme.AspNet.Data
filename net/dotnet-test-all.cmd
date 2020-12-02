@echo off
cd "%~dp0"
dotnet test --no-build DevExtreme.AspNet.Data.Tests         || exit /b 1
dotnet test --no-build DevExtreme.AspNet.Data.Tests.NET4    || exit /b 1
dotnet test --no-build DevExtreme.AspNet.Data.Tests.EFCore1 || exit /b 1
dotnet test --no-build DevExtreme.AspNet.Data.Tests.EFCore2 || exit /b 1
dotnet test --no-build DevExtreme.AspNet.Data.Tests.EFCore3 || exit /b 1
dotnet test --no-build DevExtreme.AspNet.Data.Tests.EFCore5 || exit /b 1
dotnet test --no-build DevExtreme.AspNet.Data.Tests.EF6     || exit /b 1
dotnet test --no-build DevExtreme.AspNet.Data.Tests.Xpo     || exit /b 1
dotnet test --no-build DevExtreme.AspNet.Data.Tests.NH      || exit /b 1
dotnet test --no-build DevExtreme.AspNet.Data.Tests.L2S     || exit /b 1
SqlLocalDB stop
