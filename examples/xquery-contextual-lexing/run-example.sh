#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")"

mkdir -p interp
dotnet trash parse -t ANTLRv4 FooLexer.g4 FooParser.g4 \
  | dotnet trash interp -o interp

dotnet trash parse --allstar -L interp --xquery-hooks hooks.json \
  --tokens --no-output input.txt 2>&1 \
  | sed 's/\r$//' \
  | grep '^\[@' \
  | grep -v 'channel=' \
  | sed -E "s/.*='([^']*)',<([^>]*)>.*/\2: \1/; s/^-1:/EOF:/"
