param([string]$OutputDirectory = (Join-Path ([IO.Path]::GetTempPath()) ('syncrash-compare-' + [guid]::NewGuid().ToString('N'))))
$ErrorActionPreference = 'Stop'
$root = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$output = [IO.Path]::GetFullPath($OutputDirectory)
if (Test-Path -LiteralPath $output) { throw "El directorio de informe ya existe: $output" }
New-Item -ItemType Directory -Path $output | Out-Null
$first = Join-Path $output 'first/Syncrash.exe'
$second = Join-Path $output 'second/Syncrash.exe'
& (Join-Path $root 'src/Syncrash/build.ps1') -OutputPath $first | Out-Null
& (Join-Path $root 'src/Syncrash/build.ps1') -OutputPath $second | Out-Null
$a = [IO.File]::ReadAllBytes($first)
$b = [IO.File]::ReadAllBytes($second)
$difference = $null
for ($i = 0; $i -lt [Math]::Min($a.Length, $b.Length); $i++) {
    if ($a[$i] -ne $b[$i]) { $difference = $i; break }
}
if ($null -eq $difference -and $a.Length -ne $b.Length) { $difference = [Math]::Min($a.Length, $b.Length) }
$safeRoot = $root.Replace('\', '/')
$commit = & git -c "safe.directory=$safeRoot" -C $root rev-parse HEAD
if ($LASTEXITCODE -ne 0) { throw 'No se pudo identificar el commit.' }
$dirty = [bool](& git -c "safe.directory=$safeRoot" -C $root status --porcelain)
if ($LASTEXITCODE -ne 0) { throw 'No se pudo revisar el árbol de trabajo.' }
$report = [ordered]@{
    checked_at_utc = [DateTime]::UtcNow.ToString('o')
    source_commit = $commit.Trim()
    working_tree_clean = -not $dirty
    first = [ordered]@{ file = 'first/Syncrash.exe'; bytes = $a.Length; sha256 = (Get-FileHash -Algorithm SHA256 -LiteralPath $first).Hash.ToLowerInvariant() }
    second = [ordered]@{ file = 'second/Syncrash.exe'; bytes = $b.Length; sha256 = (Get-FileHash -Algorithm SHA256 -LiteralPath $second).Hash.ToLowerInvariant() }
    byte_identical = ($null -eq $difference)
    first_difference_offset = $difference
}
$report | ConvertTo-Json -Depth 5 | Set-Content -Encoding utf8 (Join-Path $output 'comparison.json')
Write-Output "Comparación de builds: $($report.byte_identical); $($report.first.sha256); informe: $output"
if ($null -ne $difference) { throw "Compilaciones diferentes desde el byte $difference" }
