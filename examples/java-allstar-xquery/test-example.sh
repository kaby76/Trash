#!/usr/bin/env bash
set -euo pipefail
export MSYS2_ARG_CONV_EXCL='*'
cd "$(dirname "$0")"

parse=../../src/trparse/bin/Release/net10.0/trparse.dll
interp=../../src/trinterp/bin/Release/net10.0/trinterp.dll
mkdir -p interp
dotnet "$parse" -t ANTLRv4 JavaLexer.g4 JavaParser.g4 |
    dotnet "$interp" -o interp

dotnet "$parse" --allstar -L interp --xquery-hooks hooks.json \
    --no-output input.java
if dotnet "$parse" --allstar -L interp --xquery-hooks hooks.json \
    --no-output invalid-record.java >/dev/null 2>&1; then
    echo 'Expected a non-final varargs record component to be rejected.' >&2
    exit 1
fi

bundle=$(mktemp)
archive=$(mktemp)
trap 'rm -f "$bundle" "$archive"' EXIT
tar --format=pax -C fixtures \
    --transform='s|^src|jdk-jdk-21-ga/src|' -czf "$archive" src
gzip -dc "$archive" |
    dotnet "$parse" --allstar -L interp --xquery-hooks hooks.json \
        --bundle-glob 'jdk-jdk-21-ga/src/**/*.java' > "$bundle"
tar -tf "$bundle" | grep -qx 'jdk-jdk-21-ga/src/TaggedRecord.java.pt'
tar -tf "$bundle" | grep -qx 'jdk-jdk-21-ga/src/TaggedRecord.java.errors'
echo 'Java AllStar/XQuery smoke tests passed.'
