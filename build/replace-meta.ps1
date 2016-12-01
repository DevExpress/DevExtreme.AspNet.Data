param ([string]$build_number, [string]$tag)

$meta_version_numeric = "0.0"

if ($tag -match '^v?(([.\d]+)[\w-]*)$') {
    $meta_version_full = $matches[1]
    $meta_version_numeric = $matches[2]
} elseif ($build_number) {
    $meta_version_full = "$meta_version_numeric-ci-build$build_number"
} else {
    $meta_version_full = $meta_version_numeric
}

$meta_company = "Developer Express Inc."
$meta_copyright = "Copyright (c) $meta_company"
$meta_description = "DevExtreme data layer extension for ASP.NET"
$meta_license_url = "https://raw.githubusercontent.com/DevExpress/DevExtreme.AspNet.Data/master/LICENSE"
$meta_project_url = "https://github.com/DevExpress/DevExtreme.AspNet.Data"

$targets = ("..\net\DevExtreme.AspNet.Data\AssemblyInfo.cs", "..\net\DevExtreme.AspNet.Data\project.json")

$targets | %{
    $path = "$PSScriptRoot\$_"

    (Get-Content $path) | %{
        $_  -replace '(AssemblyVersion.+?")[^"]+', "`${1}$meta_version_numeric" `
            -replace '("version":.+?")[^"]+', "`${1}$meta_version_full" `
            -replace '%meta_company%', $meta_company `
            -replace '%meta_copyright%', $meta_copyright `
            -replace '%meta_description%', $meta_description `
            -replace '%meta_license_url%', $meta_license_url `
            -replace '%meta_project_url%', $meta_project_url
    } | Set-Content $path
}
