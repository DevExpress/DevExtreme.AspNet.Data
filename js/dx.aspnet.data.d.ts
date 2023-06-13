import DxCustomStore from "devextreme/data/custom_store";

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

    onBeforeSend?: (operation: string, ajaxSettings: JQueryAjaxSettings) => void | PromiseLike<any>| any,
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

declare class CustomStore<
    TItem = any,
    TKey = any,
> extends DxCustomStore<TItem, TKey> {
    byKey(key: TKey): Promise<TItem>;
}

export function createStore(options: Options): CustomStore;
