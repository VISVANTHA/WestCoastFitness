# Branch-aware build entry point. Detects the current git branch and
# dispatches to build-positive.ps1 or build-negative.ps1.
Set-Location (Split-Path -Parent $PSScriptRoot)
. ./scripts/lib/common.ps1

$branch = Get-CurrentBranch
$variant = Get-BranchVariant -Branch $branch

switch ($variant) {
    'positive' { & ./scripts/build-positive.ps1; exit $LASTEXITCODE }
    'negative' { & ./scripts/build-negative.ps1; exit $LASTEXITCODE }
    default {
        Write-Error "The benchmark build must be run from the WestCoastFitness-positive or WestCoastFitness-negative branch (current branch: '$branch')."
        exit 1
    }
}
