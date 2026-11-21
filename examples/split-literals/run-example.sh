#!/usr/bin/env bash
set -euo pipefail
export MSYS2_ARG_CONV_EXCL='*'
cd "$(dirname "$0")"

parse='../../src/trparse/bin/Release/net11.0/trparse.dll'
split='../../src/trsplit/bin/Release/net11.0/trsplit.dll'
query='../../src/trxquery/bin/Release/net11.0/trxquery.dll'
sponge='../../src/trsponge/bin/Release/net11.0/trsponge.dll'
interp='../../src/trinterp/bin/Release/net11.0/trinterp.dll'

scratch="$(mktemp -d ./split-literals-XXXXXXXX)"
trap 'case "$scratch" in ./split-literals-*) rm -rf -- "$scratch" ;; esac' EXIT

dotnet "$parse" SplitLiterals.g4 \
  | dotnet "$query" -q declare-literals.xq \
  | dotnet "$split" \
  | dotnet "$sponge" -o "$scratch/rewritten"

grep -q "TRSPLIT_LITERAL_1 : 'on' ;" "$scratch/rewritten/SplitLiteralsLexer.g4"
grep -q "TRSPLIT_LITERAL_2 : 'end' ;" "$scratch/rewritten/SplitLiteralsLexer.g4"

dotnet "$parse" "$scratch/rewritten/SplitLiteralsLexer.g4" \
                "$scratch/rewritten/SplitLiteralsParser.g4" \
  | dotnet "$interp" \
  | dotnet "$parse" --allstar --no-output input.txt

dotnet "$parse" SplitLiteralsWithExisting.g4 \
  | dotnet "$query" -q declare-literals.xq \
  | dotnet "$split" \
  | dotnet "$sponge" -o "$scratch/existing"
grep -q "TRSPLIT_LITERAL__1 : 'end' ;" \
  "$scratch/existing/SplitLiteralsWithExistingLexer.g4"
test "$(grep -c ": 'on' ;" "$scratch/existing/SplitLiteralsWithExistingLexer.g4")" -eq 1
dotnet "$parse" "$scratch/existing/SplitLiteralsWithExistingLexer.g4" \
                "$scratch/existing/SplitLiteralsWithExistingParser.g4" \
  | dotnet "$interp" \
  | dotnet "$parse" --allstar --no-output input.txt

echo 'Split grammar with declared literals parsed input.txt.'
