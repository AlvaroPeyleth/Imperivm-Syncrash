param([string]$OutputDirectory = (Join-Path ([IO.Path]::GetTempPath()) ('syncrash-tests-' + [guid]::NewGuid().ToString('N'))))
$ErrorActionPreference = 'Stop'
$root = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$source = Join-Path $root 'src/Syncrash'
$output = [IO.Path]::GetFullPath($OutputDirectory)
New-Item -ItemType Directory -Force -Path $output | Out-Null
$app = Join-Path $output 'Syncrash.exe'
$tests = Join-Path $output 'SyncrashTests.exe'
& (Join-Path $source 'build.ps1') -OutputPath $app | Out-Host

$compiler = Join-Path $env:WINDIR 'Microsoft.NET/Framework64/v4.0.30319/csc.exe'
if (-not (Test-Path -LiteralPath $compiler)) { $compiler = Join-Path $env:WINDIR 'Microsoft.NET/Framework/v4.0.30319/csc.exe' }
if (-not (Test-Path -LiteralPath $compiler)) { throw 'No se encuentra el compilador de .NET Framework.' }
$sources = @('Program.cs', 'PatchEngine.cs', 'Window.cs', 'AssemblyInfo.cs') | ForEach-Object { Join-Path $source $_ }
& $compiler /nologo /target:exe /platform:anycpu /optimize+ /main:SyncrashTests ("/out:$tests") `
    ("/resource:$(Join-Path $source 'recipe.json'),Syncrash.Recipe") `
    /r:System.Windows.Forms.dll /r:System.Drawing.dll /r:System.Runtime.Serialization.dll `
    $sources (Join-Path $PSScriptRoot 'SyncrashTests.cs')
if ($LASTEXITCODE -ne 0) { throw 'No se pudo compilar el arnés de pruebas.' }
& $tests $app
if ($LASTEXITCODE -ne 0) { throw "Pruebas fallidas: $LASTEXITCODE" }
Write-Output "Artefactos de prueba: $output"
