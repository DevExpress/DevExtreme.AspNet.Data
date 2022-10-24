/* eslint-env node, es6 */
"use strict";

const DefinePlugin = require("webpack").DefinePlugin;

module.exports = function(env) {
    const definitions = {
        ASPNET_DATA_SCRIPT: JSON.stringify(Boolean(env.NOJQUERY)
            ? "../js-nojquery/index.js"
            : "../js/dx.aspnet.data.js"
        )
    };

    if(Boolean(env.CJS)) {
        definitions["define"] = false;
    }

    return {
        mode: "development",
        entry: "./test.js",
        output: {
            path: __dirname,
            filename: "test-bundle.js"
        },
        plugins: [
            new DefinePlugin(definitions)
        ],
        devtool: false
    }
};
