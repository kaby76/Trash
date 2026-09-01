#!/usr/bin/env bash

set -euo pipefail
cd "$(dirname "$0")"

bash run-example.sh > /dev/null
diff --strip-trailing-cr -u expected.tree result.tree
grep -q "input rejected by grammar" ordinary.err
