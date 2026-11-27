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
grep -q '^SKIP : <COMMENT :' Generated-CongoCC/Split.ccc
grep -Fq 'SKIP : <?BLOCK_COMMENT : "/*" (~[])* "*/"> ;' Generated-CongoCC/Split.ccc
grep -q '^document : (SCAN ' Generated-CongoCC/Split.ccc
grep -q '^value : SCAN ' Generated-CongoCC/Split.ccc
! printf '%s\n' "$output" | grep -q 'Expansion is unreachable'
tree_output=$(cd Generated-CongoCC && bash run.sh -tree --no-output --input '+1, -2.5;' 2>&1)
[[ "$tree_output" == '(document (value + 1) , (value - 2.5) ; <EOF>)' ]]
newline_tree=$(cd Generated-CongoCC && bash run.sh -tree --no-output --input $'+1, -2.5;\n' 2>&1)
[[ "$newline_tree" == '(document (value + 1) , (value - 2.5) ; \n <EOF>)' ]]
