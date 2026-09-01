#!/usr/bin/env bash

set -euo pipefail
cd "$(dirname "$0")"

actual=$(mktemp)
trap 'rm -f "$actual"' EXIT

bash run-example.sh > "$actual"
diff --strip-trailing-cr -u expected.txt "$actual"
