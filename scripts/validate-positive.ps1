# Sanity-checks that artifacts/positive/ contains the outputs build-positive.ps1
# is supposed to produce. Run after build-positive.ps1.
Set-Location (Split-Path -Parent $PSScriptRoot)

$Variant = 'positive'
$Missing = @()

$required = @(
    "artifacts/$Variant/build-info.json",
    "artifacts/$Variant/frontend/index.html",
    "artifacts/$Variant/backend/WestCoastFitness.Api.dll",
    "artifacts/$Variant/tests"
)

foreach ($path in $required) {
    if (-not (Test-Path $path)) { $Missing += $path }
}

if ($Missing.Count -gt 0) {
    Write-Error "Validation FAILED for $Variant branch. Missing: $($Missing -join ', ')"
    exit 1
}

Write-Host "Validation passed: artifacts/$Variant/ contains the expected build outputs."
