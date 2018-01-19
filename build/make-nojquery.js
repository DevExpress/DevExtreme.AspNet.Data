// jshint esversion:6, node:true

const fs = require("fs");
const path = require("path");

const JQ_SURROGATE_AMD = `{
                    ajax: require("devextreme/core/utils/ajax").sendRequest,
                    Deferred: require("devextreme/core/utils/deferred").Deferred,
                    extend: require("devextreme/core/utils/extend").extend
                }`;

const JQ_SURROGATE_GLOBAL = `throw "This script should be used with an AMD loader"`; // TODO check this message

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

const scriptText = fs.readFileSync(path.join(__dirname, "../js/dx.aspnet.data.js"), "utf-8");
const dtsText = fs.readFileSync(path.join(__dirname, "../js/dx.aspnet.data.d.ts"), "utf-8");
const sampleText = fs.readFileSync(path.join(__dirname, "../js-test/check-ts-compilation.ts"), "utf-8");

fs.writeFileSync(
    path.join(__dirname, "../js/dx.aspnet.data.nojquery.js"),
    scriptText
        .replace("require(\"jquery\")", JQ_SURROGATE_AMD)
        .replace(/DevExpress\.data\.AspNet = [^]+?\)/, JQ_SURROGATE_GLOBAL)
);

fs.writeFileSync(
    path.join(__dirname, "../js/dx.aspnet.data.nojquery.d.ts"),
    dtsText.replace("JQueryAjaxSettings", AJAX_SETTINGS_SURROGATE)
);

fs.writeFileSync(
    path.join(__dirname, "../js-test/check-ts-compilation.nojquery.ts"),
    sampleText.replace("dx.aspnet.data", "dx.aspnet.data.nojquery")
);
