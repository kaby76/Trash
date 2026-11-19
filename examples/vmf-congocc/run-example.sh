#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")"
repo_root=$(cd ../.. && pwd)
trgen_dll="$repo_root/src/trgen/bin/Release/net11.0/trgen.dll"
if [[ -f "$trgen_dll" ]]; then
    dotnet "$trgen_dll" -t CongoCC
else
    dotnet trash gen -t CongoCC
fi
cd Generated-CongoCC
bash build.sh
bash run.sh --per-file ../simple.vmf
