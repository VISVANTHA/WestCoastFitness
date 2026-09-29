#!/usr/bin/env bash
# Shared helpers for the WestCoastFitness benchmark build/metric scripts.
# Sourced by the other scripts in this directory; not meant to be run directly.

repo_root() {
  cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd
}

current_branch() {
  git branch --show-current
}

branch_variant() {
  local branch="$1"
  case "$branch" in
    *-positive) echo "positive" ;;
    *-negative) echo "negative" ;;
    *) echo "" ;;
  esac
}

require_variant_branch() {
  local branch
  branch="$(current_branch)"
  local variant
  variant="$(branch_variant "$branch")"
  if [ -z "$variant" ]; then
    echo "The benchmark build must be run from the WestCoastFitness-positive or WestCoastFitness-negative branch (current branch: '${branch}')." >&2
    exit 1
  fi
  echo "$variant"
}

clean_artifacts() {
  local variant="$1"
  rm -rf "artifacts/${variant}"
  mkdir -p \
    "artifacts/${variant}/frontend" \
    "artifacts/${variant}/backend" \
    "artifacts/${variant}/tests" \
    "artifacts/${variant}/coverage/frontend" \
    "artifacts/${variant}/coverage/backend" \
    "artifacts/${variant}/lint" \
    "artifacts/${variant}/sast" \
    "artifacts/${variant}/sca" \
    "artifacts/${variant}/duplication" \
    "artifacts/${variant}/mutation" \
    "artifacts/${variant}/metrics"
}

write_build_info() {
  local variant="$1"
  local out="artifacts/${variant}/build-info.json"
  local commit node_version npm_version dotnet_version shell_version timestamp

  commit="$(git rev-parse HEAD 2>/dev/null || echo unknown)"
  node_version="$(node --version 2>/dev/null || echo unknown)"
  npm_version="$(npm --version 2>/dev/null || echo unknown)"
  dotnet_version="$(dotnet --version 2>/dev/null || echo unknown)"
  shell_version="Bash $(bash --version 2>/dev/null | head -n1 | grep -oE '[0-9]+\.[0-9]+\.[0-9]+' | head -n1 || echo unknown)"
  timestamp="$(date -u +"%Y-%m-%dT%H:%M:%SZ")"

  cat > "$out" <<JSON
{
  "project": "Leading - West Coast Fitness Club",
  "branch": "${variant}",
  "gitCommit": "${commit}",
  "buildConfiguration": "Release",
  "dotnetVersion": "${dotnet_version}",
  "nodeVersion": "${node_version}",
  "npmVersion": "${npm_version}",
  "shell": "${shell_version}",
  "buildTimestampUtc": "${timestamp}"
}
JSON
}

# Runs "$@" and appends its name to the FAILURES array (must be declared by
# the caller) instead of stopping the script when it fails. Used by the
# negative-branch scripts, which must collect every failure rather than
# aborting after the first one.
run_step() {
  local name="$1"
  shift
  echo "== ${name} =="
  if ! "$@"; then
    echo "!! ${name} failed (continuing)" >&2
    FAILURES+=("${name}")
    return 1
  fi
  return 0
}
