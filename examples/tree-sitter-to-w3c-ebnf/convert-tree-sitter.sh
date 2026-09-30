#!/usr/bin/env bash
# Usage: bash convert-tree-sitter.sh path/to/grammar.json > grammar.ebnf
set -euo pipefail

if [[ $# -ne 1 || ! -f "$1" ]]; then
  echo 'Usage: bash convert-tree-sitter.sh path/to/grammar.json' >&2
  exit 2
fi

here="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
root="$(cd -- "$here/../.." && pwd)"
input="$(cd -- "$(dirname -- "$1")" && pwd)/$(basename -- "$1")"
scratch="$(mktemp -d)"
trap 'rm -f -- "$scratch"/*; rmdir -- "$scratch"' EXIT

dotnet "$root/src/trparse/bin/Release/net10.0/trparse.dll" "$here/JSON.g4" \
  | dotnet "$root/src/trinterp/bin/Release/net10.0/trinterp.dll" -o "$scratch"

dotnet "$root/src/trparse/bin/Release/net10.0/trparse.dll" --allstar -L "$scratch" "$input" \
  | dotnet "$root/src/trxquery/bin/Release/net10.0/trxquery.dll" -q "$here/convert.xq"
