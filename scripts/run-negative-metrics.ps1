# Runs the full metric-analysis suite against artifacts/negative/. Identical
# tooling to run-positive-metrics.ps1; the negative branch is expected to
# violate the thresholds these produce, not to fail to run them.
Set-Location (Split-Path -Parent $PSScriptRoot)
. ./scripts/lib/common.ps1
. ./scripts/lib/metrics.ps1

$Variant = 'negative'
@('lint', 'sca', 'sast', 'duplication', 'mutation', 'metrics') | ForEach-Object {
    New-Item -ItemType Directory -Force -Path "artifacts/$Variant/$_" | Out-Null
}

try { Invoke-LintMetric -Variant $Variant } catch { Write-Warning "lint metric step failed: $_" }
try { Invoke-ScaMetric -Variant $Variant } catch { Write-Warning "sca metric step failed: $_" }
try { Invoke-SastMetric -Variant $Variant } catch { Write-Warning "sast metric step failed: $_" }
try { Invoke-DuplicationMetric -Variant $Variant } catch { Write-Warning "duplication metric step failed: $_" }
try { Invoke-MutationMetric -Variant $Variant } catch { Write-Warning "mutation metric step failed: $_" }
try { Write-MetricsSummary -Variant $Variant } catch { Write-Warning "metrics summary step failed: $_" }

Write-Host "Negative metrics complete: artifacts/$Variant/"
