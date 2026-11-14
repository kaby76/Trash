#!/usr/bin/env bash
# Convert one ANTLR4 .g4 file to the proposed class-like layout on stdout.
set -euo pipefail
if [[ $# -ne 1 || ! -f "$1" ]]; then
  echo 'Usage: bash convert-antlr4.sh path/to/Grammar.g4' >&2
  exit 2
fi
here="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
input="$(cd -- "$(dirname -- "$1")" && pwd)/$(basename -- "$1")"
cd "$here"
dotnet trash parse -t ANTLRv4 "$input" \
  | dotnet trash xquery -q "$here/convert.xq"
