param([string]$OutputDirectory = (Join-Path ([IO.Path]::GetTempPath()) ('syncrash-tests-' + [guid]::NewGuid().ToString('N'))), [string]$ScreenBundleDirectory)
$ErrorActionPreference = 'Stop'
$root = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$source = Join-Path $root 'src/Syncrash'
$output = [IO.Path]::GetFullPath($OutputDirectory)
New-Item -ItemType Directory -Force -Path $output | Out-Null
$app = Join-Path $output 'Syncrash.exe'
$tests = Join-Path $output 'SyncrashTests.exe'
& (Join-Path $source 'build.ps1') -OutputPath $app -ScreenBundleDirectory $ScreenBundleDirectory | Out-Host

$compiler = Join-Path $env:WINDIR 'Microsoft.NET/Framework64/v4.0.30319/csc.exe'
if (-not (Test-Path -LiteralPath $compiler)) { $compiler = Join-Path $env:WINDIR 'Microsoft.NET/Framework/v4.0.30319/csc.exe' }
if (-not (Test-Path -LiteralPath $compiler)) { throw 'No se encuentra el compilador de .NET Framework.' }
$sources = @('Program.cs', 'PatchEngine.cs', 'Window.cs', 'ScreenCompatibility.cs', 'VoiceCompatibility.cs', 'AssemblyInfo.cs', 'CurseGuard.cs', 'MenuGuard.cs') | ForEach-Object { Join-Path $source $_ }
$guardResources = @(
    "/resource:$(Join-Path $source 'curse-rc1.json'),Syncrash.curse-rc1",
    "/resource:$(Join-Path $source 'curse-rc1-remove.json'),Syncrash.curse-rc1-remove",

    "/resource:$(Join-Path $source 'menu-rc1.json'),Syncrash.menu-rc1",
    "/resource:$(Join-Path $source 'menu-only-rc1.json'),Syncrash.menu-only-rc1",
    "/resource:$(Join-Path $source 'menu-rc1-remove.json'),Syncrash.menu-rc1-remove",
    "/resource:$(Join-Path $source 'menu-only-rc1-remove.json'),Syncrash.menu-only-rc1-remove",
    "/resource:$(Join-Path $source 'upgrade-rc1-minus.json'),Syncrash.upgrade-rc1-minus",

    "/resource:$(Join-Path $source 'upgrade-laa.json'),Syncrash.upgrade-laa",
    "/resource:$(Join-Path $source 'menu-guard.json'),Syncrash.menu-guard",
    "/resource:$(Join-Path $source 'menu-guard-remove.json'),Syncrash.menu-guard-remove",
    "/resource:$(Join-Path $source 'menu-only-remove.json'),Syncrash.menu-only-remove",
    "/resource:$(Join-Path $source 'menu-only.json'),Syncrash.menu-only",
    "/resource:$(Join-Path $source 'curse-with-menu.json'),Syncrash.curse-with-menu",
    "/resource:$(Join-Path $source 'curse-with-menu-remove.json'),Syncrash.curse-with-menu-remove",

    "/resource:$(Join-Path $source 'curse-guard.json'),Syncrash.CurseGuard",
    "/resource:$(Join-Path $source 'curse-guard-remove.json'),Syncrash.CurseGuardRemove")
& $compiler /nologo /target:exe /platform:anycpu /optimize+ /main:SyncrashTests ("/out:$tests") `
    ("/resource:$(Join-Path $source 'recipe.json'),Syncrash.Recipe") `
    ("/resource:$(Join-Path $source 'upgrade-v2.json'),Syncrash.UpgradeV2") `
    ("/resource:$(Join-Path $source 'voice-map.json'),Syncrash.Voices") `
    /r:System.Windows.Forms.dll /r:System.Drawing.dll /r:System.Runtime.Serialization.dll `
    $guardResources $sources (Join-Path $PSScriptRoot 'SyncrashTests.cs') (Join-Path $PSScriptRoot 'ScreenCompatibilityTests.cs') (Join-Path $PSScriptRoot 'VoiceCompatibilityTests.cs')
if ($LASTEXITCODE -ne 0) { throw 'No se pudo compilar el arnés de pruebas.' }
& $tests $app
if ($LASTEXITCODE -ne 0) { throw "Pruebas fallidas: $LASTEXITCODE" }
Write-Output "Artefactos de prueba: $output"
