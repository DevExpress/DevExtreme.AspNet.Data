// jshint esversion:6, node:true

const fs = require("fs");
const path = require("path");

const README = `# DevExtreme ASP.NET Data

jQuery-free version of [devextreme-aspnet-data](https://www.npmjs.com/package/devextreme-aspnet-data).
`;

const JQ_SURROGATE_MODULAR = `$1{
$1    ajax: require("devextreme/core/utils/ajax").sendRequest,
$1    Deferred: require("devextreme/core/utils/deferred").Deferred,
$1    extend: require("devextreme/core/utils/extend").extend
$1}`;

const JQ_SURROGATE_GLOBAL = `throw "This script should be used with an AMD or CommonJS loader"`;

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
nojqueryPackageJSON.peerDependencies.devextreme = ">=17.2.6-pre-18044";
fs.writeFileSync(path.join(outputPath, "package.json"), JSON.stringify(nojqueryPackageJSON, null, "  "));

fs.writeFileSync(
    path.join(outputPath, "index.js"),
    fs.readFileSync(path.join(__dirname, "../js/dx.aspnet.data.js"), "utf-8")
        .replace(/( +)require\("jquery"\)/g, JQ_SURROGATE_MODULAR)
        .replace(/DevExpress\.data\.AspNet = [^]+?\)/, JQ_SURROGATE_GLOBAL)
        .replace(/\/\* global .+?\*\//, "/* global define */")
);

fs.writeFileSync(
    path.join(outputPath, "index.d.ts"),
    fs.readFileSync(path.join(__dirname, "../js/dx.aspnet.data.d.ts"), "utf-8")
        .replace("JQueryAjaxSettings", AJAX_SETTINGS_SURROGATE)
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
