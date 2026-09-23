param([string]$OutputPath = (Join-Path $PSScriptRoot '../../Syncrash.exe'))
$ErrorActionPreference = 'Stop'
$compiler = Join-Path $env:WINDIR 'Microsoft.NET/Framework64/v4.0.30319/csc.exe'
if (-not (Test-Path -LiteralPath $compiler)) {
    $compiler = Join-Path $env:WINDIR 'Microsoft.NET/Framework/v4.0.30319/csc.exe'
}
if (-not (Test-Path -LiteralPath $compiler)) { throw 'No se encuentra el compilador de .NET Framework.' }
$output = [IO.Path]::GetFullPath($OutputPath)
$recipe = Join-Path $PSScriptRoot 'recipe.json'
$banner = Join-Path $PSScriptRoot 'assets/banner.png'
$manifest = Join-Path $PSScriptRoot 'app.manifest'
$icon = Join-Path $PSScriptRoot 'assets/syncrash.ico'
$mark = Join-Path $PSScriptRoot 'assets/mark.png'
$sources = @('Program.cs', 'Window.cs', 'AssemblyInfo.cs') | ForEach-Object { Join-Path $PSScriptRoot $_ }
& $compiler /nologo /target:winexe /platform:anycpu /optimize+ ("/out:$output") ("/win32manifest:$manifest") ("/win32icon:$icon") ("/resource:$icon,Syncrash.Icon") ("/resource:$mark,Syncrash.Mark") ("/resource:$recipe,Syncrash.Recipe") ("/resource:$banner,Syncrash.Banner") /r:System.Windows.Forms.dll /r:System.Drawing.dll /r:System.Runtime.Serialization.dll $sources
if ($LASTEXITCODE -ne 0) { throw 'No se pudo compilar Syncrash.' }
Get-FileHash -Algorithm SHA256 -LiteralPath $output
