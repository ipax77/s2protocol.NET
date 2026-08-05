param([string] $Solution = 's2protocol.NET.sln')

$ErrorActionPreference = 'Stop'
$output = & dotnet list $Solution package --vulnerable --include-transitive 2>&1
$output | Write-Output
if ($LASTEXITCODE -ne 0) {
    throw "NuGet vulnerability audit failed with exit code $LASTEXITCODE."
}
if (($output -join "`n") -match 'has the following vulnerable packages') {
    throw 'The dependency graph contains known vulnerable packages.'
}
