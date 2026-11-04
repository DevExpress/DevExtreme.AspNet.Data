"use strict";

module.exports = [
    {
        ignores: [
            "docs/",
            "js-test/check-ts-compilation.*",
            "js-test/test-bundle.js",
            "net/"
        ]
    },
    {
        languageOptions: {
            ecmaVersion: 5,
            sourceType: "script",
            globals: {
                define: "readonly",
                module: "readonly",
                require: "readonly"
            }
        },
        rules: {
            "eqeqeq": [ "error", "allow-null" ],
            "no-undef": "error",
            "no-unused-expressions": "error",
            "no-unused-vars": [ "error", { "caughtErrors": "none" } ],
            "strict": [ "error", "safe" ]
        }
    },
    {
        files: [ "eslint.config.js", "build/**", "js-test/karma.conf.js", "js-test/webpack.config.js" ],
        languageOptions: {
            ecmaVersion: 6,
            sourceType: "commonjs",
            globals: {
                __dirname: "readonly",
                console: "readonly",
                process: "readonly"
            }
        }
    }
];
