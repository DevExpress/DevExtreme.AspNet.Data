// jshint esversion:6, node:true

const fs = require("fs");
const path = require("path");

const JQ_SURROGATE_AMD = `{
                    ajax: require("devextreme/core/utils/ajax").sendRequest,
                    Deferred: require("devextreme/core/utils/deferred").Deferred,
                    extend: require("devextreme/core/utils/extend").extend
                }`;

const JQ_SURROGATE_GLOBAL = `throw "This script should be used with an AMD loader"`; // TODO check this message

const script = fs.readFileSync(path.join(__dirname, "../js/dx.aspnet.data.js"), "utf-8");

fs.writeFileSync(
    path.join(__dirname, "../js/dx.aspnet.data.nojquery.js"),
    script
        .replace("require(\"jquery\")", JQ_SURROGATE_AMD)
        .replace(/DevExpress\.data\.AspNet = [^]+?\)/, JQ_SURROGATE_GLOBAL)
);
