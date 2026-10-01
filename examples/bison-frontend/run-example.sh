#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")"

tree=$(dotnet trash parse Message.y MessageLexer.g4 | dotnet trash interp |
    dotnet trash parse --allstar --pinterp Bison_Message.interp \
        --linterp MessageLexer.interp input.txt | dotnet trash tree -a)
grep -q '(bison_start (message Ada !) <EOF>)' <<<"$tree"
echo 'Bison grammar and ANTLR4 lexer compiled and input parsed successfully.'
printf '%s\n' "$tree"
