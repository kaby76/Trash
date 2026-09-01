#!/usr/bin/env bash

set -euo pipefail
cd "$(dirname "$0")"

bash run-example.sh > /dev/null
diff --strip-trailing-cr -u expected.tree result.tree
grep -q "input rejected by grammar" ordinary.err
grep -q "DECIMAL_LITERAL / INT_LITERAL: 1" context.err
grep -q "ordinary winner: DECIMAL_LITERAL" context.err
grep -q "selected winner: INT_LITERAL" context.err
