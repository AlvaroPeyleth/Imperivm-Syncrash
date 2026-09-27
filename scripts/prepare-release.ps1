param(
    [Parameter(Mandatory = $true)][string]$ExePath,
    [Parameter(Mandatory = $true)][string]$OutputDirectory,
    [string]$ScreenBundleDirectory
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
$assemblyInfo = Get-Content -LiteralPath (Join-Path $root 'src/Syncrash/AssemblyInfo.cs') -Raw -Encoding UTF8
$versionMatches = [regex]::Matches($assemblyInfo, '(?m)^\s*\[assembly:\s*AssemblyVersion\("(\d+\.\d+\.\d+\.\d+)"\)\]\s*$')
if ($versionMatches.Count -ne 1) { throw 'No se pudo identificar una única AssemblyVersion en AssemblyInfo.cs.' }
$expectedVersion = $versionMatches[0].Groups[1].Value
if ($version -ne $expectedVersion) { throw "Versión de candidato inesperada: $version; se esperaba $expectedVersion." }
if (-not $ScreenBundleDirectory) { throw 'Indica ScreenBundleDirectory con los componentes y fuentes revisados para reproducir el EXE completo.' }
$ScreenBundleDirectory = [IO.Path]::GetFullPath($ScreenBundleDirectory)
$screenFiles = @()
foreach ($name in @('Syncrash-screen-LICENSE.txt','dxwnd.dxw','dxwnd-smooth.dxw','dxwnd.dll','winmm.dll','PROVENANCE.txt','Sources/dxwnd-2.06.15-source.rar','Sources/proxy-source.zip','Sources/syncrash-dxwnd-changes.zip')) {
    $path = Join-Path $ScreenBundleDirectory $name
    if (-not (Test-Path -LiteralPath $path -PathType Leaf)) { throw "Falta el componente o fuente: $name" }
    $screenFiles += [pscustomobject]@{ file=$name; sha256=(Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash.ToLowerInvariant(); bytes=(Get-Item -LiteralPath $path).Length }
}
# build.ps1 validates every bundled input against reviewed hashes and embeds its sources.
$exeInfo = [ordered]@{
    file = [IO.Path]::GetFileName($exe)
    bytes = (Get-Item -LiteralPath $exe).Length
    sha256 = (Get-FileHash -Algorithm SHA256 -LiteralPath $exe).Hash.ToLowerInvariant()
}
$rebuildDirectory = Join-Path ([IO.Path]::GetTempPath()) ('syncrash-release-check-' + [guid]::NewGuid().ToString('N'))
$rebuild = Join-Path $rebuildDirectory 'Syncrash.exe'
try {
    & (Join-Path $root 'src/Syncrash/build.ps1') -OutputPath $rebuild -ScreenBundleDirectory $ScreenBundleDirectory | Out-Null
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
    packaging_host = "PowerShell $($PSVersionTable.PSVersion); .NET $([Environment]::Version)"
    executable = $exeInfo
    package_exe_sha256 = $finalHash
    embedded_screen_inputs = $screenFiles
}
$manifest | ConvertTo-Json -Depth 5 | Set-Content -Encoding utf8 (Join-Path $package 'release-manifest.json')
("$finalHash  Syncrash.exe`n") | Set-Content -Encoding ascii (Join-Path $package 'SHA256SUMS.txt')
@"
Syncrash ${version}: candidato local para pruebas, sin publicar.
Uso: cierra Imperivm, comprueba tu instalación Steam vanilla y aplica desde la interfaz.
Pantalla adaptable y Reparar voces de unidades son opcionales y están marcadas por defecto. Marcar y aplicar instala cada función; desmarcar y aplicar la retira. Pantalla incluye suavizado GPU.
Voces admite español, italiano e inglés según Settings.ini. Extrae WAV de tus PAK locales sin modificarlos. Si cambias de idioma, cierra el juego y vuelve a aplicar Syncrash antes de jugar. Desmarcar voces y aplicar retira los archivos registrados; conserva archivos ajenos o modificados y avisa si impiden completar la retirada.
Syncrash.exe incluye pantalla y fuentes/licencias; no necesita una carpeta screen externa. Tras aplicar, cierra Syncrash y abre Imperivm (versión Steam), también directamente desde gbr.exe. Exporta las fuentes desde el pie de la interfaz o --export-screen-sources <nuevo.zip>.
Recuperación: desmarca pantalla y voces, aplica y después verifica los archivos del juego en Steam. Steam no retira por sí solo los WAV añadidos. No se crea copia de gbr.exe.
Código, recetas y compilación: https://github.com/AlvaroPeyleth/Imperivm-Syncrash. Fuentes de pantalla exportables desde el EXE.
SHA256SUMS.txt identifica el ejecutable de esta entrega.
"@ | Set-Content -Encoding utf8 (Join-Path $package 'LEEME.txt')
$zipName = "Syncrash-$version-candidato.zip"
$zip = Join-Path $output $zipName
Compress-Archive -Path (Join-Path $package '*') -DestinationPath $zip
$zipHash = (Get-FileHash -Algorithm SHA256 -LiteralPath $zip).Hash.ToLowerInvariant()
("$zipHash  $zipName`n") | Set-Content -Encoding ascii (Join-Path $output 'ZIP-SHA256SUMS.txt')
Write-Output "Candidato local: $output; EXE $finalHash; ZIP $zipHash"
