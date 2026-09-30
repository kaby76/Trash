#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")"

scratch="$(mktemp -d ./abnf-frontend-test-XXXXXXXX)"
trap 'rm -f -- "$scratch"/Abnf_Message.interp "$scratch"/Abnf_Message.tokens "$scratch"/Abnf_MessageLexer.interp "$scratch"/Abnf_MessageLexer.tokens "$scratch"/output.tree; rmdir -- "$scratch"' EXIT

dotnet trash parse -t ABNF Message.abnf | dotnet trash interp -o "$scratch"
dotnet trash parse --allstar -L "$scratch" input.txt | dotnet trash tree -a >"$scratch/output.tree"

grep -q '(abnf_start ' "$scratch/output.tree"
grep -q '(message ' "$scratch/output.tree"
grep -q '(name ' "$scratch/output.tree"

echo 'ABNF grammar compiled and input parsed successfully.'
cat "$scratch/output.tree"
