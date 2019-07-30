# Server Side Configuration

## Installation

The server-side part of the extension is the `DevExtreme.AspNet.Data.dll` assembly. Install it [from NuGet](http://www.nuget.org/packages/DevExtreme.AspNet.Data/) with the following command.

    Install-Package DevExtreme.AspNet.Data

## Custom Model Binder

The server needs a custom model binder that receives data loading options from the client and binds them to the model. Refer to the links below for instructions on how to implement it.

- [ASP.NET Core](https://github.com/DevExpress/DevExtreme.AspNet.Data/blob/master/net/Sample/DataSourceLoadOptions.cs)
- [Web API](https://github.com/DevExpress/devextreme-examples/blob/17_2/datagrid-webapi/datagrid-webapi/DataSourceLoadOptions.cs)
- [ASP.NET MVC 5](https://github.com/DevExpress/devextreme-examples/blob/17_2/datagrid-mvc5/datagrid-mvc5/DataSourceLoadOptions.cs)

**NOTE:** If you use `DevExtreme.AspNet.Data` with [DevExtreme-based ASP.NET Core controls](https://docs.devexpress.com/AspNetCore/400263) or [DevExtreme ASP.NET MVC 5 controls](https://docs.devexpress.com/DevExtremeAspNetMvc/400943/), you do not need to implement the custom model binder because it is included in these libraries.

## Controller Example

- [ASP.NET Core](https://github.com/DevExpress/DevExtreme.AspNet.Data/blob/master/net/Sample/Controllers/NorthwindController.cs)
- [Web API](https://github.com/DevExpress/devextreme-examples/blob/17_2/datagrid-webapi/datagrid-webapi/Controllers/OrdersController.cs)
- [ASP.NET MVC 5](https://github.com/DevExpress/devextreme-examples/blob/17_2/datagrid-mvc5/datagrid-mvc5/Controllers/OrdersController.cs)

## API Reference

The server-side API is documented [here](https://devexpress.github.io/DevExtreme.AspNet.Data/net/api/DevExtreme.AspNet.Data.html).

## See Also

- [Sample ASP.NET Core project](https://github.com/DevExpress/DevExtreme.AspNet.Data/tree/master/net/Sample)
- [KB T334360 - How to implement a data service that supports remote operations for dxDataGrid](https://www.devexpress.com/Support/Center/Example/Details/T334360)
