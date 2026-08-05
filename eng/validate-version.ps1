param([string] $Tag)

$ErrorActionPreference = 'Stop'
[xml] $props = Get-Content -LiteralPath (Join-Path $PSScriptRoot '..' 'Directory.Build.props') -Raw
$version = [string] $props.Project.PropertyGroup.Version
if ($version -ne '1.0.0') {
    throw "Expected centralized version 1.0.0, found '$version'."
}

$readme = Get-Content -LiteralPath (Join-Path $PSScriptRoot '..' 'README.md') -Raw
if ($readme -notmatch [regex]::Escape("v$version")) {
    throw "README changelog does not contain v$version."
}

if ($Tag -and $Tag -ne "v$version") {
    throw "Release tag '$Tag' must equal 'v$version'."
}
Write-Output "Validated version $version$(if ($Tag) { " and tag $Tag" })."
