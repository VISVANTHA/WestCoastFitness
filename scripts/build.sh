#!/usr/bin/env bash
# Branch-aware build entry point. Detects the current git branch and
# dispatches to build-positive.sh or build-negative.sh.
set -uo pipefail
cd "$(dirname "${BASH_SOURCE[0]}")/.."
source scripts/lib/common.sh

branch="$(current_branch)"
variant="$(branch_variant "$branch")"

case "$variant" in
  positive)
    exec bash ./scripts/build-positive.sh
    ;;
  negative)
    exec bash ./scripts/build-negative.sh
    ;;
  *)
    echo "The benchmark build must be run from the WestCoastFitness-positive or WestCoastFitness-negative branch (current branch: '${branch}')." >&2
    exit 1
    ;;
esac
