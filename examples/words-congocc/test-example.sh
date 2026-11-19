#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")"
jar="${CONGOCC_JAR:-$HOME/congo-parser-generator/bin/congocc.jar}"
if [[ ! -f "$jar" ]] || ! command -v java >/dev/null 2>&1 || ! command -v javac >/dev/null 2>&1; then
    echo "Skipping CongoCC example test: CongoCC JAR and Java/Javac are required."
    exit 0
fi
output=$(bash run-example.sh 2>&1)
printf '%s\n' "$output"
[[ "$output" == *'CongoCC 0 input success'* ]]
file_output=$(cd Generated-CongoCC && printf '%s\n' '../three-words.txt' | bash run.sh -x --per-file 2>&1)
printf '%s\n' "$file_output"
[[ "$file_output" == *'CongoCC 0 ../three-words.txt success'* ]]