#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")"
repo_root=$(cd ../.. && pwd)
# Use the just-built trgen when working from a checkout.
trgen_dll="$repo_root/src/trgen/bin/Release/net11.0/trgen.dll"
if [[ -f "$trgen_dll" ]]; then
    dotnet "$trgen_dll" -t CongoCC
else
    dotnet trash gen -t CongoCC
fi
cd Generated-CongoCC
bash build.sh
bash run.sh --input 'red , green , blue' --per-file
