#!/usr/bin/env bash
set -euo pipefail
export MSYS2_ARG_CONV_EXCL='*'
cd "$(dirname "$0")"

parse='../../src/trparse/bin/Release/net10.0/trparse.dll'
rename='../../src/trrename/bin/Release/net10.0/trrename.dll'
text='../../src/trtext/bin/Release/net10.0/trtext.dll'
scratch="$(mktemp -d /tmp/trrename-test-XXXXXXXX)"
map_file="$(mktemp ./trrename-map-XXXXXXXX)"
trap 'rm -f -- "$map_file"; case "$scratch" in /tmp/trrename-test-*) rm -rf -- "$scratch" ;; esac' EXIT

dotnet "$parse" Expression.g4 >"$scratch/input.tar" 2>"$scratch/parse.log"
dotnet "$rename" -r 'e,exp;a,atom;INT,Int;MUL,OpMul;DIV,OpDiv;ADD,OpAdd;SUB,OpSub' \
    <"$scratch/input.tar" >"$scratch/result.tar"
dotnet "$text" <"$scratch/result.tar" >"$scratch/text.tar"
tar -xOf "$scratch/text.tar" Expression.g4 >"$scratch/Expression.g4"
diff -u Gold/Expression.g4 "$scratch/Expression.g4"
diff -u <(printf '%s\n' Expression.g4 Expression.g4.errors) \
    <(tar -tf "$scratch/text.tar")

# A custom selection may update only parser references, not lexer declarations.
dotnet "$rename" -e '//parserRuleSpec//TOKEN_REF' -r 'INT,NewInt' \
    <"$scratch/input.tar" | dotnet "$text" >"$scratch/selected.tar"
tar -xOf "$scratch/selected.tar" Expression.g4 >"$scratch/selected.g4"
grep -q '^a : NewInt ;' "$scratch/selected.g4"
grep -q '^INT :' "$scratch/selected.g4"

# Mappings apply once to original spellings, not in sequence.
dotnet "$rename" -r 'e,a;a,b' <"$scratch/input.tar" \
    | dotnet "$text" >"$scratch/noncascading.tar"
tar -xOf "$scratch/noncascading.tar" Expression.g4 >"$scratch/noncascading.g4"
grep -q '^a : a ' "$scratch/noncascading.g4"
grep -q '^b : INT' "$scratch/noncascading.g4"

printf 'e,exp\na,atom\n' >"$map_file"
dotnet "$rename" -R "$map_file" <"$scratch/input.tar" \
    | dotnet "$text" >"$scratch/mapfile.tar"
tar -xOf "$scratch/mapfile.tar" Expression.g4 | grep -q '^atom : INT'

echo 'trrename regression tests passed.'
