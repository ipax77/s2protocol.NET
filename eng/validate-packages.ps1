param(
    [string] $ArtifactsPath = (Join-Path $PSScriptRoot '..' 'artifacts'),
    [string] $Version = '1.0.0'
)

$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.IO.Compression.FileSystem

function Assert-True([bool] $Condition, [string] $Message) {
    if (-not $Condition) { throw $Message }
}

function Read-Package([string] $Path) {
    $archive = [IO.Compression.ZipFile]::OpenRead($Path)
    try {
        $entries = @($archive.Entries | ForEach-Object FullName)
        $nuspecEntry = $archive.Entries | Where-Object FullName -Like '*.nuspec' | Select-Object -First 1
        Assert-True ($null -ne $nuspecEntry) "Package '$Path' has no nuspec."
        $reader = [IO.StreamReader]::new($nuspecEntry.Open())
        try { [xml] $nuspec = $reader.ReadToEnd() } finally { $reader.Dispose() }
        return @{ Entries = $entries; Nuspec = $nuspec }
    }
    finally { $archive.Dispose() }
}

foreach ($id in @('s2protocol.NET', 's2cli')) {
    $packagePath = Join-Path $ArtifactsPath "$id.$Version.nupkg"
    $symbolsPath = Join-Path $ArtifactsPath "$id.$Version.snupkg"
    Assert-True (Test-Path -LiteralPath $packagePath) "Missing package '$packagePath'."
    Assert-True (Test-Path -LiteralPath $symbolsPath) "Missing symbols '$symbolsPath'."

    $package = Read-Package $packagePath
    $metadata = $package.Nuspec.package.metadata
    Assert-True ($metadata.id -eq $id) "Unexpected package ID '$($metadata.id)'."
    Assert-True ($metadata.version -eq $Version) "Unexpected $id version '$($metadata.version)'."
    Assert-True ($metadata.license.InnerText -eq 'MIT') "$id must use the MIT license expression."
    Assert-True ($metadata.readme -eq 'README.md') "$id must declare README.md."
    Assert-True ($metadata.repository.url -eq 'https://github.com/ipax77/s2protocol.NET') "$id repository URL is invalid."
    Assert-True (-not [string]::IsNullOrWhiteSpace($metadata.repository.commit)) "$id has no repository commit metadata."
    Assert-True ($package.Entries -contains 'README.md') "$id package is missing README.md."
    Assert-True ($package.Entries -contains 'LICENSE') "$id package is missing LICENSE."
    Assert-True (-not ($package.Entries -match '\.SC2Replay$|(^|/)obj/|s2protocol\.NET\.tests')) "$id contains unintended files."

    if ($id -eq 's2protocol.NET') {
        Assert-True ($package.Entries -contains 'lib/net10.0/s2protocol.NET.dll') 'Library assembly is missing.'
        Assert-True ($package.Entries -contains 'lib/net10.0/s2protocol.NET.xml') 'Library XML docs are missing.'
        Assert-True (-not ($package.Entries -match '^lib/(?!net10\.0/)')) 'Library package contains a target other than net10.0.'
    }
    else {
        Assert-True ($package.Entries -contains 'tools/net10.0/any/s2cli.dll') 'CLI tool assembly is missing.'
        Assert-True ($package.Entries -contains 'tools/net10.0/any/s2cli.xml') 'CLI XML docs are missing.'
        Assert-True (-not ($package.Entries -match '^tools/(?!net10\.0/)')) 'CLI package contains a target other than net10.0.'
    }

    $symbols = Read-Package $symbolsPath
    Assert-True (@($symbols.Entries -match '\.pdb$').Count -gt 0) "$id symbols package contains no PDB."
}

Write-Output "Validated s2protocol.NET and s2cli $Version packages."
