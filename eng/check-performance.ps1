param(
    [string] $BaselinePath = (Join-Path $PSScriptRoot 'performance-baseline.json'),
    [string] $ResultsPath = (Join-Path $PSScriptRoot '..' 'BenchmarkDotNet.Artifacts' 'results'),
    [string] $SummaryPath
)

$ErrorActionPreference = 'Stop'

function Convert-TimeToNanoseconds([string] $value) {
    if ($value -notmatch '^\s*([\d,.]+)\s*(ns|[μu]s|ms|s)\s*$') {
        throw "Unsupported elapsed-time value '$value'."
    }

    $number = [double]::Parse($Matches[1].Replace(',', ''), [Globalization.CultureInfo]::InvariantCulture)
    $scale = switch ($Matches[2]) {
        'ns' { 1 }
        'μs' { 1e3 }
        'us' { 1e3 }
        'ms' { 1e6 }
        's'  { 1e9 }
    }

    return $number * $scale
}

function Convert-SizeToBytes([string] $value) {
    if ($value -notmatch '^\s*([\d,.]+)\s*(B|KB|MB|GB)\s*$') {
        throw "Unsupported allocation value '$value'."
    }

    $number = [double]::Parse($Matches[1].Replace(',', ''), [Globalization.CultureInfo]::InvariantCulture)
    $scale = switch ($Matches[2]) {
        'B'  { 1 }
        'KB' { 1KB }
        'MB' { 1MB }
        'GB' { 1GB }
    }

    return $number * $scale
}

function Find-ScenarioRow($scenario, [object[]] $rows) {
    $matches = @($rows | Where-Object {
        if ($_.Method -ne $scenario.method) {
            return $false
        }

        foreach ($parameter in $scenario.parameters.PSObject.Properties) {
            if ($_.$($parameter.Name) -ne [string] $parameter.Value) {
                return $false
            }
        }

        return $true
    })

    if ($matches.Count -ne 1) {
        throw "Expected one result for '$($scenario.name)', found $($matches.Count)."
    }

    return $matches[0]
}

$baseline = Get-Content -LiteralPath $BaselinePath -Raw | ConvertFrom-Json
$elapsedLimit = 1 + ($baseline.elapsedRegressionBudgetPercent / 100)
$allocationLimit = 1 + ($baseline.allocatedBytesRegressionBudgetPercent / 100)
$reportCache = @{}
$failures = 0
$lines = [Collections.Generic.List[string]]::new()
$lines.Add('## Performance regression report')
$lines.Add('')
$lines.Add("Elapsed time and allocated bytes enforce $($baseline.elapsedRegressionBudgetPercent)% and $($baseline.allocatedBytesRegressionBudgetPercent)% regression budgets, respectively.")
$lines.Add('')
$lines.Add('| Scenario | Mean | Elapsed change | Allocated | Allocation change | Status |')
$lines.Add('| --- | ---: | ---: | ---: | ---: | :---: |')

foreach ($scenario in $baseline.scenarios) {
    if (-not $reportCache.ContainsKey($scenario.report)) {
        $reportPath = Join-Path $ResultsPath $scenario.report
        if (-not (Test-Path -LiteralPath $reportPath)) {
            throw "Benchmark report '$reportPath' was not found."
        }

        $reportCache[$scenario.report] = @(Import-Csv -LiteralPath $reportPath)
    }

    $row = Find-ScenarioRow $scenario $reportCache[$scenario.report]
    $meanNanoseconds = Convert-TimeToNanoseconds $row.Mean
    $allocatedBytes = Convert-SizeToBytes $row.Allocated
    $elapsedRatio = $meanNanoseconds / $scenario.baselineMeanNanoseconds
    $allocationRatio = $allocatedBytes / $scenario.baselineAllocatedBytes
    $elapsedStatus = if ($elapsedRatio -le $elapsedLimit) { 'within' } else { 'over' }
    $allocationStatus = if ($allocationRatio -le $allocationLimit) { 'within' } else { 'over' }
    if ($elapsedRatio -gt $elapsedLimit -or $allocationRatio -gt $allocationLimit) {
        $failures++
    }

    $status = if ($elapsedRatio -le $elapsedLimit -and $allocationRatio -le $allocationLimit) { '✅' } else { '❌' }
    $meanText = ($meanNanoseconds / 1e6).ToString('N3', [Globalization.CultureInfo]::InvariantCulture)
    $elapsedChangeText = ($elapsedRatio - 1).ToString(
        '+0.0%;-0.0%;0.0%', [Globalization.CultureInfo]::InvariantCulture)
    $allocatedText = ($allocatedBytes / 1MB).ToString('N2', [Globalization.CultureInfo]::InvariantCulture)
    $allocationChangeText = ($allocationRatio - 1).ToString(
        '+0.0%;-0.0%;0.0%', [Globalization.CultureInfo]::InvariantCulture)
    $lines.Add("| $($scenario.name) | $meanText ms | $elapsedChangeText ($elapsedStatus) | " +
        "$allocatedText MB | $allocationChangeText ($allocationStatus) | $status |")
}

$report = $lines -join [Environment]::NewLine
Write-Output $report
if ($SummaryPath) {
    $report | Out-File -LiteralPath $SummaryPath -Encoding utf8 -Append
}

if ($failures -gt 0) {
    throw "$failures scenario(s) exceeded a performance regression budget."
}
