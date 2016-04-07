// https://github.com/DevExpress/DevExtreme.AspNet.Data
// Copyright (c) Developer Express Inc.

(function($, DX) {

    function createStore(options) {
        var store = new DX.data.CustomStore(createStoreConfig(options));
        store._useDefaultSearch = true;
        return store;
    }

    function createStoreConfig(options) {
        var keyExpr = options.key,
            loadUrl = options.loadUrl,
            loadParams = options.loadParams,
            updateUrl = options.updateUrl,
            insertUrl = options.insertUrl,
            deleteUrl = options.deleteUrl,
            onBeforeSend = options.onBeforeSend;

        function send(operation, settings) {
            if(onBeforeSend)
                onBeforeSend(operation, settings);

            return $.ajax(settings);
        }

        function filterByKey(keyValue) {
            if(!$.isArray(keyExpr))
                return [keyExpr, keyValue];

            return $.map(keyExpr, function(i) {
                return [[i, keyValue[i]]];
            });
        }

        function loadOptionsToActionParams(options, isCountQuery) {
            var result = {
                requireTotalCount: options.requireTotalCount,
                isCountQuery: isCountQuery,
                skip: options.skip,
                take: options.take,
            };

            var normalizeSorting = DX.data.utils.normalizeSortingInfo;

            if("sort" in options)
                result.sort = JSON.stringify(normalizeSorting(options.sort));

            if("group" in options)
                result.group = JSON.stringify(normalizeSorting(options.group));

            if($.isArray(options.filter))
                result.filter = JSON.stringify(options.filter);

            $.extend(result, loadParams);

            return result;
        }

        return {
            key: keyExpr,

            load: function(loadOptions) {
                var d = new $.Deferred();

                send("load", {
                    url: loadUrl,
                    data: loadOptionsToActionParams(loadOptions)
                }).done(function(res) {
                    if("totalCount" in res)
                        d.resolve(res.data, { totalCount: res.totalCount });
                    else
                        d.resolve(res);
                }).fail(function(error) {
                    d.reject(error);
                });

                return d.promise();
            },

            totalCount: function(loadOptions) {
                return send("load", {
                    url: loadUrl,
                    data: loadOptionsToActionParams(loadOptions, true)
                });
            },

            byKey: keyExpr && function(key) {
                var d = new $.Deferred();

                send("load", {
                    url: loadUrl,
                    data: loadOptionsToActionParams({ filter: filterByKey(key) })
                }).done(function(res) {
                    d.resolve(res[0]);
                }).fail(function(error) {
                    d.reject(error);
                });

                return d.promise();
            },

            update: updateUrl && function(key, values) {
                return send("update", {
                    url: updateUrl,
                    type: options.updateMethod || "PUT",
                    data: {
                        key: serializeKey(key),
                        values: JSON.stringify(values)
                    }
                });
            },

            insert: insertUrl && function(values) {
                return send("insert", {
                    url: insertUrl,
                    type: options.insertMethod || "POST",
                    data: { values: JSON.stringify(values) }
                });
            },

            remove: deleteUrl && function(key) {
                return send("delete", {
                    url: deleteUrl,
                    type: options.deleteMethod || "DELETE",
                    data: { key: serializeKey(key) }
                });
            }

        };
    }

    function serializeKey(key) {
        if(typeof key === "object")
            return JSON.stringify(key);

        return key;
    }

    $.extend(DX.data, {
        AspNet: {
            createStore: createStore
        }
    });

})(jQuery, DevExpress);