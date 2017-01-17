# DevExtreme ASP.NET Data

[![Build status](https://ci.appveyor.com/api/projects/status/6jyq7ocmvbuhvypo/branch/master?svg=true)](https://ci.appveyor.com/project/dxrobot/devextreme-aspnet-data/branch/master)
[![NuGet](https://img.shields.io/nuget/v/DevExtreme.AspNet.Data.svg)](https://www.nuget.org/packages/DevExtreme.AspNet.Data)
[![npm](https://img.shields.io/npm/v/devextreme-aspnet-data.svg)](https://www.npmjs.com/package/devextreme-aspnet-data)
[![Bower](https://img.shields.io/bower/v/devextreme-aspnet-data.svg)](https://libraries.io/bower/devextreme-aspnet-data)

This extension enables [DevExtreme client-side widgets](https://js.devexpress.com) to consume data from the server in ASP.NET applications. A widget communicates with the server in the following manner: the widget sends data loading options (filtering, grouping, sorting, and other options) to the server, the server processes data according to these options and then sends processed data back to the widget. In this way, you delegate all intensive data operations from the client to the server, thus significantly improving the performance of DevExtreme widgets. This extension can be used directly with the widgets as well as with their [server-side wrappers](https://js.devexpress.com/Documentation/16_2/Guide/ASP.NET_MVC_Wrappers/Prerequisites_and_Installation/).

## Installation

### Server-Side

The server-side part of the extension is the `DevExtreme.AspNet.Data.dll` assembly. Install it [from NuGet](http://www.nuget.org/packages/DevExtreme.AspNet.Data/) with the following command.

    Install-Package DevExtreme.AspNet.Data

### Client-Side

The client-side part is the `dx.aspnet.data.js` script. You can install it in one of the following ways.

* Using [npm](https://www.npmjs.com/package/devextreme-aspnet-data). 

    Run the following command in the command line.

        npm install devextreme-aspnet-data

* Using [bower](https://libraries.io/bower/devextreme-aspnet-data).     

    Run the following command in the command line...

        bower install devextreme-aspnet-data

    ... or add `devextreme-aspnet-data` to the *bower.json* file into the `dependencies` section.

    ```
    "dependencies": {
        ...
        "devextreme-aspnet-data": "^1"
    }
    ```

After installation, link the `dx.aspnet.data.js` script *after* the DevExtreme scripts on your page.

```HTML
<!-- if you have used npm -->
<script src="/node_modules/devextreme-aspnet-data/js/dx.aspnet.data.js"></script>
<!-- if you have used bower -->
<script src="/bower_components/devextreme-aspnet-data/js/dx.aspnet.data.js"></script>
```

#### See Also
- [Install DevExtreme Using npm](https://js.devexpress.com/Documentation/Guide/Getting_Started/Installation/npm_Package/)
- [Install DevExtreme Using Bower](https://js.devexpress.com/Documentation/Guide/Getting_Started/Installation/Bower_Package/)

## Custom Model Binder

The server needs a custom model binder that will receive data loading options from the client and bind them to the model. The following links show how to implement it.

- [Web API](https://github.com/DevExpress/devextreme-examples/blob/16_2/datagrid-webapi/datagrid-webapi/DataSourceLoadOptions.cs)
- [ASP.NET Core MVC](https://github.com/DevExpress/DevExtreme.AspNet.Data/blob/master/net/Sample/DataSourceLoadOptions.cs)

**NOTE:** If you use `DevExtreme.AspNet.Data` along with [DevExtreme ASP.NET MVC Wrappers](https://js.devexpress.com/Documentation/16_2/Guide/ASP.NET_MVC_Wrappers/Prerequisites_and_Installation/), you do not need to implement the custom model binder, because it is already done in the ASP.NET MVC Wrappers library.

## Controller Example

- [Web API](https://github.com/DevExpress/devextreme-examples/blob/16_2/datagrid-webapi/datagrid-webapi/Controllers/OrdersController.cs)
- [ASP.NET Core MVC](https://github.com/DevExpress/DevExtreme.AspNet.Data/blob/master/net/Sample/Controllers/NorthwindController.cs)

## Client-Side Method

To reach the controller from the client side, use the `DevExpress.data.AspNet.createStore` method. It accepts an object with the following fields.

- `key` - the key property;       
- `loadUrl` - the URL to the GET method;      
- `loadParams` - parameters that should be passed to the GET method (if there are any);       
- `updateUrl` - the URL to the POST method;       
- `insertUrl` - the URL to the PUT method;        
- `deleteUrl` - the URL to the DELETE method;     
- `onBeforeSend` - a function that customizes the query before it is sent.

You can find an example [here](https://github.com/DevExpress/DevExtreme.AspNet.Data/blob/master/net/Sample/Views/Home/Index.cshtml).

DevExtreme ASP.NET MVC Wrappers call the `DevExpress.data.AspNet.createStore` method internally. To configure the parameters, use the lambda expression of the `DataSource()` method.

```Razor
@(Html.DevExtreme().DataGrid()
    .DataSource(ds => ds.WebApi()
        .Controller("NorthwindContext")
        .Key("OrderID")
        .LoadAction("GetAllOrders")
        .InsertAction("InsertOrder")
        .UpdateAction("UpdateOrder")
        .DeleteAction("RemoveOrder")
    )
)
```

## See Also

- [Sample project](https://github.com/DevExpress/DevExtreme.AspNet.Data/tree/master/net/Sample)
- [DataGrid and Web API example](https://github.com/DevExpress/devextreme-examples/tree/16_2/datagrid-webapi)
- [PivotGrid and Web API example](https://github.com/DevExpress/devextreme-examples/tree/16_2/pivotgrid-webapi)
- [KB T334360 - How to implement a data service that supports remote operations for dxDataGrid](https://www.devexpress.com/Support/Center/Example/Details/T334360)
- [DataGrid - Use CustomStore](https://js.devexpress.com/Documentation/16_2/Guide/Widgets/DataGrid/Use_CustomStore/)
- [PivotGrid - Use CustomStore](https://js.devexpress.com/Documentation/16_2/Guide/Widgets/PivotGrid/Use_CustomStore/)