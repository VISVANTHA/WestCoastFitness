#!/usr/bin/env bash
# Sanity-checks that artifacts/positive/ contains the outputs build-positive.sh
# is supposed to produce. Run after build-positive.sh.
set -uo pipefail
cd "$(dirname "${BASH_SOURCE[0]}")/.."

VARIANT=positive
MISSING=()

check() {
  local path="$1"
  if [ ! -e "$path" ]; then
    MISSING+=("$path")
  fi
}

check "artifacts/${VARIANT}/build-info.json"
check "artifacts/${VARIANT}/frontend/index.html"
check "artifacts/${VARIANT}/backend/WestCoastFitness.Api.dll"
check "artifacts/${VARIANT}/tests"

if [ "${#MISSING[@]}" -gt 0 ]; then
  echo "Validation FAILED for ${VARIANT} branch. Missing:" >&2
  printf ' - %s\n' "${MISSING[@]}" >&2
  exit 1
fi

echo "Validation passed: artifacts/${VARIANT}/ contains the expected build outputs."
