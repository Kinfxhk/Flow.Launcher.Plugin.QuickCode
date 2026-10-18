<#
  Converts Unicode Unihan "Unihan_DictionaryLikeData.txt" (kCangjie field) into
  src/Flow.Launcher.Plugin.QuickCode/Data/cangjie.tsv  (char<TAB>CODE, sorted by code point).

  The input file is NOT bundled; the generated cangjie.tsv is committed to the repo.

  Usage:  pwsh -File tools/GenTable.ps1 [-UnihanPath <path>] [-Output <path>]
#>
param(
    [string]$UnihanPath = "C:/Users/kinfx/AppData/Local/hermes/profiles/it-dog/cache/scratch/Unihan_DictionaryLikeData.txt",
    [string]$Output = "src/Flow.Launcher.Plugin.QuickCode/Data/cangjie.tsv"
)

$ErrorActionPreference = "Stop"

if (-not (Test-Path $UnihanPath)) { throw "Input file not found: $UnihanPath" }

$rows = New-Object System.Collections.Generic.List[object]

foreach ($line in [System.IO.File]::ReadLines($UnihanPath)) {
    if ($line.StartsWith("#")) { continue }
    $parts = $line.Split("`t")
    if ($parts.Count -ne 3) { continue }
    if ($parts[1] -ne "kCangjie") { continue }
    $hex = $parts[0]
    if (-not $hex.StartsWith("U+")) { continue }
    $codePoint = [Convert]::ToInt32($hex.Substring(2), 16)
    $char = [char]::ConvertFromUtf32($codePoint)
    $code = $parts[2].Trim()
    if ($code -eq "") { continue }
    $rows.Add([PSCustomObject]@{ CodePoint = $codePoint; Char = $char; Code = $code })
}

$sorted = $rows | Sort-Object -Property CodePoint

$sb = New-Object System.Text.StringBuilder
foreach ($r in $sorted) {
    [void]$sb.Append($r.Char).Append("`t").Append($r.Code).Append("`n")
}

$outDir = Split-Path -Parent $Output
if (-not (Test-Path $outDir)) { New-Item -ItemType Directory -Force -Path $outDir | Out-Null }

$utf8NoBom = New-Object System.Text.UTF8Encoding($false)
[System.IO.File]::WriteAllText($Output, $sb.ToString(), $utf8NoBom)

Write-Host ("Wrote {0} entries to {1}" -f $sorted.Count, $Output)
