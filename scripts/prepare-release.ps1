param(
    [Parameter(Mandatory = $true)][string]$ExePath,
    [Parameter(Mandatory = $true)][string]$OutputDirectory
)
$ErrorActionPreference = 'Stop'
$root = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$exe = [IO.Path]::GetFullPath($ExePath)
$output = [IO.Path]::GetFullPath($OutputDirectory)
if (-not (Test-Path -LiteralPath $exe -PathType Leaf)) { throw 'Falta el ejecutable candidato.' }
if (Test-Path -LiteralPath $output) { throw "El directorio de entrega ya existe: $output" }
$gitArgs = @('-c', "safe.directory=$($root.Replace('\', '/'))", '-C', $root)
$commit = (& git @gitArgs rev-parse HEAD).Trim()
if ($LASTEXITCODE -ne 0 -or $commit -notmatch '^[0-9a-f]{40}$') { throw 'No se pudo identificar el commit fuente.' }
if (& git @gitArgs status --porcelain) { throw 'El árbol fuente tiene cambios: prepara el candidato desde un commit limpio.' }
$version = [Reflection.AssemblyName]::GetAssemblyName($exe).Version.ToString()
if ($version -ne '1.0.3.0') { throw "Versión de candidato inesperada: $version" }
$exeInfo = [ordered]@{
    file = [IO.Path]::GetFileName($exe)
    bytes = (Get-Item -LiteralPath $exe).Length
    sha256 = (Get-FileHash -Algorithm SHA256 -LiteralPath $exe).Hash.ToLowerInvariant()
}
$rebuildDirectory = Join-Path ([IO.Path]::GetTempPath()) ('syncrash-release-check-' + [guid]::NewGuid().ToString('N'))
$rebuild = Join-Path $rebuildDirectory 'Syncrash.exe'
try {
    & (Join-Path $root 'src/Syncrash/build.ps1') -OutputPath $rebuild | Out-Null
    $sourceHash = (Get-FileHash -Algorithm SHA256 -LiteralPath $rebuild).Hash.ToLowerInvariant()
    if ($sourceHash -ne $exeInfo.sha256) { throw 'El candidato no coincide con un build del commit fuente limpio.' }
}
finally {
    if (Test-Path -LiteralPath $rebuild) { Remove-Item -LiteralPath $rebuild }
    if (Test-Path -LiteralPath $rebuildDirectory) { Remove-Item -LiteralPath $rebuildDirectory }
}
$package = Join-Path $output 'package'
New-Item -ItemType Directory -Path $package -Force | Out-Null
Copy-Item -LiteralPath $exe -Destination (Join-Path $package 'Syncrash.exe')
Copy-Item -LiteralPath (Join-Path $root 'LICENSE') -Destination (Join-Path $package 'LICENSE')
$finalHash = (Get-FileHash -Algorithm SHA256 -LiteralPath (Join-Path $package 'Syncrash.exe')).Hash.ToLowerInvariant()
if ($finalHash -ne $exeInfo.sha256) { throw 'La copia del paquete no conserva los bytes verificados.' }
$manifest = [ordered]@{
    created_at_utc = [DateTime]::UtcNow.ToString('o')
    source_commit = $commit
    source_working_tree_clean = $true
    candidate_version = $version
    published = $false
    target = '.NET Framework 4.8; Windows WinForms; AnyCPU'
    compiler = '.NET SDK 8.0.400 Roslyn; deterministic build'
    executable = $exeInfo
    package_exe_sha256 = $finalHash
}
$manifest | ConvertTo-Json -Depth 5 | Set-Content -Encoding utf8 (Join-Path $package 'release-manifest.json')
("$finalHash  Syncrash.exe`n") | Set-Content -Encoding ascii (Join-Path $package 'SHA256SUMS.txt')
@'
Syncrash 1.0.3.0: candidato local para pruebas. No es la entrega v1.0.0 publicada.
Uso: cierra Imperivm, comprueba tu instalación Steam vanilla y aplica desde la interfaz.
Recuperación: verifica los archivos del juego en Steam. No se crea copia de seguridad.
El aplicador no lleva firma digital: comprueba su SHA256 con SHA256SUMS.txt y con la ficha de esta entrega.
No desactives protecciones del sistema para ejecutar el aplicador.
'@ | Set-Content -Encoding utf8 (Join-Path $package 'LEEME.txt')
$zip = Join-Path $output 'Syncrash-1.0.3-candidato.zip'
Compress-Archive -Path (Join-Path $package '*') -DestinationPath $zip
$zipHash = (Get-FileHash -Algorithm SHA256 -LiteralPath $zip).Hash.ToLowerInvariant()
("$zipHash  Syncrash-1.0.3-candidato.zip`n") | Set-Content -Encoding ascii (Join-Path $output 'ZIP-SHA256SUMS.txt')
Write-Output "Candidato local: $output; EXE $finalHash; ZIP $zipHash"
