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

        function send(operation, requiresKey, ajaxSettings, customSuccessHandler) {
            var d = $.Deferred();

            if(requiresKey && !keyExpr) {
                d.reject(new Error("Primary key is not specified (operation: '" + operation + "', url: '" + ajaxSettings.url + "')"));
            } else {
                if(operation === "load")
                    ajaxSettings.cache = false;

                if(onBeforeSend)
                    onBeforeSend(operation, ajaxSettings);

                $.ajax(ajaxSettings)
                    .done(function(res) {
                        if(customSuccessHandler)
                            customSuccessHandler(d, res);
                        else
                            d.resolve(res);
                    })
                    .fail(function(xhr, textStatus) {
                        var message = getErrorMessageFromXhr(xhr);
                        if(message)
                            d.reject(message);
                        else
                            d.reject(xhr, textStatus);
                    });
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
            var result = {};

            if(isCountQuery)
                result.isCountQuery = isCountQuery;

            if(options) {

                $.each(["skip", "take", "requireTotalCount", "requireGroupCount"], function() {
                    if(this in options)
                        result[this] = options[this];
                });

                var normalizeSorting = DX.data.utils.normalizeSortingInfo,
                    group = options.group,
                    filter = options.filter;

                if(options.sort)
                    result.sort = JSON.stringify(normalizeSorting(options.sort));

                if(group) {
                    if(!isAdvancedGrouping(group))
                        group = normalizeSorting(group);
                    result.group = JSON.stringify(group);
                }

                if($.isArray(filter)) {
                    filter = $.extend(true, [], filter);
                    stringifyDatesInFilter(filter);
                    result.filter = JSON.stringify(filter);
                }

                if(options.totalSummary)
                    result.totalSummary = JSON.stringify(options.totalSummary);

                if(options.groupSummary)
                    result.groupSummary = JSON.stringify(options.groupSummary);
            }

            $.extend(result, loadParams);

            return result;
        }

        return {
            key: keyExpr,

            load: function(loadOptions) {
                return send(
                    "load",
                    false,
                    {
                        url: loadUrl,
                        data: loadOptionsToActionParams(loadOptions)
                    },
                    function(d, res) {
                        if("data" in res)
                            d.resolve(res.data, { totalCount: res.totalCount, summary: res.summary, groupCount: res.groupCount });
                        else
                            d.resolve(res);
                    }
                );
            },

            totalCount: function(loadOptions) {
                return send("load", false, {
                    url: loadUrl,
                    data: loadOptionsToActionParams(loadOptions, true)
                });
            },

            byKey: function(key) {
                return send(
                    "load",
                    true,
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
                return send("update", true, {
                    url: updateUrl,
                    type: options.updateMethod || "PUT",
                    data: {
                        key: serializeKey(key),
                        values: JSON.stringify(values)
                    }
                });
            },

            insert: insertUrl && function(values) {
                return send("insert", true, {
                    url: insertUrl,
                    type: options.insertMethod || "POST",
                    data: { values: JSON.stringify(values) }
                });
            },

            remove: deleteUrl && function(key) {
                return send("delete", true, {
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

    function serializeDate(date) {

        function zpad(text, len) {
            text = String(text);
            while(text.length < len)
                text = "0" + text;
            return text;
        }

        var builder = [1 + date.getMonth(), "/", date.getDate(), "/", date.getFullYear()],
            h = date.getHours(),
            m = date.getMinutes(),
            s = date.getSeconds(),
            f = date.getMilliseconds();

        if(h + m + s + f > 0)
            builder.push(" ", zpad(h, 2), ":", zpad(m, 2), ":", zpad(s, 2), ".", zpad(f, 3));        

        return builder.join("");
    }

    function stringifyDatesInFilter(crit) {
        $.each(crit, function(k, v) {
            switch($.type(v)) {
                case "array":
                    stringifyDatesInFilter(v);
                    break;
                case "date":
                    crit[k] = serializeDate(v);
                    break;
            }
        });
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

    function getErrorMessageFromXhr(xhr) {
        var mime = xhr.getResponseHeader("Content-Type"),
            responseText = xhr.responseText;

        if(!mime)
            return null;

        if(mime.indexOf("text/plain") === 0)
            return responseText;

        if(mime.indexOf("application/json") === 0) {
            var jsonObj = safeParseJSON(responseText);

            if(typeof jsonObj === "string")
                return jsonObj;

            if($.isPlainObject(jsonObj)) {
                for(var key in jsonObj) {
                    if(typeof jsonObj[key] === "string")
                        return jsonObj[key];
                }
            }

            return responseText;
        }

        return null;
    }

    function safeParseJSON(json) {
        try {
            return JSON.parse(json);
        } catch(x) {
            return null;
        }
    }

    $.extend(DX.data, {
        AspNet: {
            createStore: createStore
        }
    });

})(jQuery, window.DevExpress);