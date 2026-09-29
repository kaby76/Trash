#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")"

scratch="$(mktemp -d ./bison-frontend-test-XXXXXXXX)"
trap 'rm -f -- "$scratch"/Bison_Message.interp "$scratch"/Bison_Message.tokens "$scratch"/MessageLexer.interp "$scratch"/MessageLexer.tokens "$scratch"/output.tree; rmdir -- "$scratch"' EXIT

dotnet trash parse Message.y MessageLexer.g4 | dotnet trash interp -o "$scratch"
dotnet trash parse --allstar -L "$scratch" \
  --pinterp Bison_Message.interp --linterp MessageLexer.interp input.txt \
  | dotnet trash tree -a >"$scratch/output.tree"

grep -q '(bison_start (message Ada !) <EOF>)' "$scratch/output.tree"
echo 'Bison grammar and ANTLR4 lexer compiled and input parsed successfully.'
cat "$scratch/output.tree"
