# Target Frameworks Map

## Summary

[.NET 8.0](#net-80-projects)

- tfm: `net8.0`: [Assemblies: 7, Package References: 20, References: 1](#net-80-dependencies)

[.NET 9.0](#net-90-projects)

- tfm: `net9.0`: [Assemblies: 1, Package References: 5 (override version 1)](#net-90-dependencies)

[.NET Framework 4.6.2](#net-framework-462-projects)

- tfm: `net462`: [Assemblies: 2, Package References: 4, References: 1](#net-framework-462-dependencies)

[.NET Framework 4.7.2](#net-framework-472-projects)

- tfm: `net472`: [Assemblies: 3, Package References: 7, References: 4](#net-framework-472-dependencies)

[net10.0](#net100-projects)

- tfm: ``: [Assemblies: 1, Package References: 5 (override version 1)](#net100-dependencies)

[Expected Directory.Packages.props file](expected-directorypackagesprops-file)

## Package References (by Target Framework)

### .NET 8.0 Dependencies

TFM: `net8.0`

- `Azure.Identity`
- `DevExpress.Xpo`
- `FluentNHibernate`
- `LinqKit.Microsoft.EntityFrameworkCore`
- `Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation`
- `Microsoft.EntityFrameworkCore.SqlServer`
- `Microsoft.Identity.Client`
- `Microsoft.NET.Test.Sdk`
- `Microsoft.Web.LibraryManager.Build`
- `Newtonsoft.Json`[Deprecated ->](#newtonsoftjson-tfmnet80)
- `System.Data.SqlClient`
- `System.Formats.Asn1`
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

- `LinqKit.Microsoft.EntityFrameworkCore`
- `Microsoft.EntityFrameworkCore.SqlServer`
- `Microsoft.NET.Test.Sdk`
- `xunit`
- `xunit.runner.visualstudio`
---
- `Microsoft.EntityFrameworkCore.SqlServer` VersionOverride=`9.0.0`[->](#microsoftentityframeworkcoresqlserver-versionoverride900)

### .NET Framework 4.6.2 Dependencies

TFM: `net462`

- `System.Data.SqlClient`
- `System.Text.Json`
- `xunit.assert`
- `xunit.core`

### .NET Framework 4.7.2 Dependencies

TFM: `net472`

- `EntityFramework`
- `LinqKit.EntityFramework`
- `Newtonsoft.Json`[Deprecated ->](#newtonsoftjson-tfmnet472)
- `System.Data.SqlClient`
- `System.Linq.Dynamic.Core`
- `xunit`
- `xunit.runner.visualstudio`

### net10.0 Dependencies

TFM: ``

- `LinqKit.Microsoft.EntityFrameworkCore`
- `Microsoft.EntityFrameworkCore.SqlServer`
- `Microsoft.NET.Test.Sdk`
- `xunit`
- `xunit.runner.visualstudio`
---
- `Microsoft.EntityFrameworkCore.SqlServer` VersionOverride=`10.0.0-preview.3.25171.6`[->](#microsoftentityframeworkcoresqlserver-versionoverride1000-preview3251716)

## Projects (by Target Framework)

### .NET 8.0 Projects

TFM: `net8.0`

- DevExtreme.AspNet.Data (`\DevExtreme.AspNet.Data\DevExtreme.AspNet.Data.csproj`)
- DevExtreme.AspNet.Data.Tests (`\DevExtreme.AspNet.Data.Tests\DevExtreme.AspNet.Data.Tests.csproj`)
- DevExtreme.AspNet.Data.Tests.Common (`\DevExtreme.AspNet.Data.Tests.Common\DevExtreme.AspNet.Data.Tests.Common.csproj`)
- DevExtreme.AspNet.Data.Tests.EFCore8 (`\DevExtreme.AspNet.Data.Tests.EFCore8\DevExtreme.AspNet.Data.Tests.EFCore8.csproj`)
- DevExtreme.AspNet.Data.Tests.NH (`\DevExtreme.AspNet.Data.Tests.NH\DevExtreme.AspNet.Data.Tests.NH.csproj`)
- DevExtreme.AspNet.Data.Tests.Xpo (`\DevExtreme.AspNet.Data.Tests.Xpo\DevExtreme.AspNet.Data.Tests.Xpo.csproj`)
- Sample (`\Sample\Sample.csproj`)

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

### net10.0 Projects

TFM: ``

- DevExtreme.AspNet.Data.Tests.EFCore10 (`\DevExtreme.AspNet.Data.Tests.EFCore10\DevExtreme.AspNet.Data.Tests.EFCore10.csproj`)

## Projects with exact Versions or VersionOverrides

### TFM: `net9.0`

#### `Microsoft.EntityFrameworkCore.SqlServer` VersionOverride=`9.0.0`
- `\DevExtreme.AspNet.Data.Tests.EFCore9\DevExtreme.AspNet.Data.Tests.EFCore9.csproj`

### TFM: ``

#### `Microsoft.EntityFrameworkCore.SqlServer` VersionOverride=`10.0.0-preview.3.25171.6`
- `\DevExtreme.AspNet.Data.Tests.EFCore10\DevExtreme.AspNet.Data.Tests.EFCore10.csproj`

## Projects with deprecated packages

### TFM: `net8.0`

#### Newtonsoft.Json tfm=`net8.0`
- `\Sample\Sample.csproj`

### TFM: `net472`

#### Newtonsoft.Json tfm=`net472`
- `\DevExtreme.AspNet.Data.Tests.NET4\DevExtreme.AspNet.Data.Tests.NET4.csproj`

## Expected Directory.Packages.props file

```xml
<Project>
  <PropertyGroup>
    <NoWarn>NU1507</NoWarn>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
  </PropertyGroup>
  <!-- Common (.NET/.NET Framework) PackageReferences -->
  <ItemGroup>
    <PackageVersion Include="Newtonsoft.Json" Version="13.0.3" />
    <PackageVersion Include="System.Data.SqlClient" Version="4.8.6" />
    <PackageVersion Include="System.Linq.Dynamic.Core" Version="1.6.0" />
    <PackageVersion Include="System.Text.Json" Version="8.0.5" />
    <PackageVersion Include="xunit" Version="2.9.2" />
    <PackageVersion Include="xunit.assert" Version="2.9.2" />
    <PackageVersion Include="xunit.core" Version="2.9.2" />
    <PackageVersion Include="xunit.runner.visualstudio" Version="2.8.2" />
  </ItemGroup>
  <!-- .NET PackageReferences -->
  <ItemGroup Condition="'$(TargetFramework)' == 'net8.0' Or '$(TargetFramework)' == 'net9.0'">
    <PackageVersion Include="Azure.Identity" Version="1.12.0" />
    <PackageVersion Include="DevExpress.Xpo" Version="24.2.3" />
    <PackageVersion Include="FluentNHibernate" Version="3.4.0" />
    <PackageVersion Include="LinqKit.Microsoft.EntityFrameworkCore" Version="6.1.3" />
    <PackageVersion Include="Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation" Version="8.0.11" />
    <PackageVersion Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.11" />
    <PackageVersion Include="Microsoft.Identity.Client" Version="4.61.3" />
    <PackageVersion Include="Microsoft.NET.Test.Sdk" Version="17.4.1" />
    <PackageVersion Include="Microsoft.Web.LibraryManager.Build" Version="2.1.175" />
    <PackageVersion Include="System.Formats.Asn1" Version="8.0.1" />
    <PackageVersion Include="System.Net.Http" Version="4.3.4" />
    <PackageVersion Include="System.Text.RegularExpressions" Version="4.3.1" />
  </ItemGroup>
  <!-- .NET Framework PackageReferences -->
  <ItemGroup Condition="'$(TargetFramework)' == 'net462' Or '$(TargetFramework)' == 'net472'">
    <PackageVersion Include="EntityFramework" Version="6.5.1" />
    <PackageVersion Include="LinqKit.EntityFramework" Version="1.2.3" />
  </ItemGroup>
  <!-- (Unknown) PackageReferences -->
  <ItemGroup Condition="'$(TargetFramework)' == ''">
    <PackageVersion Include="LinqKit.Microsoft.EntityFrameworkCore" Version="6.1.3" />
    <PackageVersion Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.11" />
    <PackageVersion Include="Microsoft.NET.Test.Sdk" Version="17.4.1" />
  </ItemGroup>
</Project>

```
