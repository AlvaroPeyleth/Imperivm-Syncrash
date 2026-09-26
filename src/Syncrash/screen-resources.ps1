# Builds a deterministic, reviewed resource set. No downloads or recursive bundling.
param(
    [Parameter(Mandatory=$true)][string]$BundleDirectory,
    [Parameter(Mandatory=$true)][string]$OutputDirectory
)
$ErrorActionPreference = 'Stop'
$bundle = [IO.Path]::GetFullPath($BundleDirectory)
$output = [IO.Path]::GetFullPath($OutputDirectory)
if (Test-Path -LiteralPath $output) { throw 'Screen resource output already exists.' }
$specs = [regex]::Matches([IO.File]::ReadAllText((Join-Path $PSScriptRoot 'ScreenCompatibility.cs')), 'new ScreenFile\("([^"\\/]+)", "([0-9a-f]{64})"\)')
if ($specs.Count -ne 4) { throw 'Review the screen component list.' }
$inputs = @{}
foreach ($spec in $specs) { $inputs[$spec.Groups[1].Value] = $spec.Groups[2].Value }
# Pins refer to the reviewed local source bundle and its native binaries.
$inputs['PROVENANCE.txt'] = '1095b341d5465642cc88fd4c3959b3fb4100953a2f8f1609910521af9bb868ad'
$inputs['Sources/dxwnd-2.06.15-source.rar'] = 'c0f7632332c5389a1876b0c561286b82594729c192d72e5b9d815f1828f70d38'
$inputs['Sources/proxy-source.zip'] = '9ff1a14853c56f5d97102486e844e5e7184b9077b857c10f0e715a990d60f2d8'
$inputs['Sources/syncrash-dxwnd-changes.zip'] = 'a513116c3d82dc6a4d857b097f8ee23b10df3b341674c8cdd815c1e9620b6a26'
$smooth = [regex]::Match([IO.File]::ReadAllText((Join-Path $PSScriptRoot 'ScreenCompatibility.cs')), 'SmoothProfileHash = "([0-9a-f]{64})"')
if (-not $smooth.Success) { throw 'Missing smooth profile hash.' }
$inputs['dxwnd-smooth.dxw'] = $smooth.Groups[1].Value
# Verify the bytes being embedded, without reading each source a second time.
$data = @{}
foreach ($name in $inputs.Keys) {
    $bytes = [IO.File]::ReadAllBytes((Join-Path $bundle $name))
    $sha = [Security.Cryptography.SHA256]::Create()
    try { $hash = ([BitConverter]::ToString($sha.ComputeHash($bytes))).Replace('-','').ToLowerInvariant() }
    finally { $sha.Dispose() }
    if ($hash -ne $inputs[$name]) { throw "Unreviewed screen input: $name" }
    $data[$name] = $bytes
}
New-Item -ItemType Directory -Path $output | Out-Null
$resources = @()
foreach ($spec in $specs) {
    $name = $spec.Groups[1].Value
    $path = Join-Path $output $name
    [IO.File]::WriteAllBytes($path, $data[$name])
    $resources += "/resource:$path,Syncrash.Screen.$name"
}
$smoothPath = Join-Path $output 'dxwnd-smooth.dxw'
[IO.File]::WriteAllBytes($smoothPath, $data['dxwnd-smooth.dxw'])
$resources += "/resource:$smoothPath,Syncrash.Screen.SmoothProfile"
Add-Type -AssemblyName System.IO.Compression
$zipPath = Join-Path $output 'screen-sources.zip'
$zipStream = [IO.File]::Open($zipPath, [IO.FileMode]::CreateNew)
try {
    $archive = New-Object IO.Compression.ZipArchive($zipStream, [IO.Compression.ZipArchiveMode]::Create, $true)
    try {
        foreach ($name in @('PROVENANCE.txt','Sources/dxwnd-2.06.15-source.rar','Sources/proxy-source.zip','Sources/syncrash-dxwnd-changes.zip','Syncrash-screen-LICENSE.txt')) {
            $entry = $archive.CreateEntry($name, [IO.Compression.CompressionLevel]::NoCompression)
            $entry.LastWriteTime = [DateTimeOffset]::new(1980,1,1,0,0,0,[TimeSpan]::Zero)
            $stream = $entry.Open()
            try { $stream.Write($data[$name], 0, $data[$name].Length) } finally { $stream.Dispose() }
        }
    } finally { $archive.Dispose() }
} finally { $zipStream.Dispose() }
$hashPath = Join-Path $output 'screen-sources.sha256'
[IO.File]::WriteAllText($hashPath, (Get-FileHash -LiteralPath $zipPath -Algorithm SHA256).Hash.ToLowerInvariant(), [Text.Encoding]::ASCII)
$resources += "/resource:$zipPath,Syncrash.ScreenSources"
$resources += "/resource:$hashPath,Syncrash.ScreenSourcesHash"
$resources
