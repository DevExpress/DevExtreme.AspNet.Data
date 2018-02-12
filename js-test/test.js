// jshint strict: true, browser: true, undef: true, unused: true, eqeqeq: true
/* global ASPNET_DATA_SCRIPT, DevExpress, define, Promise */

(function(factory) {
    "use strict";

    var QUnit = window.QUnit;

    if(typeof define === "function" && define.amd) {
        define("qunit-amd", function() {
            return QUnit; // https://github.com/karma-runner/karma-qunit/issues/57
        });

        define(function(require) {
            factory(
                require("qunit-amd"),
                require("xhr-mock").default,
                require("devextreme/data/data_source"),
                require(ASPNET_DATA_SCRIPT)
            );
        });
    } else {
        factory(
            QUnit,
            window.XHRMock,
            DevExpress.data.DataSource,
            DevExpress.data.AspNet
        );
    }

})(function(QUnit, XHRMock, DataSource, AspNet) {
    "use strict";

    var createStore = AspNet.createStore;

    function willRespondWithJson(obj) {
        XHRMock.use(function(req, res) {
            return res
                .status(200)
                .header("Content-Type", "application/json")
                .body(JSON.stringify(obj));
        });
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

            XHRMock.use(function() {
                return new Promise(function() { });
            });

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
    });

    QUnit.module("load options", function() {

        QUnit.test("search becomes filter", function(assert) {
            var done = assert.async();

            XHRMock.use(function(req) {
                assert.equal(req.url().query.filter, '[["haystack","contains","needle"]]');
                done();
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
                    });

                    createStore({ loadUrl: "/" }).load({ select: rawValue });
                };
            }

            QUnit.test("as string", testCase("abc", [ "abc" ]));
            QUnit.test("as array", testCase([ "abc" ], [ "abc" ]));
        });

        QUnit.test("undefined values", function(assert) {
            var done = assert.async();

            var loadOptionNames = [
                "skip", "take",
                "requireTotalCount", "requireGroupCount",
                "sort", "group", "filter",
                "totalSummary", "groupSummary"
            ];

            XHRMock.use(function(req) {
                var query = req.url().query;
                loadOptionNames.forEach(function(i) {
                    assert.ok(!(i in query));
                });
                done();
            });

            var loadOptions = { };
            loadOptionNames.forEach(function(i) {
                loadOptions[i] = undefined;
            });

            createStore({ loadUrl: "/" }).load(loadOptions);
        });

        QUnit.test("zero and false", function(assert) {
            var done = assert.async();

            XHRMock.use(function(req) {
                var query = req.url().query;
                assert.equal(query.skip, "0");
                assert.equal(query.take, "0");
                assert.equal(query.requireTotalCount, "false");
                assert.equal(query.requireGroupCount, "false");
                done();
            });

            createStore({ loadUrl: "/" }).load({
                skip: 0,
                take: 0,
                requireTotalCount: false,
                requireGroupCount: false
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

    });

    QUnit.module("check request data onBeforeSend", function() {

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

        createStore({ loadUrl: "http://cross-domain.example.net" }).load().done(function(res) {
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

        XHRMock.use(function(req, res) {
            actualMethods.push(req.method());
            notifyRequest();
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
                .body("key-text");
        });

        createStore({ key: "any", insertUrl: "/" })
            .insert({ })
            .done(function(values, key) {
                assert.equal(key, "key-text");
                done();
            });
    });

    QUnit.test("insert responds with JSON", function(assert) {
        var done = assert.async();

        willRespondWithJson({ keyProp: "keyValue" });

        createStore({ key: "any", insertUrl: "/" })
            .insert({ })
            .done(function(values, key) {
                assert.deepEqual(key, { keyProp: "keyValue" });
                done();
            });
    });

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
});
