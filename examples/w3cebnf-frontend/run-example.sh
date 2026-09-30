#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")"
root="$(cd ../.. && pwd)"

scratch="$(mktemp -d ./w3cebnf-frontend-test-XXXXXXXX)"
trap 'rm -f -- "$scratch"/W3Cebnf_Message.interp "$scratch"/W3Cebnf_Message.tokens "$scratch"/W3Cebnf_MessageLexer.interp "$scratch"/W3Cebnf_MessageLexer.tokens "$scratch"/output.tree; rmdir -- "$scratch"' EXIT

dotnet "$root/src/trparse/bin/Release/net10.0/trparse.dll" -t W3CEBNF Message.ebnf \
  | dotnet "$root/src/trinterp/bin/Release/net10.0/trinterp.dll" -o "$scratch"
dotnet "$root/src/trparse/bin/Release/net10.0/trparse.dll" --allstar -L "$scratch" input.txt \
  | dotnet "$root/src/trtree/bin/Release/net10.0/trtree.dll" -a >"$scratch/output.tree"

grep -q '(w3c_start ' "$scratch/output.tree"
grep -q '(message ' "$scratch/output.tree"
grep -q '(name ' "$scratch/output.tree"

echo 'W3C EBNF grammar compiled and input parsed successfully.'
cat "$scratch/output.tree"
