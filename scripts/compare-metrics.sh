#!/usr/bin/env bash
# Compares artifacts/positive/metrics/summary.json against
# artifacts/negative/metrics/summary.json and prints a side-by-side table.
# Run after both branches have been built and metered at least once.
set -uo pipefail
cd "$(dirname "${BASH_SOURCE[0]}")/.."

POSITIVE_SUMMARY="artifacts/positive/metrics/summary.json"
NEGATIVE_SUMMARY="artifacts/negative/metrics/summary.json"

for f in "$POSITIVE_SUMMARY" "$NEGATIVE_SUMMARY"; do
  if [ ! -f "$f" ]; then
    echo "Missing ${f}. Run run-positive-metrics.sh / run-negative-metrics.sh on both branches first." >&2
    exit 1
  fi
done

read_field() {
  local file="$1" field="$2"
  grep -o "\"${field}\": *[^,}]*" "$file" | head -n1 | sed -E "s/\"${field}\": *//"
}

echo "Metric                    | Positive     | Negative"
echo "---------------------------|--------------|-------------"
printf '%-27s| %-13s| %s\n' "Frontend line coverage" "$(read_field "$POSITIVE_SUMMARY" frontendLineCoverage)" "$(read_field "$NEGATIVE_SUMMARY" frontendLineCoverage)"
printf '%-27s| %-13s| %s\n' "Backend line coverage" "$(read_field "$POSITIVE_SUMMARY" backendLineCoverage)" "$(read_field "$NEGATIVE_SUMMARY" backendLineCoverage)"

POSITIVE_FAILURES="artifacts/positive/metrics/build-failures.txt"
NEGATIVE_FAILURES="artifacts/negative/metrics/build-failures.txt"
POSITIVE_FAIL_COUNT=0
NEGATIVE_FAIL_COUNT=0
[ -f "$POSITIVE_FAILURES" ] && POSITIVE_FAIL_COUNT="$(wc -l < "$POSITIVE_FAILURES")"
[ -f "$NEGATIVE_FAILURES" ] && NEGATIVE_FAIL_COUNT="$(wc -l < "$NEGATIVE_FAILURES")"
printf '%-27s| %-13s| %s\n' "Failing build steps" "$POSITIVE_FAIL_COUNT" "$NEGATIVE_FAIL_COUNT"

echo ""
echo "Full detail: artifacts/positive/ vs artifacts/negative/"
