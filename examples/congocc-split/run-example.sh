#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")"
repo_root=$(cd ../.. && pwd)
for tool in trparse trinterp trxquery trcombine; do
    dll="$repo_root/src/$tool/bin/Release/net11.0/$tool.dll"
    if [[ -f "$dll" ]]; then
        var="${tool^^}_DLL"
        printf -v "$var" '%s' "$dll"
        export "$var"
    fi
done
trgen_dll="$repo_root/src/trgen/bin/Release/net11.0/trgen.dll"
if [[ -f "$trgen_dll" ]]; then
    dotnet "$trgen_dll" -t CongoCC --force
else
    dotnet trash gen -t CongoCC --force
fi
cd Generated-CongoCC
bash build.sh
bash run.sh --per-file ../input.txt
bash run.sh --input '+3;' --per-file
