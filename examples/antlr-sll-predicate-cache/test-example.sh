#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")"
dotnet_cmd=${DOTNET:-dotnet}

parse=../../src/trparse/bin/Release/net10.0/trparse.dll
interp=../../src/trinterp/bin/Release/net10.0/trinterp.dll
tree=../../src/trtree/bin/Release/net10.0/trtree.dll

parse_tree() {
    "$dotnet_cmd" "$parse" Nest.g4 |
        "$dotnet_cmd" "$interp" |
        "$dotnet_cmd" "$parse" --allstar "$@" examples/n10.txt |
        "$dotnet_cmd" "$tree" --text -a
}

sll_tree=$(parse_tree --sll)
ll_tree=$(parse_tree)
if [[ -z "$sll_tree" || "$sll_tree" != "$ll_tree" ]]; then
    printf 'ALL(*) SLL and LL trees differ for n10.txt.\nSLL: %s\nLL: %s\n' \
        "$sll_tree" "$ll_tree" >&2
    exit 1
fi

printf 'ALL(*) SLL and LL produce the same n10.txt tree.\n'
