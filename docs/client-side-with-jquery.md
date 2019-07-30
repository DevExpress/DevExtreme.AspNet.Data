# Client Side with jQuery

## Installation

The `dx.aspnet.data.js` script is the client-side part. You can install it in one of the following ways.

* Use [npm](https://www.npmjs.com/package/devextreme-aspnet-data).

    1. Run the following command in the command line:

        ```
        npm install devextreme-aspnet-data
        ```
        
    2. Link the `dx.aspnet.data.js` script on your page:

        ```html
        <script src="/node_modules/devextreme-aspnet-data/js/dx.aspnet.data.js"></script>
        ```

* Use [unpkg](https://unpkg.com/).

    Link the `dx.aspnet.data.js` script on your page in the following way:

    ```html
    <script src="https://unpkg.com/devextreme-aspnet-data/js/dx.aspnet.data.js"></script>
    ```

* Use [bower](https://libraries.io/bower/devextreme-aspnet-data).

    **NOTE: Since Bower is deprecated we do not recommend you use this approach.**

    1. Run the following command in the command line:

        ```
        bower install devextreme-aspnet-data
        ```

        ... or add `devextreme-aspnet-data` to the *bower.json* file's `dependencies` section.

        ```
        "dependencies": {
            ...
            "devextreme-aspnet-data": "^2"
        }
        ```

    2. Link the `dx.aspnet.data.js` script on your page:

        ```html
        <script src="/bower_components/devextreme-aspnet-data/js/dx.aspnet.data.js"></script>
        ```

#### See Also
- [Install DevExtreme Using npm](https://js.devexpress.com/Documentation/Guide/Getting_Started/Installation/npm_Package/)
- [Install DevExtreme Using Bower](https://js.devexpress.com/Documentation/Guide/Getting_Started/Installation/Bower_Package/)

## API Reference

The client-side API consists of the `DevExpress.data.AspNet.createStore` method that returns a [`CustomStore`](https://js.devexpress.com/DevExtreme/ApiReference/Data_Layer/CustomStore/)'s instance. This instance is configured to access a controller.

### Configuration

When you call the `DevExpress.data.AspNet.createStore` method, pass an object with the properties described below.

- `cacheRawData` - refer to [`CustomStore.cacheRawData`](https://js.devexpress.com/DevExtreme/ApiReference/Data_Layer/CustomStore/Configuration/#cacheRawData).
- `deleteMethod` - the HTTP method for delete requests; `"DELETE"` by default.
- `deleteUrl` - the URL used to delete data.
- `errorHandler` - refer to [`CustomStore.errorHandler`](https://js.devexpress.com/DevExtreme/ApiReference/Data_Layer/CustomStore/Configuration/#errorHandler).
- `insertMethod` - the HTTP method for insert requests; `"POST"` by default.
- `insertUrl` - the URL used to insert data.
- `key`- refer to [`CustomStore.key`](https://js.devexpress.com/DevExtreme/ApiReference/Data_Layer/CustomStore/Configuration/#key).
- `loadMethod` - the HTTP method for load requests; `"GET"` by default.
- `loadMode` - refer to [`CustomStore.loadMode`](https://js.devexpress.com/DevExtreme/ApiReference/Data_Layer/CustomStore/Configuration/#loadMode).
- `loadParams` - additional parameters that should be passed to `loadUrl`.
- `loadUrl` - the URL used to load data.
- `onAjaxError` - a function to be called when an AJAX request fails.
  
    ```js
    onAjaxError: (e: { xhr, error }) => void
    ```

    The `e` object has the following properties:

    Property  | Type | Description
    -- | -- | --
    `xhr` | [`jqXHR`](http://api.jquery.com/jQuery.ajax/#jqXHR) for jQuery;  [`XMLHttpRequest`](https://developer.mozilla.org/en-US/docs/Web/API/XMLHttpRequest) otherwise | The request object.
    `error` | `string` or [`Error`](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Error) | The error object. You can assign a custom error message or a JavaScript `Error` object.

- `onBeforeSend` - a function that customizes the request before it is sent.

    ```js
    onBeforeSend: (operation, ajaxSettings) => void
    ```

    Parameter  | Type | Description
    --- | -- | ----
    `operation` | `string` | The operation to be performed by the request: `"load"`, `"update"`, `"insert"`, or `"delete"`.
    `ajaxSettings` | `object` | Request settings. Refer to [`jQuery.ajax()`](http://api.jquery.com/jquery.ajax/).

- `onInserted` - refer to [`CustomStore.onInserted`](https://js.devexpress.com/DevExtreme/ApiReference/Data_Layer/CustomStore/Configuration/#onInserted).
- `onInserting` - refer to [`CustomStore.onInserting`](https://js.devexpress.com/DevExtreme/ApiReference/Data_Layer/CustomStore/Configuration/#onInserting).
- `onLoaded` - refer to [`CustomStore.onLoaded`](https://js.devexpress.com/DevExtreme/ApiReference/Data_Layer/CustomStore/Configuration/#onLoaded).
- `onLoading` - refer to [`CustomStore.onLoading`](https://js.devexpress.com/DevExtreme/ApiReference/Data_Layer/CustomStore/Configuration/#onLoading).
- `onModified` - refer to [`CustomStore.onModified`](https://js.devexpress.com/DevExtreme/ApiReference/Data_Layer/CustomStore/Configuration/#onModified).
- `onModifying` - refer to [`CustomStore.onModifying`](https://js.devexpress.com/DevExtreme/ApiReference/Data_Layer/CustomStore/Configuration/#onModifying).
- `onPush` - refer to [`CustomStore.onPush`](https://js.devexpress.com/DevExtreme/ApiReference/Data_Layer/CustomStore/Configuration/#onPush).
- `onRemoved` - refer to [`CustomStore.onRemoved`](https://js.devexpress.com/DevExtreme/ApiReference/Data_Layer/CustomStore/Configuration/#onRemoved).
- `onRemoving` - refer to [`CustomStore.onRemoving`](https://js.devexpress.com/DevExtreme/ApiReference/Data_Layer/CustomStore/Configuration/#onRemoving).
- `onUpdated` - refer to [`CustomStore.onUpdated`](https://js.devexpress.com/DevExtreme/ApiReference/Data_Layer/CustomStore/Configuration/#onUpdated).
- `onUpdating` - refer to [`CustomStore.onUpdating`](https://js.devexpress.com/DevExtreme/ApiReference/Data_Layer/CustomStore/Configuration/#onUpdating).
- `updateMethod` - the HTTP method for update requests; `"PUT"` by default.
- `updateUrl` - the URL used to update data.

### Methods and Events

Refer to the `CustomStore` [methods](https://js.devexpress.com/DevExtreme/ApiReference/Data_Layer/CustomStore/Methods/) and [events](https://js.devexpress.com/DevExtreme/ApiReference/Data_Layer/CustomStore/Events/) for a list of available methods and events.

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
