module.exports = function(config) {
    var options = {
        basePath: "",
        frameworks: [ "qunit" ],
        reporters: [ "dots" ],
        browsers: [ "PhantomJS" ],
        singleRun: true
    };

    if(config.bundled) {
        options.files = [ "test-bundle.js" ];
    } else {
        options.files = require("./test-deps.js");
        options.files.push("test.js");
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
