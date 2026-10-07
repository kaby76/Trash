#!/usr/bin/env bash
set -euo pipefail
export MSYS2_ARG_CONV_EXCL='*'
cd "$(dirname "$0")"

# Compile the target-neutral Java grammar into .interp tables.
mkdir -p interp data
dotnet trash parse -t ANTLRv4 JavaLexer.g4 JavaParser.g4 |
    dotnet trash interp -o interp

# This is the source archive for the jdk-21-ga tag, not a JDK binary archive.
archive=data/jdk-21-ga.tar.gz
if [[ ! -f "$archive" ]]; then
    curl --fail --location --retry 3 \
        'https://github.com/openjdk/jdk/archive/refs/tags/jdk-21-ga.tar.gz' \
        --output "$archive"
fi

root=data/jdk21
mkdir -p "$root"
if [[ ! -f "$root/.extracted" ]]; then
    tar -xzf "$archive" -C "$root" --strip-components=1
    touch "$root/.extracted"
fi

# Archive member paths now start at src/, so this glob selects Java sources
# without depending on GitHub's jdk-jdk-21-ga wrapper directory.
tar --format=pax -C "$root" -cf - src |
    dotnet trash parse --allstar -L interp --xquery-hooks hooks.json \
        --bundle-glob 'src/**/*.java' --per-file > data/jdk21-parsed.tar

echo 'Wrote data/jdk21-parsed.tar (original src files plus Java .pt/.errors sidecars).'
