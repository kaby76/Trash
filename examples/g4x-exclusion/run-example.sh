#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")"
export MSYS2_ARG_CONV_EXCL='*'

mkdir -p interp
dotnet trash parse -t G4X SetDiffLexer.g4x SetDiffParser.g4x \
  | dotnet trash interp -o interp
dotnet trash parse --allstar -L interp input.txt \
  | dotnet trash tree --text -a
