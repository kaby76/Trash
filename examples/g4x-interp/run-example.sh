#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")"

expected='(Start (Expression (Expression (Expression 1) + 2) + 3) <EOF>)'
actual=$(dotnet trash parse ArithmeticLexer.g4x ArithmeticParser.g4x |
    dotnet trash interp |
    dotnet trash parse --allstar input.txt |
    dotnet trash tree -a)
if [[ "$actual" != "$expected" ]]; then
    printf 'Unexpected tree:\n%s\n' "$actual" >&2
    exit 1
fi
printf '%s\n' "$actual"
