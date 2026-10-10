#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")"
export MSYS2_ARG_CONV_EXCL='*'

# The parser grammar requests context-aware lexing in its options block.
# trparse therefore chooses ALL(*) without either lexer-mode command-line flag.
mkdir -p interp
dotnet trash parse -t G4X Java27Lexer.g4x Java27Parser.g4x |
  dotnet trash interp -o interp

dotnet trash parse -L interp --indirect-left-recursion examples/helloworld.java |
  dotnet trash tree --text -a
printf '\n'

dotnet trash parse -L interp --indirect-left-recursion --no-output --per-file \
  examples/AllInOne7.java examples/AllInOne11.java examples/AllInOne17.java
