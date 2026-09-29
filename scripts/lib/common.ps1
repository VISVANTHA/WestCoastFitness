# Shared helpers for the WestCoastFitness benchmark build/metric scripts.
# Dot-sourced by the other scripts in this directory; not meant to be run directly.

function Get-RepoRoot {
    Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
}

function Get-CurrentBranch {
    (git branch --show-current).Trim()
}

function Get-BranchVariant {
    param([string]$Branch)
    if ($Branch -like '*-positive') { return 'positive' }
    if ($Branch -like '*-negative') { return 'negative' }
    return ''
}

function Assert-VariantBranch {
    $branch = Get-CurrentBranch
    $variant = Get-BranchVariant -Branch $branch
    if (-not $variant) {
        Write-Error "The benchmark build must be run from the WestCoastFitness-positive or WestCoastFitness-negative branch (current branch: '$branch')."
        exit 1
    }
    return $variant
}

function Reset-Artifacts {
    param([string]$Variant)
    $root = "artifacts/$Variant"
    if (Test-Path $root) { Remove-Item -Recurse -Force $root }
    $subdirs = @('frontend', 'backend', 'tests', 'coverage/frontend', 'coverage/backend',
        'lint', 'sast', 'sca', 'duplication', 'mutation', 'metrics')
    foreach ($sub in $subdirs) {
        New-Item -ItemType Directory -Force -Path (Join-Path $root $sub) | Out-Null
    }
}

function Write-BuildInfo {
    param([string]$Variant)

    $commit = (git rev-parse HEAD 2>$null)
    if (-not $commit) { $commit = 'unknown' }

    $nodeVersion = (node --version 2>$null)
    if (-not $nodeVersion) { $nodeVersion = 'unknown' }

    $npmVersion = (npm --version 2>$null)
    if (-not $npmVersion) { $npmVersion = 'unknown' }

    $dotnetVersion = (dotnet --version 2>$null)
    if (-not $dotnetVersion) { $dotnetVersion = 'unknown' }

    $shellVersion = "PowerShell $($PSVersionTable.PSVersion.ToString())"
    $timestamp = (Get-Date).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")

    $buildInfo = [ordered]@{
        project              = 'Leading - West Coast Fitness Club'
        branch               = $Variant
        gitCommit            = $commit
        buildConfiguration   = 'Release'
        dotnetVersion        = $dotnetVersion
        nodeVersion          = $nodeVersion
        npmVersion           = $npmVersion
        shell                = $shellVersion
        buildTimestampUtc    = $timestamp
    }

    $outPath = "artifacts/$Variant/build-info.json"
    $buildInfo | ConvertTo-Json | Set-Content -Path $outPath -Encoding utf8
}

# Runs a script block, appending $Name to $Failures (a List[string] the
# caller must pass by reference) instead of throwing when it fails. Used by
# the negative-branch scripts, which must collect every failure rather than
# aborting after the first one.
function Invoke-Step {
    param(
        [string]$Name,
        [scriptblock]$Action,
        [System.Collections.Generic.List[string]]$Failures
    )
    Write-Host "== $Name =="
    try {
        & $Action
        if ($LASTEXITCODE -and $LASTEXITCODE -ne 0) {
            throw "exit code $LASTEXITCODE"
        }
    }
    catch {
        Write-Warning "$Name failed (continuing): $_"
        $Failures.Add($Name)
    }
}
