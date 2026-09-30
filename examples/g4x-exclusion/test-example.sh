#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")"

output=$(bash run-example.sh)
[[ $(grep -c '^exclusion$' <<< "$output") -eq 2 ]]
grep -Fq 'ReservedKeyword' <<< "$output"
grep -Fq "'yield'" <<< "$output"
