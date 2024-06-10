var dxVersion = require('devextreme/package.json').version;

var TEST_DEPS = [
    "../node_modules/jquery/dist/jquery.min.js",
    (dxVersion >= '23.1.0'
        ? "../node_modules/devextreme-dist/js/dx.all.js"
        : "../node_modules/devextreme/dist/js/dx.all.js"),
    "../node_modules/xhr-mock/dist/xhr-mock.js",
    "../js/dx.aspnet.data.js",
];

if(typeof module !== "undefined")
    module.exports = TEST_DEPS;
