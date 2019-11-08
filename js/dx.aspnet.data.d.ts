import CustomStore from "devextreme/data/custom_store";

interface Options {
    key?: string|Array<string>,
    errorHandler?: (e: Error) => void,

    loadUrl?: string,
    loadParams?: Object,
    loadMethod?: string,

    updateUrl?: string,
    updateMethod?: string,

    insertUrl?: string,
    insertMethod?: string,

    deleteUrl?: string,
    deleteMethod?: string,

    loadMode?: "processed" | "raw",
    cacheRawData?: boolean,

    onBeforeSend?: (operation: string, ajaxSettings: JQueryAjaxSettings) => void,
    onAjaxError?: (e: { xhr: JQueryXHR, error: string | Error }) => void

    onLoading?: (loadOptions: any) => void;
    onLoaded?: (result: Array<any>) => void;

    onInserting?: (values: any) => void;
    onInserted?: (values: any, key: any) => void;

    onUpdating?: (key: any, values: any) => void;
    onUpdated?: (key: any, values: any) => void;

    onRemoving?: (key: any) => void;
    onRemoved?: (key: any) => void;

    onModifying?: Function;
    onModified?: Function;

    onPush?: (changes: Array<any>) => void;
}

export function createStore(options: Options): CustomStore;
