param(
    [Parameter(Mandatory = $true, Position = 0)]
    [string]$FilePath,

    [Parameter(Position = 1, ValueFromRemainingArguments = $true)]
    [string[]]$Arguments,

    [int]$Iterations = 5,

    [int]$SampleMilliseconds = 2
)

$ErrorActionPreference = "Stop"

function Get-DescendantProcessIds {
    param([int]$ProcessId)

    $children = Get-CimInstance Win32_Process -Filter "ParentProcessId=$ProcessId" -ErrorAction SilentlyContinue
    foreach ($child in $children) {
        [int]$child.ProcessId
        Get-DescendantProcessIds -ProcessId ([int]$child.ProcessId)
    }
}

$rows = for ($i = 1; $i -le $Iterations; $i++) {
    $psi = [System.Diagnostics.ProcessStartInfo]::new()
    $psi.FileName = $FilePath
    $psi.UseShellExecute = $false
    $psi.RedirectStandardOutput = $true
    $psi.RedirectStandardError = $true

    foreach ($argument in $Arguments) {
        [void]$psi.ArgumentList.Add($argument)
    }

    $stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
    $process = [System.Diagnostics.Process]::Start($psi)
    $stdoutTask = $process.StandardOutput.BaseStream.CopyToAsync([System.IO.Stream]::Null)
    $stderrTask = $process.StandardError.BaseStream.CopyToAsync([System.IO.Stream]::Null)

    $peakWorkingSet = 0L
    $peakPrivate = 0L

    while (-not $process.HasExited) {
        $processIds = @($process.Id) + @(Get-DescendantProcessIds -ProcessId $process.Id)
        $workingSet = 0L
        $privateBytes = 0L

        foreach ($processId in $processIds | Select-Object -Unique) {
            try {
                $sample = [System.Diagnostics.Process]::GetProcessById($processId)
                $sample.Refresh()
                $workingSet += $sample.WorkingSet64
                $privateBytes += $sample.PrivateMemorySize64
            }
            catch {
                # The sampled process can exit between the tree query and counter read.
            }
        }

        if ($workingSet -gt $peakWorkingSet) {
            $peakWorkingSet = $workingSet
        }

        if ($privateBytes -gt $peakPrivate) {
            $peakPrivate = $privateBytes
        }

        Start-Sleep -Milliseconds $SampleMilliseconds
    }

    $process.WaitForExit()
    $null = $stdoutTask.GetAwaiter().GetResult()
    $null = $stderrTask.GetAwaiter().GetResult()
    $stopwatch.Stop()

    [pscustomobject]@{
        Run = $i
        ExitCode = $process.ExitCode
        WallMs = [math]::Round($stopwatch.Elapsed.TotalMilliseconds, 2)
        PeakWorkingSetMB = [math]::Round($peakWorkingSet / 1MB, 2)
        PeakPrivateMB = [math]::Round($peakPrivate / 1MB, 2)
    }
}

$rows | Format-Table -AutoSize

"mean_wall_ms={0:N2}; min_wall_ms={1:N2}; max_wall_ms={2:N2}; mean_peak_ws_mb={3:N2}; min_peak_ws_mb={4:N2}; max_peak_ws_mb={5:N2}; mean_peak_private_mb={6:N2}; min_peak_private_mb={7:N2}; max_peak_private_mb={8:N2}" -f `
    (($rows | Measure-Object WallMs -Average).Average), `
    (($rows | Measure-Object WallMs -Minimum).Minimum), `
    (($rows | Measure-Object WallMs -Maximum).Maximum), `
    (($rows | Measure-Object PeakWorkingSetMB -Average).Average), `
    (($rows | Measure-Object PeakWorkingSetMB -Minimum).Minimum), `
    (($rows | Measure-Object PeakWorkingSetMB -Maximum).Maximum), `
    (($rows | Measure-Object PeakPrivateMB -Average).Average), `
    (($rows | Measure-Object PeakPrivateMB -Minimum).Minimum), `
    (($rows | Measure-Object PeakPrivateMB -Maximum).Maximum)
