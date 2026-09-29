#!/usr/bin/env bash
# Runs the full metric-analysis suite against artifacts/positive/. Run after
# build-positive.sh. Each metric step is independent and best-effort so one
# missing tool doesn't prevent the rest from being collected.
set -uo pipefail
cd "$(dirname "${BASH_SOURCE[0]}")/.."
source scripts/lib/common.sh
source scripts/lib/metrics.sh

VARIANT=positive
mkdir -p "artifacts/${VARIANT}/lint" "artifacts/${VARIANT}/sca" "artifacts/${VARIANT}/sast" \
  "artifacts/${VARIANT}/duplication" "artifacts/${VARIANT}/mutation" "artifacts/${VARIANT}/metrics"

metric_lint "$VARIANT"
metric_sca "$VARIANT"
metric_sast "$VARIANT"
metric_duplication "$VARIANT"
metric_mutation "$VARIANT"
metric_summary "$VARIANT"

echo "Positive metrics complete: artifacts/${VARIANT}/"
