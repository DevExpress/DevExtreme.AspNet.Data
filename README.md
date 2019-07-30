# DevExtreme ASP.NET Data

[![Build status](https://ci.appveyor.com/api/projects/status/6jyq7ocmvbuhvypo/branch/master?svg=true)](https://ci.appveyor.com/project/dxrobot/devextreme-aspnet-data/branch/master)
[![codecov](https://codecov.io/gh/DevExpress/DevExtreme.AspNet.Data/branch/master/graph/badge.svg)](https://codecov.io/gh/DevExpress/DevExtreme.AspNet.Data)
[![NuGet](https://img.shields.io/nuget/v/DevExtreme.AspNet.Data.svg?maxAge=43200)](https://www.nuget.org/packages/DevExtreme.AspNet.Data)
[![npm](https://img.shields.io/npm/v/devextreme-aspnet-data.svg?maxAge=43200)](https://www.npmjs.com/package/devextreme-aspnet-data)
[![npm nojquery](https://img.shields.io/npm/v/devextreme-aspnet-data-nojquery.svg?maxAge=43200&label=npm+nojquery)](https://www.npmjs.com/package/devextreme-aspnet-data-nojquery)

This library enables [DevExtreme client-side widgets](https://js.devexpress.com) to perform CRUD operations via ASP.NET controllers and handlers and allows you to delegate all data-intensive operations to the server:

* a widget sends data loading options (filtering, grouping, sorting, etc.) to the server;
* the server processes data according to these options;
* the processed data is sent back to the widget.

Can be used with:

* [DevExtreme client-side widgets](https://js.devexpress.com)
* [DevExtreme-based ASP.NET Core controls](https://docs.devexpress.com/AspNetCore/400263)
* [DevExtreme ASP.NET MVC 5 controls](https://docs.devexpress.com/DevExtremeAspNetMvc/400943/)

## Installation and Configuration

`DevExtreme.AspNet.Data` consists of server-side and client-side parts. The following topics explain how to install and configure these parts:

- [Server Side Configuration](docs/server-side-configuration.md)
- [Client Side with jQuery](docs/client-side-with-jquery.md)
- [Client Side without jQuery (Angular, etc.)](docs/client-side-without-jquery.md)

## CI Builds

We recommend that you use [release builds](https://github.com/DevExpress/DevExtreme.AspNet.Data/releases). However, you can also use [CI builds](docs/using-ci-builds.md) to get urgent bug fixes or to test unreleased functionality.
