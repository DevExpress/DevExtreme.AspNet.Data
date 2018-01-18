var TEST_DEPS = [
    "../node_modules/jquery/dist/jquery.min.js",
    "../node_modules/devextreme/dist/js/dx.all.js",
    "../node_modules/xhr-mock/dist/xhr-mock.js",
    "../js/dx.aspnet.data.js",
];

if(typeof window !== "undefined" && !window.Promise)
    TEST_DEPS.push("../node_modules/bluebird/js/browser/bluebird.core.min.js");

if(typeof module !== "undefined")
    module.exports = TEST_DEPS;
