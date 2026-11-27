#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")"
jar="${CONGOCC_JAR:-$HOME/congo-parser-generator/bin/congocc.jar}"
if [[ ! -f "$jar" ]] || ! command -v java >/dev/null 2>&1 || ! command -v javac >/dev/null 2>&1; then
    echo 'Skipping CongoCC rule-boundary test: CongoCC JAR and Java/Javac are required.'
    exit 0
fi
output=$(bash run-example.sh 2>&1) || {
    printf '%s\n' "$output"
    exit 1
}
printf '%s\n' "$output"
grep -Fq 'contents : (SCAN (ENSURE ~(<ID> "=") item) => item)+ ;' Generated-CongoCC/RuleBoundary.ccc
grep -Fq 'SKIP : <WS :' Generated-CongoCC/RuleBoundary.ccc
[[ "$output" == *'CongoCC 0 ../input.txt success'* ]]
test_output=$(cd Generated-CongoCC && make test 2>&1)
printf '%s\n' "$test_output"
[[ "$test_output" == *'Test succeeded.'* ]]
