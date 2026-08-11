#Requires -Version 7
$ErrorActionPreference = 'Stop'
Set-Location $PSScriptRoot

# Clean up
Remove-Item -Recurse -Force interp, grammar.json, parse-result.json, trparse-interp.tree `
    -ErrorAction SilentlyContinue

# Step 1: parse Expression.g4 grammar → JSON, then generate .interp files from that JSON.
dotnet trash parse Expression.g4 | Out-File grammar.json -Encoding utf8NoBOM
if ($LASTEXITCODE -ne 0) { Write-Error "trparse failed"; exit 1 }
dotnet trash interp -f grammar.json -o interp/
if ($LASTEXITCODE -ne 0) { Write-Error "trinterp failed"; exit 1 }

# Step 2: parse the sample expression via Earley ATN path (-i avoids a stdin pipe).
dotnet trash parse --lib interp/ -i "1 + 2 + 3" | Out-File parse-result.json -Encoding utf8NoBOM
if ($LASTEXITCODE -ne 0) { Write-Error "trparse --lib failed"; exit 1 }

# Step 3: render the parse tree (-f avoids a stdin pipe).
dotnet trash tree -f parse-result.json | Out-File trparse-interp.tree -Encoding utf8NoBOM
if ($LASTEXITCODE -ne 0) { Write-Error "trtree failed"; exit 1 }

# Step 4: compare to golden output (normalise line endings).
$got  = (Get-Content trparse-interp.tree  -Raw) -replace "`r`n","`n" -replace "`r","`n"
$gold = (Get-Content Gold/trparse-interp.tree -Raw) -replace "`r`n","`n" -replace "`r","`n"

if ($got -eq $gold) {
    Write-Host "Test succeeded."
    exit 0
} else {
    Write-Host "Test FAILED."
    $gotLines  = $got  -split "`n"
    $goldLines = $gold -split "`n"
    $max = [Math]::Max($gotLines.Count, $goldLines.Count)
    for ($i = 0; $i -lt $max; $i++) {
        $g = if ($i -lt $gotLines.Count)  { $gotLines[$i]  } else { "<missing>" }
        $e = if ($i -lt $goldLines.Count) { $goldLines[$i] } else { "<missing>" }
        if ($g -ne $e) {
            Write-Host "Line $($i+1):"
            Write-Host "  got : $g"
            Write-Host "  want: $e"
        }
    }
    exit 1
}
