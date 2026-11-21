#!/usr/bin/env bash
set -euo pipefail
export MSYS2_ARG_CONV_EXCL='*'
cd "$(dirname "$0")"

parse='../../src/trparse/bin/Release/net11.0/trparse.dll'
query='../../src/trxquery/bin/Release/net11.0/trxquery.dll'
sponge='../../src/trsponge/bin/Release/net11.0/trsponge.dll'
interp='../../src/trinterp/bin/Release/net11.0/trinterp.dll'

scratch="$(mktemp -d ./case-fold-XXXXXXXX)"
trap 'case "$scratch" in ./case-fold-*) rm -rf -- "$scratch" ;; esac' EXIT

dotnet "$parse" FoldLexer.g4 \
  | dotnet "$query" -q fold-ascii-case.xq \
  | dotnet "$sponge" -o "$scratch"

grep -q 'SELECT : \[Ss\]\[eE\]' "$scratch/FoldLexer.g4"
grep -q 'HEX : \[A-F0-9a-f\]' "$scratch/FoldLexer.g4"
grep -q 'RANGE :\[G-Zg-z\]' "$scratch/FoldLexer.g4"
grep -q 'NOT_LETTER : .\+ ~\[A-Za-z\]' "$scratch/FoldLexer.g4"
if grep -q 'caseInsensitive' "$scratch/FoldLexer.g4"; then
  echo 'caseInsensitive option was not removed' >&2
  exit 1
fi

dotnet "$parse" "$scratch/FoldLexer.g4" FoldParser.g4 \
  | dotnet "$interp" \
  | dotnet "$parse" --allstar --no-output input.txt

echo 'ASCII case-folded lexer parsed input.txt.'
