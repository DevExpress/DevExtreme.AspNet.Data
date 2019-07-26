# Client Side with jQuery

## Installation

The `dx.aspnet.data.js` script is the client-side part. You can install it in one of the following ways:

* Use [npm](https://www.npmjs.com/package/devextreme-aspnet-data). 

    Run the following command in the command line:

        npm install devextreme-aspnet-data

* Use [bower](https://libraries.io/bower/devextreme-aspnet-data).     

    Run the following command in the command line:

        bower install devextreme-aspnet-data

    ... or add `devextreme-aspnet-data` to the *bower.json* file's `dependencies` section.

    ```
    "dependencies": {
        ...
        "devextreme-aspnet-data": "^2"
    }
    ```

* Use [unpkg](https://unpkg.com/).

    Use the following URL to load the `dx.aspnet.data.js` script: [https://unpkg.com/devextreme-aspnet-data@2.4.2/js/dx.aspnet.data.js](https://unpkg.com/devextreme-aspnet-data@2.4.2/js/dx.aspnet.data.js).


After installation, link the `dx.aspnet.data.js` script *after* the DevExtreme scripts on your page.

```HTML
<!-- if you used npm -->
<script src="/node_modules/devextreme-aspnet-data/js/dx.aspnet.data.js"></script>
<!-- if you used bower -->
<script src="/bower_components/devextreme-aspnet-data/js/dx.aspnet.data.js"></script>
```

#### See Also
- [Install DevExtreme Using npm](https://js.devexpress.com/Documentation/Guide/Getting_Started/Installation/npm_Package/)
- [Install DevExtreme Using Bower](https://js.devexpress.com/Documentation/Guide/Getting_Started/Installation/Bower_Package/)

## API Reference

The client-side API consists of a single method, `DevExpress.data.AspNet.createStore`, that creates a [CustomStore](https://js.devexpress.com/DevExtreme/ApiReference/Data_Layer/CustomStore/), configures it to reach a controller from the client side, and returns its instance.

### Configuration

When you call the `DevExpress.data.AspNet.createStore` method, pass an object with the properties described below:

#### DevExtreme Properties

The following properties are directly mapped from [properties of DevExtreme CustomStore](https://js.devexpress.com/DevExtreme/ApiReference/Data_Layer/CustomStore/Configuration/):

- [cacheRawData](https://js.devexpress.com/DevExtreme/ApiReference/Data_Layer/CustomStore/Configuration/#cacheRawData)
- [errorHandler](https://js.devexpress.com/DevExtreme/ApiReference/Data_Layer/CustomStore/Configuration/#errorHandler)
- [key](https://js.devexpress.com/DevExtreme/ApiReference/Data_Layer/CustomStore/Configuration/#key)
- [loadMode](https://js.devexpress.com/DevExtreme/ApiReference/Data_Layer/CustomStore/Configuration/#loadMode)
- [onInserted](https://js.devexpress.com/DevExtreme/ApiReference/Data_Layer/CustomStore/Configuration/#onInserted)
- [onInserting](https://js.devexpress.com/DevExtreme/ApiReference/Data_Layer/CustomStore/Configuration/#onInserting)
- [onLoaded](https://js.devexpress.com/DevExtreme/ApiReference/Data_Layer/CustomStore/Configuration/#onLoaded)
- [onLoading](https://js.devexpress.com/DevExtreme/ApiReference/Data_Layer/CustomStore/Configuration/#onLoading)
- [onModified](https://js.devexpress.com/DevExtreme/ApiReference/Data_Layer/CustomStore/Configuration/#onModified)
- [onModifying](https://js.devexpress.com/DevExtreme/ApiReference/Data_Layer/CustomStore/Configuration/#onModifying)
- [onPush](https://js.devexpress.com/DevExtreme/ApiReference/Data_Layer/CustomStore/Configuration/#onPush)
- [onRemoved](https://js.devexpress.com/DevExtreme/ApiReference/Data_Layer/CustomStore/Configuration/#onRemoved)
- [onRemoving](https://js.devexpress.com/DevExtreme/ApiReference/Data_Layer/CustomStore/Configuration/#onRemoving)
- [onUpdated](https://js.devexpress.com/DevExtreme/ApiReference/Data_Layer/CustomStore/Configuration/#onUpdated)
- [onUpdating](https://js.devexpress.com/DevExtreme/ApiReference/Data_Layer/CustomStore/Configuration/#onUpdating)

#### Properties Specific to ASP.NET CustomStore

The following properties are specific to ASP.NET CustomStore:

- `loadUrl` - the URL used to load data;
- `loadParams` - additional parameters that should be passed to `loadUrl`;
- `updateUrl` - the URL used to update data;
- `insertUrl` - the URL used to insert data;
- `deleteUrl` - the URL used to delete data;
- `loadMethod` - the HTTP method for load requests; "GET" by default;
- `updateMethod` - the HTTP method for update requests; "PUT" by default;
- `insertMethod` - the HTTP method for insert requests; "POST" by default;
- `deleteMethod` - the HTTP method for delete requests; "DELETE" by default;
- `onBeforeSend` - a function that customizes the request before it is sent; accepts the following parameters:
    - `operation`: `string`
    The operation the request should perform. Can be "load", "update", "insert" or "delete".
    - `jQueryAjaxSettings`: `object`
    Settings configuring the request. Refer to the [jQuery.ajax()](http://api.jquery.com/jquery.ajax/) description for more information.
- `onAjaxError` - a function to be called when an Ajax request fails; accepts the following parameter:
    - `e`: `object`
    Information about the event; contains the following properties:
        - `xhr`: [`jqXHR`](http://api.jquery.com/jQuery.ajax/#jqXHR) when using jQuery;  [`XMLHttpRequest`](https://developer.mozilla.org/en-US/docs/Web/API/XMLHttpRequest) otherwise
        The request object.
        - `error`: `string` | [`Error`](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Error)
        The error message. You can assign a custom error message or JavaScript Error object to this property.

### Example

You can find a jQuery example [here](https://github.com/DevExpress/DevExtreme.AspNet.Data/blob/master/net/Sample/Views/Home/Index.cshtml).

[DevExtreme-based ASP.NET Core](https://docs.devexpress.com/AspNetCore/400263) and [DevExtreme ASP.NET MVC 5](https://docs.devexpress.com/DevExtremeAspNetMvc/400943/) controls call the `DevExpress.data.AspNet.createStore` method internally. To configure its parameters, use the `DataSource()` method's lambda expression.

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

- [DataGrid and Web API example](https://github.com/DevExpress/devextreme-examples/tree/17_2/datagrid-webapi)
- [PivotGrid and Web API example](https://github.com/DevExpress/devextreme-examples/tree/17_2/pivotgrid-webapi)
- [DataGrid in an MVC 5 App example](https://github.com/DevExpress/devextreme-examples/tree/17_2/datagrid-mvc5)
- [DataGrid - Use CustomStore](https://js.devexpress.com/Documentation/Guide/Widgets/DataGrid/Use_CustomStore/)
- [PivotGrid - Use CustomStore](https://js.devexpress.com/Documentation/Guide/Widgets/PivotGrid/Use_CustomStore/)
