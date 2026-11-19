#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")"
jar="${CONGOCC_JAR:-$HOME/congo-parser-generator/bin/congocc.jar}"
if [[ ! -f "$jar" ]] || ! command -v java >/dev/null 2>&1 || ! command -v javac >/dev/null 2>&1; then
    echo "Skipping CongoCC VMF example test: CongoCC JAR and Java/Javac are required."
    exit 0
fi
output=$(bash run-example.sh 2>&1)
printf '%s\n' "$output"
[[ "$output" == *'CongoCC 0 ../simple.vmf success'* ]]
grep -q '^vmf :' Generated-CongoCC/vmf.ccc
prefixed_output=$(CONGOCC_RULE_PREFIX=ParserRule_ bash run-example.sh 2>&1)
printf '%s\n' "$prefixed_output"
[[ "$prefixed_output" == *'CongoCC 0 ../simple.vmf success'* ]]
grep -q '^ParserRule_vmf :' Generated-CongoCC/vmf.ccc
