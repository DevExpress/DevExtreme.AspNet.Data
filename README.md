# DevExtreme ASP.NET Data

[![Build status](https://ci.appveyor.com/api/projects/status/6jyq7ocmvbuhvypo/branch/master?svg=true)](https://ci.appveyor.com/project/dxrobot/devextreme-aspnet-data/branch/master)
[![codecov](https://codecov.io/gh/DevExpress/DevExtreme.AspNet.Data/branch/master/graph/badge.svg)](https://codecov.io/gh/DevExpress/DevExtreme.AspNet.Data)
[![NuGet](https://img.shields.io/nuget/v/DevExtreme.AspNet.Data.svg?maxAge=43200)](https://www.nuget.org/packages/DevExtreme.AspNet.Data)
[![npm](https://img.shields.io/npm/v/devextreme-aspnet-data.svg?maxAge=43200)](https://www.npmjs.com/package/devextreme-aspnet-data)
[![npm nojquery](https://img.shields.io/npm/v/devextreme-aspnet-data-nojquery.svg?maxAge=43200&label=npm+nojquery)](https://www.npmjs.com/package/devextreme-aspnet-data-nojquery)

This extension enables [DevExtreme client-side widgets](https://js.devexpress.com) to consume data from the server in ASP.NET applications. A widget communicates with the server in the following manner: the widget sends data loading options (filtering, grouping, sorting, and other options) to the server, the server processes data according to these options and then sends processed data back to the widget. In this way, you delegate all intensive data operations from the client to the server. This significantly improves the performance of DevExtreme widgets.

The extension can be used with the widgets directly or with [DevExtreme-based ASP.NET Core](https://docs.devexpress.com/AspNetCore/400263) and [DevExtreme ASP.NET MVC 5](https://docs.devexpress.com/DevExtremeAspNetMvc/400943/) controls.

## Installation and Configuration

The `DevExtreme.AspNet.Data` extension consists of two parts: server-side and client-side. Learn how to install and configure both the parts from the following topics:

- [Server Side Configuration](docs/server-side-configuration.md)
- [Client Side with jQuery](docs/client-side-with-jquery.md)
- [Client Side without jQuery (Angular, etc.)](docs/client-side-without-jquery.md)

## CI Builds

CI builds are used to get urgent bug fixes and to test not-yet-released functionality. In other cases, prefer release builds.

Follow [these instructions](docs/using-ci-builds.md) to get CI builds.