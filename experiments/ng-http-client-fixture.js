/* eslint-env browser */

(function() {
    "use strict";

    function createHttpClient(http) {
        return new http.HttpClient(
            new http.HttpXhrBackend({
                build: function() {
                    return new XMLHttpRequest();
                }
            })
        );
    }

    var ajax;
    var createNgHttpClientSendRequestFunc;

    if(typeof define === "function" && define.amd) {
        define(function(require) {
            var http = require("@angular/common/bundles/common-http.umd");

            ajax = require("devextreme/core/utils/ajax");
            createNgHttpClientSendRequestFunc = require("./ng-http-client-helper");

            ajax.inject({
                sendRequest: createNgHttpClientSendRequestFunc(
                    createHttpClient(http),
                    require("devextreme/core/utils/deferred").Deferred,
                    http.HttpParams
                )
            });
        });
    } else if (typeof module === "object" && module.exports) {
        throw "TODO";
    } else {
        var $ = window.jQuery;
        $.ajax = window.DevExpress.data.AspNet.createNgHttpClientSendRequestFunc(
            createHttpClient(window.ng.common.http),
            $.Deferred,
            window.ng.common.http.HttpParams
        );
    }
})();
