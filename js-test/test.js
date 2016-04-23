(function() {
    "use strict";

    var createStore = DevExpress.data.AspNet.createStore;

    $.extend($.mockjaxSettings, {
        contentType: "application/json",
        responseTime: 0,
        logging: false
    });

    QUnit.testDone(function() {
        $.mockjax.clear();
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

            $.mockjax({
                url: "/",
                status: 500,
                statusText: "Test Status"
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

            $.mockjax({
                url: "/",
                isTimeout: true
            });

            createStore({ loadUrl: "/" }).load().fail(function(error) {
                assert.equal(error.message, "Network connection timeout");
                done();
            });
        });

        QUnit.module("extract message", function() {
            var SAMPLE_MESSAGE = "Sample error message";

            function testCase(mime, responseText, expectedMessage) {
                return function(assert) {
                    var done = assert.async();

                    $.mockjax({
                        url: "/",
                        status: 400,
                        statusText: "Bad Request",
                        contentType: mime,
                        responseText: responseText
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

    });

    QUnit.module("load options", function() {

        QUnit.test("search becomes filter", function(assert) {
            var done = assert.async();

            $.mockjax({
                url: "/",
                response: function(settngs) {
                    assert.equal(settngs.data.filter, '[["haystack","contains","needle"]]');
                    done();
                }
            });

            var dataSource = new DevExpress.data.DataSource({
                store: createStore({ loadUrl: "/" }),
                searchExpr: "haystack",
                searchValue: "needle"
            });

            dataSource.load();
        });

        QUnit.test("dates in filter", function(assert) {
            var done = assert.async();

            $.mockjax({
                url: "/",
                response: function(settings) {
                    assert.equal(settings.data.filter, '[["a","1/1/2000"],["b","1/1/2000 02:03:04.005"]]');
                    done();
                }
            });

            createStore({ loadUrl: "/" }).load({
                filter: [["a", new Date(2000, 0, 1)], ["b", new Date(2000, 0, 1, 2, 3, 4, 5)]]
            });
        });

        QUnit.module("sort/group normalization", function() {

            function testCase(optionName, rawValue, normalizedValue) {
                return function(assert) {
                    var done = assert.async();

                    if(normalizedValue === "=")
                        normalizedValue = rawValue;

                    $.mockjax({
                        url: "/",
                        response: function(settings) {
                            assert.deepEqual(JSON.parse(settings.data[optionName]), normalizedValue);
                            done();
                        }
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

    });

})();
