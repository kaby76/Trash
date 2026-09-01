#!/usr/bin/env bash

# Demonstrate parser-directed token choice for overlapping Decaf integer rules.
set -euo pipefail
cd "$(dirname "$0")"

rm -rf interp result.pt result.tree ordinary.err

# Generate parser and lexer .interp files without compiling a target parser.
dotnet trash parse -l Decaf.g4 \
  | dotnet trash interp --atn -o interp

# Ordinary ANTLR lexing gives "10" to DECIMAL_LITERAL, so the parser cannot
# satisfy field_decl's INT_LITERAL transition. That failure is intentional.
set +e
dotnet trash parse --allstar -L interp input.decaf \
  > /dev/null 2> ordinary.err
ordinary_status=$?
set -e
if [[ $ordinary_status -eq 0 ]]; then
  echo "Expected ordinary ALL(*) lexing to reject input.decaf." >&2
  exit 1
fi

# Context-aware lexing sees that the parser expects INT_LITERAL and selects it
# over the earlier, equally long DECIMAL_LITERAL rule.
dotnet trash parse --context-aware-lexing -L interp input.decaf > result.pt
dotnet trash tree -f result.pt > result.tree

echo "Ordinary ALL(*) failed as expected; context-aware ALL(*) succeeded."
cat result.tree
