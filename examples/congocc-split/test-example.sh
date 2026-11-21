#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")"
jar="${CONGOCC_JAR:-$HOME/congo-parser-generator/bin/congocc.jar}"
if [[ ! -f "$jar" ]] || ! command -v java >/dev/null 2>&1 || ! command -v javac >/dev/null 2>&1; then
    echo 'Skipping CongoCC split example test: CongoCC JAR and Java/Javac are required.'
    exit 0
fi
if ! output=$(bash run-example.sh 2>&1); then
    printf '%s\n' "$output"
    exit 1
fi
printf '%s\n' "$output"
[[ $(printf '%s\n' "$output" | grep -c 'CongoCC 0 .* success') -eq 2 ]]
grep -q '^IGNORE_CASE=true;' Generated-CongoCC/Split.ccc
grep -q '^document : (SCAN ' Generated-CongoCC/Split.ccc
grep -q '^value : SCAN ' Generated-CongoCC/Split.ccc
! printf '%s\n' "$output" | grep -q 'Expansion is unreachable'
