/* eslint-env node */

"use strict";

module.exports = function(config) {
    var options = {
        basePath: "",
        frameworks: [ "qunit" ],
        reporters: [ "dots" ],
        browsers: [ "ChromeHeadless" ],
        singleRun: true
    };

    if(config.bundled) {
        options.files = [ "test-bundle.js" ];
    } else {
        options.files = require("./test-deps.js");
        options.files.push(
            "../node_modules/bluebird/js/browser/bluebird.core.min.js",
            "test.js"
        );
        options.browsers.push("IE");
        options.preprocessors = {
            "../js/*.js": "coverage"
        };
        options.reporters.push("coverage");
        options.coverageReporter = {
            reporters: [
                {
                    type: "lcovonly",
                    dir: "coverage",
                    subdir: "."
                },
                { type: "text-summary" },
            ]
        };
    }

    config.set(options);
};
