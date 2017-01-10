var path = require("path");

module.exports = {
    context: __dirname,
    resolve: {
        alias: {
            "jquery": path.join(__dirname, "node_modules/jquery/dist/jquery.min.js"),
            "mockjax": path.join(__dirname, "node_modules/jquery-mockjax/dist/jquery.mockjax.min.js"),
            "devextreme": path.join(__dirname, "node_modules/devextreme")
        }
    },
    entry: "./test",
    output: {
        path: __dirname,
        filename: "./test-bundle.js"
    }
};
