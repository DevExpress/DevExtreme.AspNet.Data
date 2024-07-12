# Server Side Configuration

## Installation

The server-side part of the extension is the `DevExtreme.AspNet.Data.dll` assembly. Install it [from NuGet](http://www.nuget.org/packages/DevExtreme.AspNet.Data/) with the following command.

    Install-Package DevExtreme.AspNet.Data

## Custom Model Binder

The server needs a custom model binder that receives data loading options from the client and binds them to the model. Refer to the links below for instructions on how to implement it.

- [ASP.NET Core](https://github.com/DevExpress/DevExtreme.AspNet.Data/blob/master/net/Sample/DataSourceLoadOptions.cs)
- [Web API](https://github.com/DevExpress-Examples/devextreme-datagrid-with-webapi/blob/23.1.3%2B/datagrid-webapi/DataSourceLoadOptions.cs)
- [ASP.NET MVC 5](https://github.com/DevExpress-Examples/devextreme-datagrid-mvc5/blob/23.1.3%2B/datagrid-mvc5/DataSourceLoadOptions.cs)

**NOTE:** If you use `DevExtreme.AspNet.Data` with [DevExtreme-based ASP.NET Core controls](https://docs.devexpress.com/AspNetCore/400263) or [DevExtreme ASP.NET MVC 5 controls](https://docs.devexpress.com/DevExtremeAspNetMvc/400943/), you do not need to implement the custom model binder because it is included in these libraries.

## Controller Example

- [ASP.NET Core](https://github.com/DevExpress/DevExtreme.AspNet.Data/blob/master/net/Sample/Controllers/NorthwindController.cs)
- [Web API](https://github.com/DevExpress-Examples/devextreme-datagrid-with-webapi/blob/23.1.3%2B/datagrid-webapi/Controllers/OrdersController.cs)
- [ASP.NET MVC 5](https://github.com/DevExpress-Examples/devextreme-datagrid-mvc5/blob/23.1.3%2B/datagrid-mvc5/Controllers/OrdersController.cs)

## API Reference

The server-side API is documented [here](https://devexpress.github.io/DevExtreme.AspNet.Data/net/api/DevExtreme.AspNet.Data.html).

## See Also

- [Sample ASP.NET Core project](https://github.com/DevExpress/DevExtreme.AspNet.Data/tree/master/net/Sample)
- [DataGrid - Web API Service demo](https://js.devexpress.com/Demos/WidgetsGallery/Demo/DataGrid/WebAPIService/)
