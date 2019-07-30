# Using CI Builds

Master branch build artifacts are stored [here](https://ci.appveyor.com/project/dxrobot/devextreme-aspnet-data/branch/master/artifacts). Download the `devextreme-aspnet-data-99.0.0-ci-NNN.tgz` and `DevExtreme.AspNet.Data.99.0.0-ci-NNN.nupkg` files and follow the instructions below.

## Client-Side Script

Unpack the `.tgz` archive to a temporary location. On Windows, you can use [7zip](http://www.7-zip.org/download.html) for that:

```bash
7zip x devextreme-aspnet-data-99.0.0-ci-NNN.tgz
7zip x devextreme-aspnet-data-99.0.0-ci-NNN.tar
```

Navigate to the package/js folder in the temporary location and copy the extracted JavaScript file to your project.

For Node projects, you can [install](https://docs.npmjs.com/cli/install) the archive as a package:

```bash
npm i /temp/path/devextreme-aspnet-data-99.0.0-ci-NNN.tgz
```

Alternatively, you get the JavaScript file directly [from the master branch](https://raw.githubusercontent.com/DevExpress/DevExtreme.AspNet.Data/master/js/dx.aspnet.data.js) on GitHub.

## NuGet Package

Follow [these instructions](https://stackoverflow.com/a/35753968) to install the downloaded .nupkg file.

## Add an Assembly Binding Redirect

For .NET framework projects, add a binding redirect to the `web.config` or `app.config` file:

```xml
<configuration>
  <runtime>
    <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
      <!-- add this -->
      <dependentAssembly>
        <assemblyIdentity name="DevExtreme.AspNet.Data" publicKeyToken="982f5dab1439d0f7"/>
        <bindingRedirect oldVersion="0.0.0.0-99.0.0.0" newVersion="99.0.0.0"/>
      </dependentAssembly>
```
