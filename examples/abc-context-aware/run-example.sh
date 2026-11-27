#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")"

repo_root=$(cd ../.. && pwd)
if command -v cygpath >/dev/null 2>&1; then
    repo_root=$(cygpath -m "$repo_root")
fi
parse="$repo_root/src/trparse/bin/Release/net11.0/trparse.dll"
interp="$repo_root/src/trinterp/bin/Release/net11.0/trinterp.dll"
query="$repo_root/src/trxquery/bin/Release/net11.0/trxquery.dll"
sponge="$repo_root/src/trsponge/bin/Release/net11.0/trsponge.dll"
gen="$repo_root/src/trgen/bin/Release/net11.0/trgen.dll"
export TRPARSE_DLL="$parse" TRINTERP_DLL="$interp" TRXQUERY_DLL="$query"
export TRCOMBINE_DLL="$repo_root/src/trcombine/bin/Release/net11.0/trcombine.dll"

scratch=$(mktemp -d ./abc-mode-free-XXXXXXXX)
cleanup() {
    rm -f -- "$scratch/abcLexer.g4" "$scratch/abcLexer.g4.errors"
    rmdir -- "$scratch" 2>/dev/null || true
}
trap cleanup EXIT

# Rewrite the original mode-based lexer through its parse tree.
dotnet "$parse" -t ANTLRv4 source/abcLexer.g4 \
    | dotnet "$query" -q rewrite-abc-modes.xq \
    | dotnet "$sponge" -o "$scratch"
test -f "$scratch/abcLexer.g4"
if grep -Eq 'mode BODY|mode\(BODY\)|type\(FIELD\)|type\(XFIELD\)' "$scratch/abcLexer.g4"; then
    echo 'ABC lexer mode rewrite was incomplete.' >&2
    exit 1
fi

# The rewritten lexer has no modes. Parser-directed token selection accepts
# fields in the header and music tokens in the body.
result=$(
    dotnet "$parse" -t ANTLRv4 "$scratch/abcLexer.g4" abcParser.g4 \
        | dotnet "$interp" \
        | dotnet "$parse" --allstar --context-aware-lexing \
            --no-output --per-file examples/*.abc 2>&1
)
printf '%s\n' "$result"
[[ $(printf '%s\n' "$result" | grep -c ' success ') -eq 14 ]]
ordinary=$(
    dotnet "$parse" -t ANTLRv4 "$scratch/abcLexer.g4" abcParser.g4 \
        | dotnet "$interp" \
        | dotnet "$parse" --allstar --no-output --per-file examples/*.abc 2>&1
)
[[ $(printf '%s\n' "$ordinary" | grep -c ' success ') -eq 14 ]]
echo 'Ordinary AllStar also accepted all 14 ABC inputs.'
cleanup
trap - EXIT

jar="${CONGOCC_JAR:-$HOME/congo-parser-generator/bin/congocc.jar}"
if [[ ! -f "$jar" ]] || ! command -v java >/dev/null 2>&1 ||
    ! command -v javac >/dev/null 2>&1; then
    echo 'Skipping CongoCC checks: CongoCC JAR and Java/Javac are required.'
    exit 0
fi

# The checked-in mode-free lexer also works with CongoCC. The generated build
# chooses a prefix for parser rules such as 'rest' that collide with REST.
dotnet "$gen" -t CongoCC --force
(
    cd Generated-CongoCC
    bash build.sh
    grep -Fxq 'ParserRule_' parser-rule-prefix.txt
    make test
)

# CongoCC can also preserve the original lexer modes as lexical states.
(
    cd source
    dotnet "$gen" -t CongoCC --force
    cd Generated-CongoCC
    bash build.sh
    grep -Fxq 'ParserRule_' parser-rule-prefix.txt
    make test
    grep -Fq '<BODY> TOKEN : <BFIELD :' abc.ccc
    grep -Fq 'TOKEN : <KFIELD :' abc.ccc
)
