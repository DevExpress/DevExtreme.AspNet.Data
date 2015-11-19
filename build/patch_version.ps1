param ([string]$version)

If ($version -match '^([.\d]+)[\w-]*') {
    $fullVersion = $matches[0]
    $numericVersion = $matches[1]

    Write-Host "Patching version: $numericVersion ($fullVersion)"

    $assemblyFile = "$PSScriptRoot\..\net\DevExtreme.AspNet.Data\AssemblyInfo.cs"
    $projectFile = "$PSScriptRoot\..\net\DevExtreme.AspNet.Data\project.json"

    (Get-Content $assemblyFile) -replace '(AssemblyVersion.+?")[^"]+', "`${1}$numericVersion" | Set-Content $assemblyFile
    (Get-Content $projectFile) -replace '("version":.+?")[^"]+', "`${1}$fullVersion" | Set-Content $projectFile
}