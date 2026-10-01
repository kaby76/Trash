#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")"

tree=$(dotnet trash parse -t ABNF Message.abnf | dotnet trash interp |
    dotnet trash parse --allstar input.txt | dotnet trash tree -a)
grep -q '(abnf_start ' <<<"$tree"
grep -q '(message ' <<<"$tree"
grep -q '(name ' <<<"$tree"

echo 'ABNF grammar compiled and input parsed successfully.'
printf '%s\n' "$tree"
