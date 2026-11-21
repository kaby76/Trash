#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")"
repo_root=$(cd ../.. && pwd)
if [[ -f "$repo_root/src/trparse/bin/Release/net11.0/trparse.dll" ]]; then
    export TRPARSE_DLL="${TRPARSE_DLL:-$repo_root/src/trparse/bin/Release/net11.0/trparse.dll}"
fi
if [[ -f "$repo_root/src/trxquery/bin/Release/net11.0/trxquery.dll" ]]; then
    export TRXQUERY_DLL="${TRXQUERY_DLL:-$repo_root/src/trxquery/bin/Release/net11.0/trxquery.dll}"
fi
if [[ -z "${TRINTERP_DLL:-}" && -f "$repo_root/src/trinterp/bin/Release/net11.0/trinterp.dll" ]]; then
    export TRINTERP_DLL="$repo_root/src/trinterp/bin/Release/net11.0/trinterp.dll"
fi
trgen_dll="$repo_root/src/trgen/bin/Release/net11.0/trgen.dll"
if [[ -f "$trgen_dll" ]]; then
    dotnet "$trgen_dll" -t CongoCC
else
    dotnet trash gen -t CongoCC
fi
cd Generated-CongoCC
CONGOCC_RULE_PREFIX=ParserRule_ bash build.sh
bash run.sh --per-file ../simple.txt
