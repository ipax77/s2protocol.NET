param(
    [Parameter(Mandatory)]
    [string] $ResultsPath,
    [double] $MinimumLineRate = 0.66,
    [double] $MinimumBranchRate = 0.52
)

$ErrorActionPreference = 'Stop'
$reports = @(Get-ChildItem -LiteralPath $ResultsPath -Recurse -Filter 'coverage.cobertura.xml' |
    Sort-Object LastWriteTimeUtc -Descending)
if ($reports.Count -eq 0) {
    throw "No Cobertura report was found under '$ResultsPath'."
}

[xml] $coverage = Get-Content -LiteralPath $reports[0].FullName -Raw
$lineRate = [double]::Parse($coverage.coverage.'line-rate', [Globalization.CultureInfo]::InvariantCulture)
$branchRate = [double]::Parse($coverage.coverage.'branch-rate', [Globalization.CultureInfo]::InvariantCulture)
Write-Output ("Coverage: {0:P2} line, {1:P2} branch." -f $lineRate, $branchRate)

if ($lineRate -lt $MinimumLineRate) {
    throw "Line coverage $($lineRate.ToString('P2')) is below $($MinimumLineRate.ToString('P2'))."
}
if ($branchRate -lt $MinimumBranchRate) {
    throw "Branch coverage $($branchRate.ToString('P2')) is below $($MinimumBranchRate.ToString('P2'))."
}
