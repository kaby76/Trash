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

# GitHub's tarball has a jdk-jdk-21-ga/ wrapper directory. Parse its Java
# source members directly without extracting or repacking the archive.
gzip -dc "$archive" |
    dotnet trash parse --allstar -L interp --xquery-hooks hooks.json \
        --bundle-glob 'jdk-jdk-21-ga/src/**/*.java' --per-file \
        > data/jdk21-parsed.tar

echo 'Wrote data/jdk21-parsed.tar (original archive plus Java .pt/.errors sidecars).'
