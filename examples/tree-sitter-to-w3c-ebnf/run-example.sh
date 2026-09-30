#!/usr/bin/env bash
set -euo pipefail

here="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
root="$(cd -- "$here/../.." && pwd)"
scratch="$(mktemp -d)"
trap 'rm -f -- "$scratch"/*; rmdir -- "$scratch"' EXIT

bash "$here/convert-tree-sitter.sh" "$here/MiniGrammar.json" \
  | sed 's/\r$//' > "$scratch/mini.ebnf"
diff -u "$here/expected.ebnf" "$scratch/mini.ebnf"
dotnet "$root/src/trparse/bin/Release/net10.0/trparse.dll" \
  -t W3CEBNF --no-output "$scratch/mini.ebnf"
echo 'Tree-sitter JSON converted and W3C EBNF reparsed successfully.' >&2
cat "$scratch/mini.ebnf"
