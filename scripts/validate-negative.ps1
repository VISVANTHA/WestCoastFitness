# Sanity-checks artifacts/negative/. Unlike validate-positive.ps1, a missing
# backend/frontend artifact is only a warning here: the negative branch is
# allowed to fail its quality gates, but the build should still preferably
# compile. build-info.json and the tests directory (evidence) are required.
Set-Location (Split-Path -Parent $PSScriptRoot)

$Variant = 'negative'
$MissingRequired = @()
$MissingOptional = @()

if (-not (Test-Path "artifacts/$Variant/build-info.json")) { $MissingRequired += "artifacts/$Variant/build-info.json" }
if (-not (Test-Path "artifacts/$Variant/tests")) { $MissingRequired += "artifacts/$Variant/tests" }

if (-not (Test-Path "artifacts/$Variant/frontend/index.html")) { $MissingOptional += "artifacts/$Variant/frontend/index.html" }
if (-not (Test-Path "artifacts/$Variant/backend/WestCoastFitness.Api.dll")) { $MissingOptional += "artifacts/$Variant/backend/WestCoastFitness.Api.dll" }

if ($MissingOptional.Count -gt 0) {
    Write-Warning "The negative branch did not produce (this may be expected): $($MissingOptional -join ', ')"
}

if ($MissingRequired.Count -gt 0) {
    Write-Error "Validation FAILED for $Variant branch. Missing required evidence: $($MissingRequired -join ', ')"
    exit 1
}

Write-Host "Validation passed: artifacts/$Variant/ contains the required benchmark evidence."
