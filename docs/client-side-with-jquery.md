# Client Side with jQuery

## Installation

The client-side part is the `dx.aspnet.data.js` script. You can install it in one of the following ways.

* Using [npm](https://www.npmjs.com/package/devextreme-aspnet-data). 

    Run the following command in the command line:

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

## API Reference

The client-side API consists of a single method, `DevExpress.data.AspNet.createStore`, that creates a [CustomStore](https://js.devexpress.com/Documentation/ApiReference/Data_Layer/CustomStore/), configures it to reach the controller from the client side, and returns its instance. When calling this method, pass in an object with the following properties:

- `key` - the key property;       
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
    The operation to be performed by the request. Can be "load", "update", "insert" or "delete".
    - `jQueryAjaxSettings`: `object`      
    Settings configuring the request. For details, refer to the [jQuery.ajax()](http://api.jquery.com/jquery.ajax/) description.
- `onAjaxError` - a function to be called when an Ajax request fails; accepts the following parameter:
    - `e`: `object`   
    Information about the event; contains the following properties:
        - `xhr`: [`jqXHR`](http://api.jquery.com/jQuery.ajax/#jqXHR) when using jQuery;  [`XMLHttpRequest`](https://developer.mozilla.org/en-US/docs/Web/API/XMLHttpRequest) otherwise    
        The request object.
        - `error`: `string` | [`Error`](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Error)    
        The error message. You can assign a custom error message or JavaScript Error object to this property.
- `errorHandler` - a function to be executed when the store throws an error; accepts a JavaScript [Error](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Error) object as the parameter.

You can find an example with jQuery [here](https://github.com/DevExpress/DevExtreme.AspNet.Data/blob/master/net/Sample/Views/Home/Index.cshtml).

DevExtreme ASP.NET MVC Controls call the `DevExpress.data.AspNet.createStore` method internally. To configure the parameters, use the lambda expression of the `DataSource()` method.

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