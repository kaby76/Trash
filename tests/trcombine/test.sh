#!/usr/bin/env bash
set -euo pipefail
export MSYS2_ARG_CONV_EXCL='*'
cd "$(dirname "$0")"

parse='../../src/trparse/bin/Release/net10.0/trparse.dll'
combine='../../src/trcombine/bin/Release/net10.0/trcombine.dll'
text='../../src/trtext/bin/Release/net10.0/trtext.dll'
scratch="$(mktemp -d /tmp/trcombine-test-XXXXXXXX)"
trap 'case "$scratch" in /tmp/trcombine-test-*) rm -rf -- "$scratch" ;; esac' EXIT

check_pair() {
    local parser=$1 lexer=$2 name=$3
    dotnet "$parse" "$parser" "$lexer" 2>"$scratch/parse.log" \
        | dotnet "$combine" >"$scratch/result.tar"
    tar -tf "$scratch/result.tar" >"$scratch/members"
    diff -u <(printf '%s\n' "$name.g4.pt" "$name.g4.errors") "$scratch/members"
    dotnet "$text" <"$scratch/result.tar" >"$scratch/combined.g4"
    grep -q "^grammar $name;" "$scratch/combined.g4"
    ! grep -q 'tokenVocab\|parser grammar\|lexer grammar' "$scratch/combined.g4"
    dotnet "$parse" "$(cygpath -w "$scratch/combined.g4")" >/dev/null 2>"$scratch/reparse.log"
    ! grep -q 'error\|Exception' "$scratch/reparse.log"
}

check_pair Gold/ArithmeticParser.g4 Gold/ArithmeticLexer.g4 Arithmetic
grep -q '^file_ :' "$scratch/combined.g4"
grep -q '^fragment SIGN :' "$scratch/combined.g4"

# Source grammar artifacts must not survive the combine, but unrelated members do.
mkdir "$scratch/raw"
dotnet "$parse" Gold/ArithmeticParser.g4 Gold/ArithmeticLexer.g4 \
    >"$scratch/raw.tar" 2>"$scratch/parse.log"
tar -xf "$scratch/raw.tar" -C "$scratch/raw"
cp Gold/ArithmeticParser.g4 Gold/ArithmeticLexer.g4 "$scratch/raw/"
printf 'keep me\n' >"$scratch/raw/notes.txt"
tar -cf "$scratch/augmented.tar" -C "$scratch/raw" \
    ArithmeticParser.g4.pt ArithmeticParser.g4.errors ArithmeticParser.g4 \
    ArithmeticLexer.g4.pt ArithmeticLexer.g4.errors ArithmeticLexer.g4 notes.txt
dotnet "$combine" <"$scratch/augmented.tar" >"$scratch/filtered.tar"
tar -tf "$scratch/filtered.tar" | sort >"$scratch/members"
diff -u <(printf '%s\n' Arithmetic.g4.errors Arithmetic.g4.pt notes.txt) "$scratch/members"
test "$(tar -xOf "$scratch/filtered.tar" notes.txt)" = 'keep me'

check_pair OptionsLexer.g4 OptionsParser.g4 Options
grep -q 'language=CSharp' "$scratch/combined.g4"
grep -q '^WS :' "$scratch/combined.g4"

echo 'trcombine regression tests passed.'
