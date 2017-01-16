var path = require("path");

module.exports = {
    context: __dirname,
    entry: "./test",
    output: {
        path: __dirname,
        filename: "./test-bundle.js"
    }
};
