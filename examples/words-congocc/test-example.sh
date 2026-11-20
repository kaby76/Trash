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
[[ $(printf '%s\n' "$output" | grep -c 'CongoCC 0 input success') -eq 2 ]]
grep -q '^wordGroup : SCAN ' Generated-CongoCC/Words.ccc
file_output=$(cd Generated-CongoCC && printf '%s\n' '../three-words.txt' | bash run.sh -x --per-file 2>&1)
printf '%s\n' "$file_output"
[[ "$file_output" == *'CongoCC 0 ../three-words.txt success'* ]]

# Reversing the alternatives must not change which inputs the parser accepts.
cat > Generated-CongoCC/Words.g4 <<'EOF'
grammar Words;
document : wordGroup EOF;
wordGroup : WORD COMMA WORD | WORD COMMA WORD COMMA WORD;
WORD : [a-zA-Z]+;
COMMA : ',';
WS : [ \t\r\n]+ -> skip;
EOF
repo_root=$(cd ../.. && pwd)
reversed_output=$(
  (
    cd Generated-CongoCC
    TRPARSE_DLL="$repo_root/src/trparse/bin/Release/net11.0/trparse.dll" \
    TRXQUERY_DLL="$repo_root/src/trxquery/bin/Release/net11.0/trxquery.dll" \
    TRINTERP_DLL="$repo_root/src/trinterp/bin/Release/net11.0/trinterp.dll" \
      bash build.sh >/dev/null
    bash run.sh --input 'red , green' --per-file
    bash run.sh --input 'red , green , blue' --per-file
  ) 2>&1
)
printf '%s\n' "$reversed_output"
[[ $(printf '%s\n' "$reversed_output" | grep -c 'CongoCC 0 input success') -eq 2 ]]
grep -q '^wordGroup : SCAN ' Generated-CongoCC/Words.ccc
