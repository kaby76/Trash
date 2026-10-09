#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")"
export MSYS2_ARG_CONV_EXCL='*'

parse=../../src/trparse/bin/Release/net10.0/trparse.dll
interp=../../src/trinterp/bin/Release/net10.0/trinterp.dll
mkdir -p interp
dotnet "$parse" -t G4X SetDiffLexer.g4x SetDiffParser.g4x |
  dotnet "$interp" -o interp

tokens=$(dotnet "$parse" --allstar -L interp \
  --pinterp SetDiffParser.interp --linterp SetDiffLexer.interp \
  --tokens --no-output input.txt 2>&1)
[[ $(grep -c '<Keyword>' <<< "$tokens") -eq 2 ]]
[[ $(grep -c '<Identifier>' <<< "$tokens") -eq 4 ]]
[[ $(grep -c '<NullLiteral>' <<< "$tokens") -eq 1 ]]
grep -Fq "'if',<Keyword>" <<< "$tokens"
grep -Fq "'null',<NullLiteral>" <<< "$tokens"
grep -Fq "'class',<Keyword>" <<< "$tokens"
grep -Fq "'iffy',<Identifier>" <<< "$tokens"
grep -Fq "'nullify',<Identifier>" <<< "$tokens"

dotnet "$parse" -t G4X ContextLexer.g4x ContextParser.g4x |
  dotnet "$interp" -o interp
dotnet "$parse" -L interp \
  --pinterp ContextParser.interp --linterp ContextLexer.interp \
  --no-output context-ok.txt > /dev/null
rejected=$(dotnet "$parse" -L interp \
  --pinterp ContextParser.interp --linterp ContextLexer.interp \
  --no-output context-excluded.txt 2>&1) || true
grep -Fq "input rejected by grammar" <<< "$rejected"
echo 'G4X set-difference example passed.'
