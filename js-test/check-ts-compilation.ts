import * as AspNetData from '../js/dx.aspnet.data'

let store = AspNetData.createStore({
    key: 'id',
    errorHandler: e => alert(e.message),
    loadUrl: "http://example.com",
    onBeforeSend: (operation, ajax) => {
        ajax.headers['X-Custom'] = 'value'
    }
})

store.load({ sort: "name" }).done(r => {

})
