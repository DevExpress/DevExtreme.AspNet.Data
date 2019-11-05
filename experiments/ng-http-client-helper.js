/* eslint-env browser */

(function(sendRequestFactory) {
    "use strict";

    if(typeof define === "function" && define.amd) {
        define(function(require, exports, module) {
            module.exports = sendRequestFactory;
        });
    } else if (typeof module === "object" && module.exports) {
        throw "TODO";
    } else {
        window.DevExpress.data.AspNet.createNgHttpClientSendRequestFunc = sendRequestFactory;
    }

})(function(httpClient, Deferred) {
    "use strict";

    var nonce = Date.now();

    function createXhrSurrogate(response) {
        function getResponseHeader(name) {
            return response.headers.get(name);
        }

        function makeResponseText() {
            var body = "error" in response ? response.error : response.body;

            if(typeof body !== "string" || String(getResponseHeader("Content-Type")).indexOf("application/json") === 0)
                return JSON.stringify(body);

            return body;
        }

        return {
            status: response.status,
            statusText: response.statusText,
            getResponseHeader: getResponseHeader,
            responseText: makeResponseText()
        };
    }

    function getAcceptHeader(options) {
        var dataType = options.dataType;
        var accepts = options.accepts;
        var fallback = ',*/*;q=0.01';

        if(dataType && accepts && accepts[dataType])
          return accepts[dataType] + fallback;

        switch (dataType) {
          case 'json': return 'application/json, text/javascript' + fallback;
          case 'text': return 'text/plain' + fallback;
          case 'xml': return 'application/xml, text/xml' + fallback;
          case 'html': return 'text/html' + fallback;
          case 'script': return 'text/javascript, application/javascript, application/ecmascript, application/x-ecmascript' + fallback;
        }

        return '*/*';
    }

    return function(options) {
        var d = Deferred();

        var method = (options.method || "get").toLowerCase();
        var headers = Object.assign({}, options.headers); // TODO Object.assign
        var data = options.data;
        var xhrFields = options.xhrFields;

        if(options.cache === false && method === "get" && data)
            data._ = nonce++;

        if(!headers.Accept)
            headers.Accept = getAcceptHeader(options);

        httpClient
            .request(
                method,
                options.url,
                {
                    params: data,
                    responseType: options.dataType,
                    headers: headers,
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
});
