# Runs the full metric-analysis suite against artifacts/positive/. Run after
# build-positive.ps1. Each metric step is independent and best-effort so one
# missing tool doesn't prevent the rest from being collected.
Set-Location (Split-Path -Parent $PSScriptRoot)
. ./scripts/lib/common.ps1
. ./scripts/lib/metrics.ps1

$Variant = 'positive'
@('lint', 'sca', 'sast', 'duplication', 'mutation', 'metrics') | ForEach-Object {
    New-Item -ItemType Directory -Force -Path "artifacts/$Variant/$_" | Out-Null
}

Invoke-LintMetric -Variant $Variant
Invoke-ScaMetric -Variant $Variant
Invoke-SastMetric -Variant $Variant
Invoke-DuplicationMetric -Variant $Variant
Invoke-MutationMetric -Variant $Variant
Write-MetricsSummary -Variant $Variant

Write-Host "Positive metrics complete: artifacts/$Variant/"
