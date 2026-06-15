# Using CI Builds

You can download automated build artifacts [here](https://github.com/DevExpress/DevExtreme.AspNet.Data/actions/workflows/ci.yml?query=branch%3Amaster). Please note that you need to be signed in to GitHub.

- Click the most recent successful workflow run result. 
- At the bottom of the opened page, download the `release-packages` archive.

It contains the following files:
- `net/DevExtreme.AspNet.Data/bin/Release/DevExtreme.AspNet.Data.99.0.0-ci-NNN.nupkg`
- `devextreme-aspnet-data-99.0.0-ci-NNN.tgz`
- `devextreme-aspnet-data-nojquery-99.0.0-ci-NNN.tgz`

## Client-Side Script / NPM

Unpack the `.tgz` archive to a temporary location. On Windows 10, you can use the [built-in `tar` command](https://learn.microsoft.com/en-us/virtualization/community/team-blog/2017/20171219-tar-and-curl-come-to-windows) for that:

```bash
tar xf devextreme-aspnet-data-99.0.0-ci-NNN.tgz
```

Navigate to the `package/js` folder in the temporary location and copy the extracted JavaScript file to your project.

For Node projects, you can [install](https://docs.npmjs.com/cli/install) the archive as a package:

```bash
npm i /temp/path/devextreme-aspnet-data-99.0.0-ci-NNN.tgz
```

Alternatively, you can get the JavaScript file directly [from the master branch](https://raw.githubusercontent.com/DevExpress/DevExtreme.AspNet.Data/master/js/dx.aspnet.data.js) on GitHub.

## NuGet Package

Follow [these instructions](https://stackoverflow.com/q/10240029) to install the downloaded .nupkg file.

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
