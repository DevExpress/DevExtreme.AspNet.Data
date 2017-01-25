import CustomStore from "devextreme/data/custom_store";

interface Options {
    key?: string|Array<string>,

    loadUrl?: string,
    loadParams?: Object,

    updateUrl?: string,
    updateMethod?: string,

    insertUrl?: string,
    insertMethod?: string,

    deleteUrl?: string,
    deleteMethod?: string,

    onBeforeSend?: (operation: string, ajaxSettings: JQueryAjaxSettings) => void
}

export function createStore(options: Options): CustomStore;
