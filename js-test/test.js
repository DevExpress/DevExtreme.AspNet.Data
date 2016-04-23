var createStore = DevExpress.data.AspNet.createStore;

QUnit.test("test of test", function(assert) {
    assert.ok("AspNet" in DevExpress.data);
});

QUnit.test("require key", function(assert) {
    var done = assert.async(4);
    
    var store = createStore({
        loadUrl: "any",
        insertUrl: "any",
        updateUrl: "any",
        deleteUrl: "any"
    });
    
    function assertError(error) {
        assert.ok(error.message.indexOf("Primary key") > -1);
    }
    
    store.byKey(123).fail(function(error) {
        assertError(error);
        done();
    });        
    
    store.insert({ }).fail(function(error) {
        assertError(error);
        done();
    });
    
    store.update(123, { }).fail(function(error) {
        assertError(error);
        done();
    });

    store.remove(1).fail(function(error) {
        assertError(error);
        done();
    });
});    
