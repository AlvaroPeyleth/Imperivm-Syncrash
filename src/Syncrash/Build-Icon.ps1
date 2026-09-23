# Package the generated PNG as a Windows icon, preserving alpha at each size.
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
$imagePath = Join-Path $PSScriptRoot 'assets/icon-source.png'
$outputPath = Join-Path $PSScriptRoot 'assets/syncrash.ico'
$image = [Drawing.Image]::FromFile($imagePath)
$frames = [Collections.Generic.List[byte[]]]::new()
$sizes = @(16, 24, 32, 48, 64, 128, 256)
try {
    foreach ($size in $sizes) {
        $bitmap = [Drawing.Bitmap]::new($size, $size, [Drawing.Imaging.PixelFormat]::Format32bppArgb)
        $graphics = [Drawing.Graphics]::FromImage($bitmap)
        $memory = [IO.MemoryStream]::new()
        try {
            $graphics.InterpolationMode = [Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
            $graphics.PixelOffsetMode = [Drawing.Drawing2D.PixelOffsetMode]::HighQuality
            $graphics.DrawImage($image, [Drawing.Rectangle]::new(0, 0, $size, $size))
            $bitmap.Save($memory, [Drawing.Imaging.ImageFormat]::Png)
            $frames.Add($memory.ToArray())
            if ($size -eq 64) { [IO.File]::WriteAllBytes((Join-Path $PSScriptRoot 'assets/mark.png'), $memory.ToArray()) }
        } finally { $graphics.Dispose(); $bitmap.Dispose(); $memory.Dispose() }
    }
} finally { $image.Dispose() }
$file = [IO.File]::Create($outputPath)
$writer = [IO.BinaryWriter]::new($file)
try {
    $writer.Write([uint16]0)
    $writer.Write([uint16]1)
    $writer.Write([uint16]$sizes.Count)
    $offset = 6 + 16 * $sizes.Count
    for ($i = 0; $i -lt $sizes.Count; $i++) {
        $dimension = if ($sizes[$i] -eq 256) { 0 } else { $sizes[$i] }
        $writer.Write([byte]$dimension); $writer.Write([byte]$dimension)
        $writer.Write([byte]0); $writer.Write([byte]0)
        $writer.Write([uint16]1); $writer.Write([uint16]32)
        $writer.Write([uint32]$frames[$i].Length); $writer.Write([uint32]$offset)
        $offset += $frames[$i].Length
    }
    foreach ($frame in $frames) { $writer.Write($frame) }
} finally { $writer.Dispose(); $file.Dispose() }
