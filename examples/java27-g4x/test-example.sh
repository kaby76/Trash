#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")"
export MSYS2_ARG_CONV_EXCL='*'

parse=../../src/trparse/bin/Release/net10.0/trparse.dll
interp=../../src/trinterp/bin/Release/net10.0/trinterp.dll

mkdir -p interp
dotnet "$parse" -t G4X Java27Lexer.g4x Java27Parser.g4x |
  dotnet "$interp" -o interp
grep -Fq 'contextAwareLexing=true' interp/Java27Parser.interp

result=$(dotnet "$parse" -L interp --indirect-left-recursion \
  --no-output --per-file \
  examples/helloworld.java \
  examples/AllInOne7.java \
  examples/AllInOne11.java \
  examples/AllInOne17.java \
  examples/module-info.java \
  examples/GenericConstructor.java 2>&1)
[[ $(grep -Fc ' success ' <<< "$result") -eq 6 ]]
[[ $(grep -Fc 'ALL(*) ' <<< "$result") -eq 6 ]]

# A rejected first file must not prevent the following input from parsing.
set +e
continued=$(dotnet "$parse" -L interp --indirect-left-recursion \
  --no-output --per-file \
  examples/Foo4391.java examples/helloworld.java 2>&1)
status=$?
set -e
[[ $status -ne 0 ]]
grep -Fq "parse failed for 'examples/Foo4391.java'" <<< "$continued"
grep -Fq 'ALL(*) 1 examples/helloworld.java success' <<< "$continued"
echo 'Java27 G4X example passed.'
