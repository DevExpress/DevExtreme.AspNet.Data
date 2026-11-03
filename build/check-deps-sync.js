"use strict";

const rootDeps = require("../package.json").devDependencies;
const sampleDeps = require("../net/Sample/package.json").dependencies;

const errors = [];

if(rootDeps["devextreme"] !== rootDeps["devextreme-dist"])
    errors.push(`devextreme ${rootDeps["devextreme"]} != devextreme-dist ${rootDeps["devextreme-dist"]} in package.json`);

[ "devextreme-dist", "jquery" ].forEach(name => {
    if(rootDeps[name] !== sampleDeps[name])
        errors.push(`${name} ${rootDeps[name]} in package.json != ${sampleDeps[name]} in net/Sample/package.json`);
});

if(errors.length) {
    errors.forEach(e => console.error(e));
    process.exit(1);
}
