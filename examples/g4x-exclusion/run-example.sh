#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")"
export MSYS2_ARG_CONV_EXCL='*'

mkdir -p interp
dotnet trash parse -t G4X SetDiffLexer.g4x SetDiffParser.g4x \
  | dotnet trash interp -o interp
dotnet trash parse --allstar -L interp \
  --pinterp SetDiffParser.interp --linterp SetDiffLexer.interp input.txt \
  | dotnet trash tree --text -a

dotnet trash parse -t G4X ContextLexer.g4x ContextParser.g4x \
  | dotnet trash interp -o interp
dotnet trash parse --allstar --context-aware-lexing -L interp \
  --pinterp ContextParser.interp --linterp ContextLexer.interp context-ok.txt \
  | dotnet trash tree --text -a
