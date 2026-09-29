# Compares artifacts/positive/metrics/summary.json against
# artifacts/negative/metrics/summary.json and prints a side-by-side table.
# Run after both branches have been built and metered at least once.
Set-Location (Split-Path -Parent $PSScriptRoot)

$positivePath = "artifacts/positive/metrics/summary.json"
$negativePath = "artifacts/negative/metrics/summary.json"

foreach ($path in @($positivePath, $negativePath)) {
    if (-not (Test-Path $path)) {
        Write-Error "Missing $path. Run run-positive-metrics.ps1 / run-negative-metrics.ps1 on both branches first."
        exit 1
    }
}

$positive = Get-Content $positivePath -Raw | ConvertFrom-Json
$negative = Get-Content $negativePath -Raw | ConvertFrom-Json

$positiveFailures = "artifacts/positive/metrics/build-failures.txt"
$negativeFailures = "artifacts/negative/metrics/build-failures.txt"
$positiveFailCount = if (Test-Path $positiveFailures) { (Get-Content $positiveFailures).Count } else { 0 }
$negativeFailCount = if (Test-Path $negativeFailures) { (Get-Content $negativeFailures).Count } else { 0 }

Write-Host ("{0,-27}| {1,-13}| {2}" -f "Metric", "Positive", "Negative")
Write-Host ("-" * 27 + "|" + "-" * 14 + "|" + "-" * 13)
Write-Host ("{0,-27}| {1,-13}| {2}" -f "Frontend line coverage", $positive.frontendLineCoverage, $negative.frontendLineCoverage)
Write-Host ("{0,-27}| {1,-13}| {2}" -f "Backend line coverage", $positive.backendLineCoverage, $negative.backendLineCoverage)
Write-Host ("{0,-27}| {1,-13}| {2}" -f "Failing build steps", $positiveFailCount, $negativeFailCount)

Write-Host ""
Write-Host "Full detail: artifacts/positive/ vs artifacts/negative/"
