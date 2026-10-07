#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")"

scratch="$(mktemp -d ./antlr4-sort-test-XXXXXXXX)"
trap 'rm -f -- "$scratch/result.tar" "$scratch/sorted.g4"; rmdir -- "$scratch"' EXIT

dotnet trash parse SortDemo.g4 \
  | dotnet trash xquery -q sort-antlr4.xq \
  | dotnet trash text >"$scratch/result.tar"
tar -xOf "$scratch/result.tar" SortDemo.g4 >"$scratch/sorted.g4"

awk '
  /^start[[:space:]]*:/{start=NR}
  /^alpha[[:space:]]*:/{alpha=NR}
  /^beta[[:space:]]*:/{beta=NR}
  /^zeta[[:space:]]*:/{zeta=NR}
  END {exit !(alpha && alpha < beta && beta < start && start < zeta)}
' "$scratch/sorted.g4"
grep -q '^start : zeta alpha beta EOF ;' "$scratch/sorted.g4"
grep -q '^parser grammar SortDemo;' "$scratch/sorted.g4"
grep -q 'tokenVocab=SortDemoLexer' "$scratch/sorted.g4"
grep -q 'Keep this comment with alpha' "$scratch/sorted.g4"

dotnet trash parse "$scratch/sorted.g4" >/dev/null
echo 'ANTLR4 parser rules sorted and the resulting grammar reparsed successfully.'

cat "$scratch/sorted.g4"
