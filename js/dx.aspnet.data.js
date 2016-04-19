// https://github.com/DevExpress/DevExtreme.AspNet.Data
// Copyright (c) Developer Express Inc.

// jshint strict: true, browser: true, jquery: true, undef: true, unused: true, eqeqeq: true

(function($, DX) {
    "use strict";

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

        function send(operation, ajaxSettings, customSuccessHandler) {
            var d = $.Deferred();

            if(operation !== "load" && !keyExpr) {
                d.reject(new Error("Primary key is not specified (operation: '" + operation + "', url: '" + ajaxSettings.url + "')"));
            } else {
                if(onBeforeSend)
                    onBeforeSend(operation, ajaxSettings);

                $.ajax(ajaxSettings)
                    .done(function(res) {
                        if(customSuccessHandler)
                            customSuccessHandler(d, res);
                        else
                            d.resolve(res);
                    })
                    .fail(d.reject);
            }

            return d.promise();
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

            var normalizeSorting = DX.data.utils.normalizeSortingInfo,
                group = options.group;

            if(options.sort)
                result.sort = JSON.stringify(normalizeSorting(options.sort));

            if(group) {
                if(!isAdvancedGrouping(group))
                    group = normalizeSorting(group);
                result.group = JSON.stringify(group);
            }

            if($.isArray(options.filter))
                result.filter = JSON.stringify(options.filter);

            if(options.totalSummary)
                result.totalSummary = JSON.stringify(options.totalSummary);

            if(options.groupSummary)
                result.groupSummary = JSON.stringify(options.groupSummary);

            $.extend(result, loadParams);

            return result;
        }

        return {
            key: keyExpr,

            load: function(loadOptions) {
                return send(
                    "load", 
                    {
                        url: loadUrl,
                        data: loadOptionsToActionParams(loadOptions)
                    },
                    function(d, res) {
                        if("data" in res)
                            d.resolve(res.data, { totalCount: res.totalCount, summary: res.summary });
                        else
                            d.resolve(res);
                    }
                );
            },

            totalCount: function(loadOptions) {
                return send("load", {
                    url: loadUrl,
                    data: loadOptionsToActionParams(loadOptions, true)
                });
            },

            byKey: function(key) {
                return send(
                    "load",
                    {
                        url: loadUrl,
                        data: loadOptionsToActionParams({ filter: filterByKey(key) })
                    },
                    function(d, res) {
                        d.resolve(res[0]);
                    }
                );
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

    function isAdvancedGrouping(expr) {
        if(!$.isArray(expr))
            return false;

        for(var i = 0; i < expr.length; i++) {
            if("groupInterval" in expr[i] || "isExpanded" in expr[i])
                return true;
        }

        return false;
    }

    $.extend(DX.data, {
        AspNet: {
            createStore: createStore
        }
    });

})(jQuery, window.DevExpress);