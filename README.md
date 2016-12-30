# DevExtreme ASP.NET Data

This extension enables [DevExtreme client-side widgets](https://js.devexpress.com) to consume data from the server in ASP.NET applications. A widget communicates with the server in the following manner: the widget sends data loading options (filtering, grouping, sorting, and other options) to the server, the server processes data according to these options and then sends processed data back to the widget. In this way, you delegate all intensive data operations from the client to the server, thus significantly improving the performance of DevExtreme widgets. This extension can be used directly with the widgets as well as with their [server-side wrappers](https://js.devexpress.com/Documentation/16_2/Guide/ASP.NET_MVC_Wrappers/Prerequisites_and_Installation/).

## Installation

The server-side part of the extension is the `DevExtreme.AspNet.Data.dll` assembly. Install it [from NuGet](http://www.nuget.org/packages/DevExtreme.AspNet.Data/) with the following command.

    Install-Package DevExtreme.AspNet.Data

The client-side part is the `dx.aspnet.data.js` script. Install it [using bower](https://libraries.io/bower/devextreme-aspnet-data) in one of the following ways.

* With the following command in the bower command line.

        bower install devextreme-aspnet-data

* By adding `devextreme-aspnet-data` to the *bower.json* file into the `dependencies` section.

    ```
    "dependencies": {
        ...
        "devextreme-aspnet-data": "^1"
    }
    ```

After installation, link the `dx.aspnet.data.js` script *after* the [DevExtreme scripts](https://js.devexpress.com/Documentation/16_2/Guide/Getting_Started/Installation/Bower_Package/) on your page.

```HTML
<script src="bower_components/devextreme-aspnet-data/js/dx.aspnet.data.js"></script>
```

## Custom Model Binder

The server needs a custom model binder that will receive data loading options from the client and bind them to the model. The following links show how to implement it.

- [Web API](https://www.devexpress.com/Support/Center/Example/Details/T334360) - see the `WebApiDataSourceLoadOptions.cs` file;
- [ASP.NET Core MVC](https://github.com/DevExpress/DevExtreme.AspNet.Data/blob/master/net/Sample/DataSourceLoadOptions.cs)

**NOTE:** If you use `DevExtreme.AspNet.Data` along with [DevExtreme ASP.NET MVC Wrappers](https://js.devexpress.com/Documentation/16_2/Guide/ASP.NET_MVC_Wrappers/Prerequisites_and_Installation/), you do not need to implement the custom model binder, because it is already done in the ASP.NET MVC Wrappers library.

## Controller Example

- [Web API](https://www.devexpress.com/Support/Center/Example/Details/T334360) - see the `CategoriesController.cs` file;
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
- [DataGrid - Use CustomStore](https://js.devexpress.com/Documentation/16_2/Guide/Widgets/DataGrid/Use_CustomStore/)
- [PivotGrid - Use CustomStore](https://js.devexpress.com/Documentation/16_2/Guide/Widgets/PivotGrid/Use_CustomStore/)