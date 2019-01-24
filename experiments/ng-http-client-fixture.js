/* eslint-env browser */
(function($, http) {
    "use strict";

    var nonce = Date.now();

    var httpClient = new http.HttpClient(
        new http.HttpXhrBackend({
            build: function() {
                return new XMLHttpRequest();
            }
        })
    );

    function createXhrSurrogate(response) {
        function getResponseHeader(name) {
            return response.headers.get(name);
        }

        function makeResponseText() {
            var error = response.error;
            if(error === undefined)
                return "N/A";

            if(typeof error !== "string" || String(getResponseHeader("Content-Type")).indexOf("application/json") === 0)
                return JSON.stringify(error);

            return error;
        }

        return {
            status: response.status,
            statusText: response.statusText,
            getResponseHeader: getResponseHeader,
            responseText: makeResponseText()
        };
    }

    $.ajax = function(options) {
        var d = $.Deferred();

        var method = (options.method || "get").toLowerCase();
        var data = options.data;
        var xhrFields = options.xhrFields;

        if(options.cache === false && method === "get" && data)
            data._ = nonce++;

        httpClient
            .request(
                method,
                options.url,
                {
                    params: data,
                    responseType: options.dataType,
                    headers: options.headers,
                    withCredentials: xhrFields && xhrFields.withCredentials,
                    observe: "response"
                }
            )
            .subscribe(
                function(response) {
                    d.resolve(response.body, "success", createXhrSurrogate(response));
                },
                function(response) {
                    d.reject(createXhrSurrogate(response), "error");
                }
            );

        return d.promise();
    };

})(window.jQuery, window.ng.common.http);
