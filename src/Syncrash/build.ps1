param([string]$OutputPath = (Join-Path $PSScriptRoot '../../Syncrash.exe'))
$ErrorActionPreference = 'Stop'
$root = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$sdkLine = dotnet --list-sdks | Where-Object { $_ -match '^8\.0\.400 \[(.+)\]$' } | Select-Object -First 1
if (-not $sdkLine -or $sdkLine -notmatch '^8\.0\.400 \[(.+)\]$') { throw 'Se requiere el SDK oficial .NET 8.0.400.' }
$compiler = Join-Path $Matches[1] '8.0.400/Roslyn/bincore/csc.dll'
if (-not (Test-Path -LiteralPath $compiler)) { throw 'No se encuentra Roslyn csc.dll del SDK .NET 8.0.400.' }
$referenceRoot = Join-Path ${env:ProgramFiles(x86)} 'Reference Assemblies/Microsoft/Framework/.NETFramework/v4.8'
if (-not (Test-Path -LiteralPath $referenceRoot)) { throw 'Se requiere el paquete de referencias .NET Framework 4.8.' }
$output = [IO.Path]::GetFullPath($OutputPath)
if ([IO.Path]::GetFileName($output) -cne 'Syncrash.exe') { throw 'El nombre de salida debe ser Syncrash.exe para fijar el nombre del ensamblado.' }
if (Test-Path -LiteralPath $output) { throw "El destino ya existe; elige una ruta nueva: $output" }
New-Item -ItemType Directory -Force -Path (Split-Path $output) | Out-Null
$recipe = Join-Path $PSScriptRoot 'recipe.json'
$banner = Join-Path $PSScriptRoot 'assets/banner.png'
$manifest = Join-Path $PSScriptRoot 'app.manifest'
$icon = Join-Path $PSScriptRoot 'assets/syncrash.ico'
$mark = Join-Path $PSScriptRoot 'assets/mark.png'
$license = Join-Path $PSScriptRoot '../../LICENSE'
$sources = @('Program.cs', 'PatchEngine.cs', 'Window.cs', 'AssemblyInfo.cs') | ForEach-Object { Join-Path $PSScriptRoot $_ }
$references = @('mscorlib', 'System', 'System.Core', 'System.Windows.Forms', 'System.Drawing', 'System.Runtime.Serialization', 'System.Xml') |
    ForEach-Object { '/r:' + (Join-Path $referenceRoot ($_.ToString() + '.dll')) }
& dotnet exec $compiler /nologo /target:winexe /platform:anycpu /optimize+ /deterministic+ /nostdlib+ /langversion:5 `
    ("/pathmap:$root=/_/syncrash") ("/out:$output") ("/win32manifest:$manifest") ("/win32icon:$icon") `
    ("/resource:$icon,Syncrash.Icon") ("/resource:$mark,Syncrash.Mark") ("/resource:$recipe,Syncrash.Recipe") `
    ("/resource:$banner,Syncrash.Banner") ("/resource:$license,Syncrash.License") $references $sources
if ($LASTEXITCODE -ne 0) { throw 'No se pudo compilar Syncrash.' }
Get-FileHash -Algorithm SHA256 -LiteralPath $output
