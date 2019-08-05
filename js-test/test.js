/* eslint-env browser */
/* global ASPNET_DATA_SCRIPT:false, DevExpress:false, Promise:false */

(function(factory) {
    "use strict";

    if(typeof define === "function" && define.amd) {
        define(function(require) {
            factory(
                require("xhr-mock").default,
                require("devextreme/core/version"),
                require("devextreme/data/data_source"),
                require("devextreme/core/utils/ajax"),
                require(ASPNET_DATA_SCRIPT)
            );
        });
    } else if (typeof module === "object" && module.exports) {
        module.exports = factory(
            require("xhr-mock").default,
            require("devextreme/core/version"),
            require("devextreme/data/data_source"),
            require("devextreme/core/utils/ajax"),
            require(ASPNET_DATA_SCRIPT)
        );
    } else {
        factory(
            window.XHRMock,
            DevExpress.VERSION,
            DevExpress.data.DataSource,
            DevExpress.utils.ajax,
            DevExpress.data.AspNet
        );
    }

})(function(XHRMock, devextremeVersion, DataSource, ajaxUtility, AspNet) {
    "use strict";

    // https://github.com/karma-runner/karma-qunit/issues/57
    var QUnit = window.QUnit;

    devextremeVersion = devextremeVersion.split(".").map(Number);

    var createStore = AspNet.createStore,
        NEVER_RESOLVE = new Promise(function() { });

    function willRespondWithJson(obj) {
        XHRMock.use(function(req, res) {
            return res
                .status(200)
                .header("Content-Type", "application/json")
                .body(JSON.stringify(obj));
        });
    }

    function wontRespond() {
        XHRMock.use(function() {
            return NEVER_RESOLVE;
        });
    }

    function useLegacyStoreResult() {
        return devextremeVersion[0] < 18
            || devextremeVersion[0] === 18 && devextremeVersion[1] < 2;
    }

    QUnit.testStart(function() {
        XHRMock.setup();
    });

    QUnit.testDone(function() {
        XHRMock.teardown();
    });

    QUnit.module("error handling", function() {

        QUnit.test("require key", function(assert) {
            var done = assert.async(4);

            var store = createStore({
                loadUrl: "any",
                insertUrl: "any",
                updateUrl: "any",
                deleteUrl: "any"
            });

            function onFail(error) {
                assert.ok(error.message.indexOf("Primary key") > -1);
                done();
            }

            store.byKey(123).fail(onFail);
            store.insert({}).fail(onFail);
            store.update(123, {}).fail(onFail);
            store.remove(1).fail(onFail);
        });

        QUnit.test("server error", function(assert) {
            var done = assert.async(6);

            XHRMock.use(function(req, res) {
                return res.status(500).reason("Test Status");
            });

            var store = createStore({
                key: "id",
                loadUrl: "/",
                insertUrl: "/",
                updateUrl: "/",
                deleteUrl: "/"
            });

            function onFail(error) {
                assert.equal(error.message, "Test Status");
                done();
            }

            store.load().fail(onFail);
            store.totalCount().fail(onFail);
            store.byKey(123).fail(onFail);
            store.insert({}).fail(onFail);
            store.update(123, {}).fail(onFail);
            store.remove(123).fail(onFail);
        });

        QUnit.test("timeout", function(assert) {
            var done = assert.async();

            wontRespond();

            var store = createStore({
                loadUrl: "/",
                onBeforeSend: function(op, ajax) {
                    ajax.timeout = 1;
                }
            });

            store.load().fail(function(error) {
                assert.equal(error.message, "Network connection timeout");
                done();
            });
        });

        QUnit.module("extract message", function() {
            var SAMPLE_MESSAGE = "Sample error message";

            function testCase(mime, responseText, expectedMessage) {
                return function(assert) {
                    var done = assert.async();

                    XHRMock.use(function(req, res) {
                        return res
                            .status(400)
                            .reason("Bad Request")
                            .header("Content-Type", mime)
                            .body(responseText);
                    });

                    createStore({ loadUrl: "/" }).load().fail(function(error) {
                        assert.equal(error.message, expectedMessage);
                        done();
                    });
                };
            }

            QUnit.test("plain text", testCase("text/plain; charset=utf8", SAMPLE_MESSAGE, SAMPLE_MESSAGE));
            QUnit.test("json string", testCase("application/json", JSON.stringify(SAMPLE_MESSAGE), SAMPLE_MESSAGE));
            QUnit.test("json with string prop", testCase("application/json", JSON.stringify({ any: SAMPLE_MESSAGE }), SAMPLE_MESSAGE));
            QUnit.test("other json used verbatim", testCase("application/json", "[{}]", "[{}]"));
            QUnit.test("other mime", testCase("unknown/unknown", "any", "Bad Request"));
        });

        QUnit.test("Issue #146", function(assert) {
            var done = assert.async(),
                failCount = 0;

            XHRMock.use(function(req, res) {
                return res.status(200).body('"text"');
            });

            function handleFail(error) {
                assert.equal(error.message, "Unexpected response received");
                if(++failCount === 3)
                    done();
            }

            var store = createStore({
                loadUrl: "/",
                key: "any"
            });

            store.load().fail(handleFail);
            store.totalCount().fail(handleFail);
            store.byKey("any").fail(handleFail);
        });

        QUnit.test("errorHandler option", function(assert) {
            var done = assert.async();
            var reason = "Test Reason";

            XHRMock.use(function(req, res) {
                return res.status(500).reason(reason);
            });

            var store = createStore({
                loadUrl: "/",
                errorHandler: function(error) {
                    assert.equal(error.message, reason);
                    done();
                }
            });

            store.load();
        });

        QUnit.module("onAjaxError", function() {

            function testCase(handler, checker) {
                return function(assert) {
                    var done = assert.async();

                    XHRMock.use(function(req, res) {
                        return res.status(500).body('{ "code": 123 }');
                    });

                    var store = createStore({
                        loadUrl: "/",
                        onAjaxError: handler
                    });

                    store.load().fail(function(error) {
                        checker(assert, error);
                        done();
                    });
                }
            };

            QUnit.test("error = String", testCase(
                function(e) {
                    e.error = "Code " + JSON.parse(e.xhr.responseText).code;
                },
                function(assert, error) {
                    assert.equal(error.message, "Code 123");
                }
            ));

            QUnit.test("error = Error object", testCase(
                function(e) {
                    e.error = new Error("abc");
                    e.error.code = JSON.parse(e.xhr.responseText).code;
                },
                function(assert, error) {
                    assert.equal(error.message, "abc");
                    assert.equal(error.code, 123)
                }
            ));

        });
    });

    QUnit.module("load options", function() {

        QUnit.test("search becomes filter", function(assert) {
            var done = assert.async();

            XHRMock.use(function(req) {
                assert.equal(req.url().query.filter, '[["haystack","contains","needle"]]');
                done();
                return NEVER_RESOLVE;
            });

            var dataSource = new DataSource({
                store: createStore({ loadUrl: "/" }),
                searchExpr: "haystack",
                searchValue: "needle"
            });

            dataSource.load();
        });

        QUnit.test("dates in filter", function(assert) {
            var done = assert.async();

            XHRMock.use(function(req) {
                assert.equal(req.url().query.filter, '[["a","1/1/2000"],["b","1/1/2000 02:03:04.005"]]');
                done();
                return NEVER_RESOLVE;
            });

            createStore({ loadUrl: "/" }).load({
                filter: [["a", new Date(2000, 0, 1)], ["b", new Date(2000, 0, 1, 2, 3, 4, 5)]]
            });
        });

        QUnit.test("skip, take, requireTotalCount, requireGroupCount are passed in the request data", function(assert) {
            var done = assert.async();

            XHRMock.use(function(req) {
                var query = req.url().query;
                assert.strictEqual(query.requireGroupCount, "true");
                assert.strictEqual(query.skip, "0");
                assert.strictEqual(query.take, "10");
                assert.strictEqual(query.requireTotalCount, "false");
                done();
                return NEVER_RESOLVE;
            });

            createStore({ loadUrl: "/" }).load({
                requireGroupCount: true,
                skip: 0,
                take: 10,
                requireTotalCount: false
            });
        });

        QUnit.module("sort/group normalization", function() {

            function testCase(optionName, rawValue, normalizedValue) {
                return function(assert) {
                    var done = assert.async();

                    if(normalizedValue === "=")
                        normalizedValue = rawValue;

                    XHRMock.use(function(req) {
                        assert.deepEqual(JSON.parse(req.url().query[optionName]), normalizedValue);
                        done();
                        return NEVER_RESOLVE;
                    });

                    var loadOptions = {};
                    loadOptions[optionName] = rawValue;
                    createStore({ loadUrl: "/" }).load(loadOptions);
                };
            }

            QUnit.test("sort as string", testCase("sort", "s", [{ selector: "s", desc: false }]));
            QUnit.test("group as string", testCase("group", "g", [{ selector: "g", desc: false }]));
            QUnit.test("grid isExpanded", testCase("group", [{ selector: "g", isExpanded: false }], "="));
            QUnit.test("grid groupInterval", testCase("group", [{ selector: "g", groupInterval: 123 }], "="));
        });

        QUnit.module("select normalization", function() {

            function testCase(rawValue, normalizedValue) {
                return function(assert) {
                    var done = assert.async();

                    XHRMock.use(function(req) {
                        assert.deepEqual(JSON.parse(req.url().query.select), normalizedValue);
                        done();
                        return NEVER_RESOLVE;
                    });

                    createStore({ loadUrl: "/" }).load({ select: rawValue });
                };
            }

            QUnit.test("as string", testCase("abc", [ "abc" ]));
            QUnit.test("as array", testCase([ "abc" ], [ "abc" ]));
        });

        QUnit.test("undefined values", function(assert) {
            var done = assert.async();

            XHRMock.use(function(req) {
                assert.ok(!("take" in req.url().query));
                done();
                return NEVER_RESOLVE;
            });

            createStore({ loadUrl: "/" }).load({
                take: undefined
            });
        });

    });

    QUnit.module("loading", function() {

        QUnit.test("totalCount", function(assert) {
            var done = assert.async();

            willRespondWithJson(123);

            createStore({ loadUrl: "/" }).totalCount().done(function(r) {
                assert.strictEqual(r, 123);
                done();
            });
        });

        QUnit.test("totalCount can receive ResponseModel.LoadResult", function(assert) {
            var done = assert.async();

            willRespondWithJson({ totalCount: 123, data: null });

            createStore({ loadUrl: "/" }).totalCount().done(function(r) {
                assert.strictEqual(r, 123);
                done();
            });
        });

        QUnit.test("load returns array", function(assert) {
            var done = assert.async();

            willRespondWithJson([1, 2, 3]);

            createStore({ loadUrl: "/" }).load().done(function(r) {
                assert.deepEqual(r, [1, 2, 3]);
                done();
            });
        });

        QUnit.test("load returns structure", function(assert) {
            var done = assert.async();

            willRespondWithJson({
                data: [1, 2, 3],
                totalCount: 123,
                summary: [1, 2, 4],
                groupCount: 11
            });

            createStore({ loadUrl: "/" }).load().done(function(data, extra) {
                assert.deepEqual(data, [1, 2, 3]);
                assert.deepEqual(extra, {
                    totalCount: 123,
                    summary: [1, 2, 4],
                    groupCount: 11
                });
                done();
            });
        });

        QUnit.test("load returns incomplete structure", function(assert) {
            var done = assert.async();

            willRespondWithJson({ data: [1, 2, 3] });

            createStore({ loadUrl: "/" }).load().done(function(data, extra) {
                assert.deepEqual(extra, {
                    totalCount: -1,
                    groupCount: -1,
                    summary: null
                });
                done();
            });
        });

        QUnit.test("byKey", function(assert) {
            var done = assert.async();

            willRespondWithJson([ "first", "other" ]);

            createStore({ key: "id", loadUrl: "/" }).byKey(123).done(function(r) {
                assert.equal(r, "first");
                done();
            });
        });

        QUnit.test("byKey can receive ResponseModel.LoadResult", function(assert) {
            var done = assert.async();

            willRespondWithJson({ data: [ "item" ] });

            createStore({ key: "any", loadUrl: "/" }).byKey(123).done(function(r) {
                assert.equal(r, "item");
                done();
            });
        });

        QUnit.test("loadMode=raw", function(assert) {
            var done = assert.async();

            var store = createStore({
                key: "this",
                loadUrl: "/",
                loadMode: "raw",
                onBeforeSend: function(op, ajax) {
                    assert.deepEqual(ajax.data, { });
                }
            });

            var loadOptions = {
                skip: 1,
                take: 2,
                filter: [ "this", ">", 0 ],
                sort: [ { selector: "this", desc: true } ]
            };

            willRespondWithJson({ data: [ 0, 1, 2, 3 ]});

            Promise.all([

                store.load(loadOptions).done(function(r) {
                    assert.deepEqual(r, [ 2, 1 ]);
                }),

                store.byKey(3).done(function(obj) {
                    assert.equal(obj, 3);
                }),

                store.totalCount().done(function(count) {
                    assert.equal(count, 4);
                })

            ]).then(done);
        });
    });

    QUnit.module("check request data onBeforeSend", { beforeEach: wontRespond }, function() {

        QUnit.test("load, no arguments", function(assert) {
            var done = assert.async();

            var store = createStore({
                loadUrl: "/load",
                loadParams: { custom: 123 },
                onBeforeSend: function(op, ajax) {
                    assert.equal(op, "load");
                    assert.deepEqual(ajax.data, { custom: 123 });
                    done();
                }
            });

            store.load();
        });

        QUnit.test("totalCount, no arguments", function(assert) {
            var done = assert.async();

            var store = createStore({
                loadUrl: "/load",
                loadParams: { custom: 123 },
                onBeforeSend: function(op, ajax) {
                    assert.equal(op, "load");
                    assert.deepEqual(ajax.data, { custom: 123, isCountQuery: true });
                    done();
                }
            });

            store.totalCount();
        });

        QUnit.test("byKey, single key", function(assert) {
            var done = assert.async();

            var store = createStore({
                key: "id",
                loadUrl: "/load",
                loadParams: { custom: 123 },
                onBeforeSend: function(op, ajax) {
                    assert.equal(op, "load");
                    assert.deepEqual(ajax.data, { custom: 123, filter: '["id",123]' });
                    done();
                }
            });

            store.byKey(123);
        });

        QUnit.test("byKey, compound key", function(assert) {
            var done = assert.async();

            var store = createStore({
                key: ["k1", "k2"],
                loadUrl: "/load",
                loadParams: { custom: 123 },
                onBeforeSend: function(op, ajax) {
                    assert.equal(op, "load");
                    assert.deepEqual(ajax.data, { custom: 123, filter: '[["k1",1],["k2",2]]' });
                    done();
                }
            });
            store.byKey({ k1: 1, k2: 2 });
        });

        QUnit.test("insert", function(assert) {
            var done = assert.async();

            var store = createStore({
                key: "id",
                insertUrl: "/insert",
                onBeforeSend: function(op, ajax) {
                    assert.equal(op, "insert");
                    assert.equal(ajax.method, "POST");
                    assert.deepEqual(ajax.data, {
                        values: '{"a":1}'
                    });
                    done();
                }
            });
            store.insert({ a: 1 });
        });

        QUnit.test("update", function(assert) {
            var done = assert.async();

            var store = createStore({
                key: "id",
                updateUrl: "/update",
                onBeforeSend: function(op, ajax) {
                    assert.equal(op, "update");
                    assert.equal(ajax.method, "PUT");
                    assert.deepEqual(ajax.data, {
                        key: 123,
                        values: '{"a":1}'
                    });
                    done();
                }
            });
            store.update(123, { a: 1 });
        });

        QUnit.test("delete", function(assert) {
            var done = assert.async();

            var store = createStore({
                key: "id",
                deleteUrl: "/delete",
                onBeforeSend: function(op, ajax) {
                    assert.equal(op, "delete");
                    assert.equal(ajax.method, "DELETE");
                    assert.deepEqual(ajax.data, {
                        key: 123,
                    });
                    done();
                }
            });
            store.remove(123);
        });
    });

    QUnit.module("cache-busting", function() {

        function testCacheBusting(testName, action) {

            QUnit.test(testName, function(assert) {
                var done = assert.async();

                XHRMock.use(function(req) {
                    assert.ok("_" in req.url().query);
                    done();
                    return NEVER_RESOLVE;
                });

                action(createStore({
                    loadUrl: "/",
                    key: "id"
                }));
            });

        }

        testCacheBusting("load", function(store) { store.load(); });
        testCacheBusting("totalCount", function(store) { store.totalCount(); });
        testCacheBusting("byKey", function(store) { store.byKey(123); });
    });

    QUnit.test("T538073", function(assert) {
        var done = assert.async();

        assert.expect(0);

        XHRMock.use(function(req, res) {
            return res.status(204);
        });

        var store = createStore({
            key: "id",
            insertUrl: "/",
            updateUrl: "/",
            deleteUrl: "/"
        });

        Promise.all([
            store.update(123, {}),
            store.insert(123, {}),
            store.remove(123)
        ]).then(done);
    });

    QUnit.test("DevExtreme#2770", function(assert) {
        assert.expect(0);
        if(document.documentMode < 10)
            return;

        var done = assert.async();

        XHRMock.use(function(req, res) {
            return res.status(200).body("[]");
        });

        createStore({ loadUrl: "http://cross-domain.example.net" }).load().done(function() {
            done();
        });
    });

    QUnit.test("custom HTTP methods", function(assert) {
        var done = assert.async();
        var actualMethods = [];

        function notifyRequest() {
            if(actualMethods.length > 5) {
                actualMethods.sort();
                assert.deepEqual(actualMethods, [
                    "CUSTOM_DELETE",
                    "CUSTOM_INSERT",
                    "CUSTOM_LOAD", "CUSTOM_LOAD", "CUSTOM_LOAD",
                    "CUSTOM_UPDATE"
                ]);
                done();
            }
        }

        XHRMock.use(function(req) {
            actualMethods.push(req.method());
            notifyRequest();
            return NEVER_RESOLVE;
        });

        var options = { key: "any" };
        [ "load", "insert", "update", "delete" ].forEach(function(op) {
            options[op + "Url"] = "/";
            options[op + "Method"] = "custom_" + op;
        });

        var store = createStore(options);
        store.load();
        store.totalCount();
        store.byKey(123);
        store.insert({});
        store.update(123, {});
        store.remove(123);
    });

    QUnit.test("insert responds with text", function(assert) {
        var done = assert.async();

        XHRMock.use(function(req, res) {
            return res
                .header("Content-Type", "text/plain")
                .status(200)
                .body("insert-response");
        });

        createStore({ key: "any", insertUrl: "/" })
            .insert({ })
            .done(useLegacyStoreResult()
                ? function(values, key) {
                    assert.equal(key, "insert-response");
                    done();
                }
                : function(responseData, key) {
                    assert.equal(responseData, "insert-response");
                    assert.equal(key, undefined);
                    done();
                }
            );
    });

    QUnit.test("insert responds with JSON", function(assert) {
        var done = assert.async();

        willRespondWithJson({ id: 123 });

        createStore({ key: "id", insertUrl: "/" })
            .insert({ })
            .done(useLegacyStoreResult()
                ? function(values, key) {
                    assert.deepEqual(key, { id: 123 });
                    done();
                }
                : function(responseData, key) {
                    assert.deepEqual(responseData, { id: 123 });
                    assert.equal(key, 123);
                    done();
                }
            );
    });

    if(!useLegacyStoreResult()) {
        QUnit.test("update's promise arguments", function(assert) {
            var done = assert.async();

            willRespondWithJson({ id: 123 });

            createStore({ key: "id", updateUrl: "/" })
                .update(123, { })
                .done(function(responseData, key) {
                    assert.deepEqual(responseData, { id: 123 });
                    assert.equal(key, 123);
                    done();
                });
        });
    }

    QUnit.test("insert, update, delete accept empty response", function(assert) {
        var done = assert.async();

        XHRMock.use(function(req, res) {
            return res.status(200).body("");
        });

        var store = createStore({
            key: "any",
            insertUrl: "/",
            updateUrl: "/",
            deleteUrl: "/"
        });

        assert.expect(0);

        Promise.all([
            store.insert({}),
            store.update(123, { }),
            store.remove(123)
        ]).then(done);
    });

    QUnit.test("store events", function(assert) {
        var done = assert.async();

        var eventNames = [ "onLoading", "onLoaded", "onInserting", "onInserted", "onUpdating", "onUpdated", "onRemoving", "onRemoved", "onModifying", "onModified" ];
        var trace = { };

        var options = {
            key: "any",
            loadUrl: "/",
            insertUrl: "/",
            updateUrl: "/",
            deleteUrl: "/"
        };

        eventNames.forEach(function(name) {
            options[name] = function() {
                trace[name] = true;
            };
        });

        var store = createStore(options);

        willRespondWithJson({ });

        Promise.all([
            store.load(),
            store.insert({ }),
            store.update(123, { }),
            store.remove(123)
        ]).then(function() {
            assert.equal(Object.keys(trace).length, eventNames.length);
            done();
        })
    });

    QUnit.test("onPush", function(assert) {
        var done = assert.async();

        assert.expect(0);

        var store = createStore({
            onPush: function() {
                done();
            }
        });

        if("push" in store) {
            store.push([
                { type: "insert", data: { } }]
            );
        } else {
            done();
        }
    });

    if(devextremeVersion[0] >= 19) {
        QUnit.test("ajax.inject", function(assert) {
            var done = assert.async();

            function customSendRequest() {
                ajaxUtility.resetInjection();
                assert.expect(0);
                done();
                return NEVER_RESOLVE;
            }

            ajaxUtility.inject({ sendRequest: customSendRequest });
            createStore({ loadUrl: "/"}).load();
        })
    }
});
