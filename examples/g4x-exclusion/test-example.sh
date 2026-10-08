#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")"
export MSYS2_ARG_CONV_EXCL='*'

parse=../../src/trparse/bin/Release/net10.0/trparse.dll
interp=../../src/trinterp/bin/Release/net10.0/trinterp.dll
mkdir -p interp
dotnet "$parse" -t G4X SetDiffLexer.g4x SetDiffParser.g4x |
  dotnet "$interp" -o interp

tokens=$(dotnet "$parse" --allstar -L interp --tokens --no-output input.txt 2>&1)
[[ $(grep -c '<Keyword>' <<< "$tokens") -eq 2 ]]
[[ $(grep -c '<Identifier>' <<< "$tokens") -eq 4 ]]
[[ $(grep -c '<NullLiteral>' <<< "$tokens") -eq 1 ]]
grep -Fq "'if',<Keyword>" <<< "$tokens"
grep -Fq "'null',<NullLiteral>" <<< "$tokens"
grep -Fq "'class',<Keyword>" <<< "$tokens"
grep -Fq "'iffy',<Identifier>" <<< "$tokens"
grep -Fq "'nullify',<Identifier>" <<< "$tokens"
echo 'G4X set-difference example passed.'
