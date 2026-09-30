#!/usr/bin/env bash

set -euo pipefail
export MSYS2_ARG_CONV_EXCL='*'
cd "$(dirname "$0")"

parse='../../src/trparse/bin/Release/net10.0/trparse.dll'
combine='../../src/trcombine/bin/Release/net10.0/trcombine.dll'
text='../../src/trtext/bin/Release/net10.0/trtext.dll'
sponge='../../src/trsponge/bin/Release/net10.0/trsponge.dll'

scratch="$(mktemp -d /tmp/trcombine-test-XXXXXXXX)"
trap 'case "$scratch" in /tmp/trcombine-test-*) rm -rf -- "$scratch" xxx ;; esac' EXIT

dotnet "$parse" ArithmeticLexer.g4 ArithmeticParser.g4 |
	dotnet "$combine" |
	dotnet "$sponge" -o xxx -c

diff -r xxx Gold

echo 'trcombine regression tests passed.'
