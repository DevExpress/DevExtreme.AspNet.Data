var files = require("./test-deps.js");
files.push("test.js"); 

module.exports = function(config) {
  config.set({
    basePath: "",
    frameworks: ["qunit"],
    files: files,
    preprocessors: {
        "../js/*.js": "coverage"
    },
    reporters: ["dots", "coverage"],
    coverageReporter: {
      reporters: [
        { type: "html" },
        { type: "text-summary" },
      ]
    },    
    browsers: ["PhantomJS"],
    singleRun: true
  })
}
