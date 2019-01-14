"use strict";

const fs = require("fs");
const path = require("path");

const MAIN_REPO_NAME = "DevExpress/DevExtreme.AspNet.Data";

const META_COMPANY = "Developer Express Inc.";
const META_COPYRIGHT = "Copyright (c) " + META_COMPANY;
const META_DESCRIPTION = "DevExtreme data layer extension for ASP.NET";
const META_PROJECT_URL = "https://github.com/" + MAIN_REPO_NAME;

const BUILD_NUMBER = process.argv[2];
const TAG = process.argv[3];
const REPO_NAME = process.argv[4];

let META_VERSION_NUMERIC = "99.0.0";
let META_VERSION_FULL;

if(REPO_NAME === MAIN_REPO_NAME && /^v?(([.\d]+)[\w-]*)$/.test(TAG)) {
    META_VERSION_FULL = RegExp.$1;
    META_VERSION_NUMERIC = RegExp.$2;
} else if(BUILD_NUMBER) {
    META_VERSION_FULL = META_VERSION_NUMERIC + "-ci-";

    if(REPO_NAME && REPO_NAME !== MAIN_REPO_NAME)
        META_VERSION_FULL += REPO_NAME.split("/")[0].toLowerCase() + "-";

    for(let i = 0; i < 6 - BUILD_NUMBER.length; i++)
        META_VERSION_FULL += "0";

    META_VERSION_FULL += BUILD_NUMBER;
} else {
    META_VERSION_FULL = META_VERSION_NUMERIC;
}

[
    "../net/DevExtreme.AspNet.Data/DevExtreme.AspNet.Data.csproj",
    "../package.json",
    "../js-nojquery/package.json"
].forEach(filePath => {
    const fullFilePath = path.join(__dirname, filePath);

    fs.writeFileSync(
        fullFilePath,
        fs.readFileSync(fullFilePath, "utf-8")
            .replace(/(<AssemblyVersion>)[^<]+/g, "$1" + META_VERSION_NUMERIC)
            .replace(/(<Version>)[^<]+/g, "$1" + META_VERSION_FULL)
            .replace(/("version":.+?")[^"]+/g, "$1" + META_VERSION_FULL)
            .replace("%meta_company%", META_COMPANY)
            .replace("%meta_copyright%", META_COPYRIGHT)
            .replace("%meta_description%", META_DESCRIPTION)
            .replace("%meta_project_url%", META_PROJECT_URL)
    );
});

[
    "../js/dx.aspnet.data.js",
    "../js-nojquery/index.js"
].forEach(filePath => {
    const fullFilePath = path.join(__dirname, filePath);

    fs.writeFileSync(
        fullFilePath,
        "// Version: " + META_VERSION_FULL + "\r\n" + fs.readFileSync(fullFilePath, "utf-8")
    );
});
