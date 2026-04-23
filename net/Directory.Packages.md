# Target Frameworks Map

## Summary

[.NET 10.0](#net-100-projects)

- tfm: `net10.0`: [Assemblies: 2, Package References: 10 (override version 1)](#net-100-dependencies)

[.NET 11.0](#net-110-projects)

- tfm: `net11.0`: [Assemblies: 1, Package References: 8 (override version 1)](#net-110-dependencies)

[.NET 8.0](#net-80-projects)

- tfm: `net8.0`: [Assemblies: 6, Package References: 17, References: 1](#net-80-dependencies)

[.NET 9.0](#net-90-projects)

- tfm: `net9.0`: [Assemblies: 1, Package References: 8 (override version 1)](#net-90-dependencies)

[.NET Framework 4.6.2](#net-framework-462-projects)

- tfm: `net462`: [Assemblies: 2, Package References: 4, References: 1](#net-framework-462-dependencies)

[.NET Framework 4.7.2](#net-framework-472-projects)

- tfm: `net472`: [Assemblies: 3, Package References: 7, References: 4](#net-framework-472-dependencies)

## Package References (by Target Framework)

### .NET 10.0 Dependencies

TFM: `net10.0`

- `Azure.Identity`
- `LinqKit.Microsoft.EntityFrameworkCore`
- `Microsoft.EntityFrameworkCore.SqlServer`
- `Microsoft.Identity.Client`
- `Microsoft.IdentityModel.Protocols.OpenIdConnect`
- `Microsoft.NET.Test.Sdk`
- `Microsoft.Web.LibraryManager.Build`
- `Newtonsoft.Json`[Deprecated ->](#newtonsoftjson-tfmnet100)
- `xunit`
- `xunit.runner.visualstudio`
---
- `Microsoft.EntityFrameworkCore.SqlServer` VersionOverride=`10.0.3`[->](#microsoftentityframeworkcoresqlserver-versionoverride1003)

### .NET 11.0 Dependencies

TFM: `net11.0`

- `Azure.Identity`
- `LinqKit.Microsoft.EntityFrameworkCore`
- `Microsoft.EntityFrameworkCore.SqlServer`
- `Microsoft.Identity.Client`
- `Microsoft.IdentityModel.Protocols.OpenIdConnect`
- `Microsoft.NET.Test.Sdk`
- `xunit`
- `xunit.runner.visualstudio`
---
- `Microsoft.EntityFrameworkCore.SqlServer` VersionOverride=`11.0.0-preview.1.26104.118`[->](#microsoftentityframeworkcoresqlserver-versionoverride1100-preview126104118)

### .NET 8.0 Dependencies

TFM: `net8.0`

- `Azure.Identity`
- `DevExpress.Xpo`
- `FluentNHibernate`
- `LinqKit.Microsoft.EntityFrameworkCore`
- `Microsoft.EntityFrameworkCore.SqlServer`
- `Microsoft.Identity.Client`
- `Microsoft.IdentityModel.Protocols.OpenIdConnect`
- `Microsoft.NET.Test.Sdk`
- `System.Data.SqlClient`[Deprecated ->](#systemdatasqlclient-tfmnet80)
- `System.Linq.Dynamic.Core`
- `System.Net.Http`
- `System.Text.Json`
- `System.Text.RegularExpressions`
- `xunit`
- `xunit.assert`
- `xunit.core`
- `xunit.runner.visualstudio`

### .NET 9.0 Dependencies

TFM: `net9.0`

- `Azure.Identity`
- `LinqKit.Microsoft.EntityFrameworkCore`
- `Microsoft.EntityFrameworkCore.SqlServer`
- `Microsoft.Identity.Client`
- `Microsoft.IdentityModel.Protocols.OpenIdConnect`
- `Microsoft.NET.Test.Sdk`
- `xunit`
- `xunit.runner.visualstudio`
---
- `Microsoft.EntityFrameworkCore.SqlServer` VersionOverride=`9.0.13`[->](#microsoftentityframeworkcoresqlserver-versionoverride9013)

### .NET Framework 4.6.2 Dependencies

TFM: `net462`

- `System.Data.SqlClient`[Deprecated ->](#systemdatasqlclient-tfmnet462)
- `System.Text.Json`
- `xunit.assert`
- `xunit.core`

### .NET Framework 4.7.2 Dependencies

TFM: `net472`

- `EntityFramework`
- `LinqKit.EntityFramework`
- `Newtonsoft.Json`[Deprecated ->](#newtonsoftjson-tfmnet472)
- `System.Data.SqlClient`[Deprecated ->](#systemdatasqlclient-tfmnet472)
- `System.Linq.Dynamic.Core`
- `xunit`
- `xunit.runner.visualstudio`

## Projects (by Target Framework)

### .NET 10.0 Projects

TFM: `net10.0`

- DevExtreme.AspNet.Data.Tests.EFCore10 (`\DevExtreme.AspNet.Data.Tests.EFCore10\DevExtreme.AspNet.Data.Tests.EFCore10.csproj`)
- Sample (`\Sample\Sample.csproj`)

### .NET 11.0 Projects

TFM: `net11.0`

- DevExtreme.AspNet.Data.Tests.EFCore11 (`\DevExtreme.AspNet.Data.Tests.EFCore11\DevExtreme.AspNet.Data.Tests.EFCore11.csproj`)

### .NET 8.0 Projects

TFM: `net8.0`

- DevExtreme.AspNet.Data (`\DevExtreme.AspNet.Data\DevExtreme.AspNet.Data.csproj`)
- DevExtreme.AspNet.Data.Tests (`\DevExtreme.AspNet.Data.Tests\DevExtreme.AspNet.Data.Tests.csproj`)
- DevExtreme.AspNet.Data.Tests.Common (`\DevExtreme.AspNet.Data.Tests.Common\DevExtreme.AspNet.Data.Tests.Common.csproj`)
- DevExtreme.AspNet.Data.Tests.EFCore8 (`\DevExtreme.AspNet.Data.Tests.EFCore8\DevExtreme.AspNet.Data.Tests.EFCore8.csproj`)
- DevExtreme.AspNet.Data.Tests.NH (`\DevExtreme.AspNet.Data.Tests.NH\DevExtreme.AspNet.Data.Tests.NH.csproj`)
- DevExtreme.AspNet.Data.Tests.Xpo (`\DevExtreme.AspNet.Data.Tests.Xpo\DevExtreme.AspNet.Data.Tests.Xpo.csproj`)

### .NET 9.0 Projects

TFM: `net9.0`

- DevExtreme.AspNet.Data.Tests.EFCore9 (`\DevExtreme.AspNet.Data.Tests.EFCore9\DevExtreme.AspNet.Data.Tests.EFCore9.csproj`)

### .NET Framework 4.6.2 Projects

TFM: `net462`

- DevExtreme.AspNet.Data (`\DevExtreme.AspNet.Data\DevExtreme.AspNet.Data.csproj`)
- DevExtreme.AspNet.Data.Tests.Common (`\DevExtreme.AspNet.Data.Tests.Common\DevExtreme.AspNet.Data.Tests.Common.csproj`)

### .NET Framework 4.7.2 Projects

TFM: `net472`

- DevExtreme.AspNet.Data.Tests (`\DevExtreme.AspNet.Data.Tests.NET4\DevExtreme.AspNet.Data.Tests.NET4.csproj`)
- DevExtreme.AspNet.Data.Tests.EF6 (`\DevExtreme.AspNet.Data.Tests.EF6\DevExtreme.AspNet.Data.Tests.EF6.csproj`)
- DevExtreme.AspNet.Data.Tests.L2S (`\DevExtreme.AspNet.Data.Tests.L2S\DevExtreme.AspNet.Data.Tests.L2S.csproj`)

## Projects with dependencies provided by FrameworkReference ⚠️

### TFM: `net10.0`

#### Microsoft.AspNetCore.App
- `\Sample\Sample.csproj`

#### Microsoft.NETCore.App
- `\DevExtreme.AspNet.Data.Tests.EFCore10\DevExtreme.AspNet.Data.Tests.EFCore10.csproj`
- `\Sample\Sample.csproj`

### TFM: `net11.0`

#### Microsoft.NETCore.App
- `\DevExtreme.AspNet.Data.Tests.EFCore11\DevExtreme.AspNet.Data.Tests.EFCore11.csproj`

### TFM: `net8.0`

#### Microsoft.NETCore.App
- `\DevExtreme.AspNet.Data.Tests.Common\DevExtreme.AspNet.Data.Tests.Common.csproj`
- `\DevExtreme.AspNet.Data.Tests.EFCore8\DevExtreme.AspNet.Data.Tests.EFCore8.csproj`
- `\DevExtreme.AspNet.Data.Tests.NH\DevExtreme.AspNet.Data.Tests.NH.csproj`
- `\DevExtreme.AspNet.Data.Tests.Xpo\DevExtreme.AspNet.Data.Tests.Xpo.csproj`
- `\DevExtreme.AspNet.Data.Tests\DevExtreme.AspNet.Data.Tests.csproj`
- `\DevExtreme.AspNet.Data\DevExtreme.AspNet.Data.csproj`

### TFM: `net9.0`

#### Microsoft.NETCore.App
- `\DevExtreme.AspNet.Data.Tests.EFCore9\DevExtreme.AspNet.Data.Tests.EFCore9.csproj`

### TFM: `net462`

#### Microsoft.NETCore.App
- `\DevExtreme.AspNet.Data.Tests.Common\DevExtreme.AspNet.Data.Tests.Common.csproj`
- `\DevExtreme.AspNet.Data\DevExtreme.AspNet.Data.csproj`

### TFM: `net472`

#### Microsoft.NETCore.App
- `\DevExtreme.AspNet.Data.Tests.EF6\DevExtreme.AspNet.Data.Tests.EF6.csproj`
- `\DevExtreme.AspNet.Data.Tests.L2S\DevExtreme.AspNet.Data.Tests.L2S.csproj`
- `\DevExtreme.AspNet.Data.Tests.NET4\DevExtreme.AspNet.Data.Tests.NET4.csproj`

## Projects with exact Versions or VersionOverrides

### TFM: `net10.0`

#### `Microsoft.EntityFrameworkCore.SqlServer` VersionOverride=`10.0.3`
- `\DevExtreme.AspNet.Data.Tests.EFCore10\DevExtreme.AspNet.Data.Tests.EFCore10.csproj`
- `\Sample\Sample.csproj`

### TFM: `net11.0`

#### `Microsoft.EntityFrameworkCore.SqlServer` VersionOverride=`11.0.0-preview.1.26104.118`
- `\DevExtreme.AspNet.Data.Tests.EFCore11\DevExtreme.AspNet.Data.Tests.EFCore11.csproj`

### TFM: `net9.0`

#### `Microsoft.EntityFrameworkCore.SqlServer` VersionOverride=`9.0.13`
- `\DevExtreme.AspNet.Data.Tests.EFCore9\DevExtreme.AspNet.Data.Tests.EFCore9.csproj`

## Projects with deprecated packages ❌

### TFM: `net10.0`

#### Newtonsoft.Json tfm=`net10.0`
- `\Sample\Sample.csproj`

### TFM: `net8.0`

#### System.Data.SqlClient tfm=`net8.0`
- `\DevExtreme.AspNet.Data.Tests.Common\DevExtreme.AspNet.Data.Tests.Common.csproj`

### TFM: `net462`

#### System.Data.SqlClient tfm=`net462`
- `\DevExtreme.AspNet.Data.Tests.Common\DevExtreme.AspNet.Data.Tests.Common.csproj`

### TFM: `net472`

#### Newtonsoft.Json tfm=`net472`
- `\DevExtreme.AspNet.Data.Tests.NET4\DevExtreme.AspNet.Data.Tests.NET4.csproj`

#### System.Data.SqlClient tfm=`net472`
- `\DevExtreme.AspNet.Data.Tests.EF6\DevExtreme.AspNet.Data.Tests.EF6.csproj`
- `\DevExtreme.AspNet.Data.Tests.L2S\DevExtreme.AspNet.Data.Tests.L2S.csproj`