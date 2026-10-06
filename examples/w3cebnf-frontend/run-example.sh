#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")"
root="$(cd ../.. && pwd)"

tree=$(dotnet "$root/src/trparse/bin/Release/net10.0/trparse.dll" -t W3CEBNF Message.ebnf |
    dotnet "$root/src/trinterp/bin/Release/net10.0/trinterp.dll" |
    dotnet "$root/src/trparse/bin/Release/net10.0/trparse.dll" --allstar input.txt |
    dotnet "$root/src/trtree/bin/Release/net10.0/trtree.dll" --text -a)
grep -q '(w3c_start ' <<<"$tree"
grep -q '(message ' <<<"$tree"
grep -q '(name ' <<<"$tree"

echo 'W3C EBNF grammar compiled and input parsed successfully.'
printf '%s\n' "$tree"
