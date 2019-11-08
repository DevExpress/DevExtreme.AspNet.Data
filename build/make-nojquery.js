"use strict";

const fs = require("fs");
const path = require("path");

const README = `# DevExtreme ASP.NET Data

jQuery-free version of [devextreme-aspnet-data](https://www.npmjs.com/package/devextreme-aspnet-data).
`;

const AJAX_SETTINGS_SURROGATE = `{
        cache?: boolean;
        contentType?: any;
        data?: any;
        dataType?: string;
        headers?: { [key: string]: any; };
        method?: string;
        password?: string;
        timeout?: number;
        url?: string;
        username?: string;
        xhrFields?: { [key: string]: any; };
    }`;

const outputPath = path.join(__dirname, "..", "js-nojquery");
if(!fs.existsSync(outputPath))
    fs.mkdirSync(outputPath);

const originalPackageJSON = require("../package.json");
const nojqueryPackageJSON = { };
[ "name", "title", "version", "description", "homepage", "license", "author", "repository", "dependencies", "peerDependencies" ]
    .forEach(i => nojqueryPackageJSON[i] = originalPackageJSON[i]);
nojqueryPackageJSON.name += "-nojquery";
fs.writeFileSync(path.join(outputPath, "package.json"), JSON.stringify(nojqueryPackageJSON, null, "  "));

fs.writeFileSync(
    path.join(outputPath, "index.js"),
    fs.readFileSync(path.join(__dirname, "../js/dx.aspnet.data.js"), "utf-8")
        .replace(/\brequire\("jquery"\).Deferred/g, `require("devextreme/core/utils/deferred").Deferred`)
        .replace(/\brequire\("jquery"\).extend/g, `require("devextreme/core/utils/extend").extend`)
        .replace(/DevExpress\.data\.AspNet = [^]+?\)/, `throw "This script should be used with an AMD or CommonJS loader"`)
        .replace(/\/\* global .+?\*\//, "/* global define, module, require */")
);

fs.writeFileSync(
    path.join(outputPath, "index.d.ts"),
    fs.readFileSync(path.join(__dirname, "../js/dx.aspnet.data.d.ts"), "utf-8")
        .replace("JQueryAjaxSettings", AJAX_SETTINGS_SURROGATE)
        .replace("JQueryXHR", "XMLHttpRequest")
);

fs.writeFileSync(
    path.join(__dirname, "../js-test/check-ts-compilation.nojquery.ts"),
    fs.readFileSync(path.join(__dirname, "../js-test/check-ts-compilation.ts"), "utf-8")
        .replace("../js/dx.aspnet.data", "../js-nojquery/index")
);

fs.writeFileSync(path.join(outputPath, "README.md"), README);

fs.copyFileSync(
    path.join(__dirname, "../LICENSE"),
    path.join(outputPath, "LICENSE")
);
