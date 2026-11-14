#!/usr/bin/env bash
set -euo pipefail
here="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
scratch="$(mktemp -d)"
trap 'rm -rf -- "$scratch"' EXIT
for grammar in ClassSyntaxDemo ModeDemo ParserOnly; do
  bash "$here/convert-antlr4.sh" "$here/$grammar.g4" > "$scratch/$grammar.txt"
  diff --strip-trailing-cr -u "$here/expected/$grammar.txt" "$scratch/$grammar.txt"
done
grep -q 'If before Identifier;' "$scratch/ClassSyntaxDemo.txt"
grep -q 'mode StringMode {' "$scratch/ModeDemo.txt"
grep -q 'tokenVocab = ModeDemo' "$scratch/ParserOnly.txt"
echo 'ANTLR4 class-layout conversions match expected output.' >&2
