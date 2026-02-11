param(
    [string]$ReplaysDir = "C:\data\ds\testreplays",
    [string]$DotNetExe = "C:\Users\pax77\source\repos\s2protocol.NET\src\s2cli\bin\Release\net10.0\s2cli.exe",
    [string]$PythonExe = "C:\Python27\Scripts\s2_cli.exe"
)

$ErrorActionPreference = "Stop"

function TimeCommand {
    param (
        [string]$Command,
        [string[]]$Arguments
    )
    $start = Get-Date
    & $Command @Arguments | Out-Null
    $elapsed = (Get-Date) - $start
    return $elapsed.TotalMilliseconds
}

$replayFiles = Get-ChildItem -Path $ReplaysDir -Filter *.SC2Replay
if ($replayFiles.Count -eq 0) {
    Write-Host "❌ No replays found in $ReplaysDir" -ForegroundColor Red
    exit 1
}

[int]$successDotNet = 0
[int]$successPython = 0
[double[]]$dotnetTimes = @()
[double[]]$pythonTimes = @()

Write-Host "▶ Benchmarking $($replayFiles.Count) replays..." -ForegroundColor Cyan

foreach ($replay in $replayFiles) {
    Write-Host "🔍 $($replay.Name)"

    # C# Decoder
    try {
        $time = TimeCommand -Command $DotNetExe -Arguments @("--replay", $replay.FullName, "--all")
        $dotnetTimes += $time
        $successDotNet++
        Write-Host "  ✅ .NET: ${time}ms"
    }
    catch {
        Write-Host "  ❌ .NET failed: $_" -ForegroundColor Red
    }

    # Python Decoder
    try {
        $time = TimeCommand -Command $PythonExe -Arguments @($replay.FullName, "--all")
        $pythonTimes += $time
        $successPython++
        Write-Host "  ✅ Python: ${time}ms"
    }
    catch {
        Write-Host "  ❌ Python failed: $_" -ForegroundColor Red
    }
}

function Get-Stats {
    param($label, $times)
    if ($times.Count -eq 0) {
        Write-Host "No timings for $label"
        return
    }
    $min = ($times | Measure-Object -Minimum).Minimum
    $max = ($times | Measure-Object -Maximum).Maximum
    $avg = ($times | Measure-Object -Average).Average
    Write-Host "`n📈 $label stats:"
    Write-Host "  Count: $($times.Count)"
    Write-Host ("  Avg:   {0:N2} ms" -f $avg)
    Write-Host ("  Min:   {0:N2} ms" -f $min)
    Write-Host ("  Max:   {0:N2} ms" -f $max)
}

Write-Host "`n🎉 Benchmark complete!"
Write-Host "✅ .NET succeeded:    $successDotNet / $($replayFiles.Count)"
Write-Host "✅ Python succeeded:  $successPython / $($replayFiles.Count)"

Get-Stats "C# .NET" $dotnetTimes
Get-Stats "Python" $pythonTimes
