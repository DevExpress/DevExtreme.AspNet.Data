import * as AspNetData from '../js/dx.aspnet.data'

let store = AspNetData.createStore({
    key: 'id',
    errorHandler: e => alert(e.message),
    loadUrl: "http://example.com",
    onBeforeSend: (operation, ajax) => {
        ajax.headers['X-Custom'] = 'value'
    },
    onAjaxError: e => {
        e.error = e.xhr.responseText;
        e.error = new Error();
    }
})

store.load({ sort: "name" }).done(r => {

})

AspNetData.createStore({
    loadMode: "raw",
    cacheRawData: false,

    onLoading: (loadOptions) => console.log(loadOptions.filter),
    onLoaded: (result) => console.log(result.length),

    onInserting: (values) => console.log(values),
    onInserted: (values, key) => console.log(values, key),

    onUpdating: (key, values) => console.log(key, values),
    onUpdated: (key, values) => console.log(key, values),

    onRemoving: (key) => console.log(key),
    onRemoved: (key) => console.log(key),

    onModifying: () => console.log("modifying"),
    onModified: () => console.log("modified"),

    onPush: (changes) => console.log(changes.length)
})
