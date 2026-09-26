#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")"

dotnet trash parse ArithmeticLexer.g4p ArithmeticParser.g4p |
    dotnet trash interp -o interp
dotnet trash parse --allstar -L interp input.txt |
    dotnet trash tree -a > result.tree

expected='(Start (Expression (Expression (Expression 1) + 2) + 3) <EOF>)'
actual=$(cat result.tree)
if [[ "$actual" != "$expected" ]]; then
    printf 'Unexpected tree:\n%s\n' "$actual" >&2
    exit 1
fi
printf '%s\n' "$actual"
