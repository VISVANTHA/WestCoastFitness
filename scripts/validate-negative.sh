#!/usr/bin/env bash
# Sanity-checks artifacts/negative/. Unlike validate-positive.sh, a missing
# backend/frontend artifact is only a warning here: the negative branch is
# allowed to fail its quality gates, but the build should still preferably
# compile. build-info.json and the tests directory (evidence) are required.
set -uo pipefail
cd "$(dirname "${BASH_SOURCE[0]}")/.."

VARIANT=negative
MISSING_REQUIRED=()
MISSING_OPTIONAL=()

[ -e "artifacts/${VARIANT}/build-info.json" ] || MISSING_REQUIRED+=("artifacts/${VARIANT}/build-info.json")
[ -e "artifacts/${VARIANT}/tests" ] || MISSING_REQUIRED+=("artifacts/${VARIANT}/tests")

[ -e "artifacts/${VARIANT}/frontend/index.html" ] || MISSING_OPTIONAL+=("artifacts/${VARIANT}/frontend/index.html")
[ -e "artifacts/${VARIANT}/backend/WestCoastFitness.Api.dll" ] || MISSING_OPTIONAL+=("artifacts/${VARIANT}/backend/WestCoastFitness.Api.dll")

if [ "${#MISSING_OPTIONAL[@]}" -gt 0 ]; then
  echo "Note: the negative branch did not produce (this may be expected):" >&2
  printf ' - %s\n' "${MISSING_OPTIONAL[@]}" >&2
fi

if [ "${#MISSING_REQUIRED[@]}" -gt 0 ]; then
  echo "Validation FAILED for ${VARIANT} branch. Missing required evidence:" >&2
  printf ' - %s\n' "${MISSING_REQUIRED[@]}" >&2
  exit 1
fi

echo "Validation passed: artifacts/${VARIANT}/ contains the required benchmark evidence."
