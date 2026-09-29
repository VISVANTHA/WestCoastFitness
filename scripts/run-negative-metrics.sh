#!/usr/bin/env bash
# Runs the full metric-analysis suite against artifacts/negative/. Identical
# tooling to run-positive-metrics.sh; the negative branch is expected to
# violate the thresholds these produce, not to fail to run them.
set -uo pipefail
cd "$(dirname "${BASH_SOURCE[0]}")/.."
source scripts/lib/common.sh
source scripts/lib/metrics.sh

VARIANT=negative
mkdir -p "artifacts/${VARIANT}/lint" "artifacts/${VARIANT}/sca" "artifacts/${VARIANT}/sast" \
  "artifacts/${VARIANT}/duplication" "artifacts/${VARIANT}/mutation" "artifacts/${VARIANT}/metrics"

metric_lint "$VARIANT" || true
metric_sca "$VARIANT" || true
metric_sast "$VARIANT" || true
metric_duplication "$VARIANT" || true
metric_mutation "$VARIANT" || true
metric_summary "$VARIANT" || true

echo "Negative metrics complete: artifacts/${VARIANT}/"
