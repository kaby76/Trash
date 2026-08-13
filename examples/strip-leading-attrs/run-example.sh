#!/bin/bash
# Strip hidden-channel token attributes that precede the first real keyword
# inside grammarType, then display the cleaned grammarDecl subtree.
#
# Usage: bash run-example.sh

set -e
trap 'LAST_COMMAND=$CURRENT_COMMAND; CURRENT_COMMAND=$BASH_COMMAND' DEBUG
trap 'ERROR_CODE=$?; FAILED_COMMAND=$LAST_COMMAND; tput setaf 1; echo "ERROR: command \"$FAILED_COMMAND\" failed with exit code $ERROR_CODE"; tput sgr0;' ERR INT TERM
export MSYS2_ARG_CONV_EXCL="*"
where=$(dirname -- "$0")
cd "$where"
where=$(pwd)
echo "$where"

rm -f grammar.json result.pt

# Parse the example grammar into a JSON parse tree.
dotnet trash parse ExampleLexer.g4 > grammar.json

echo "=== Original grammarDecl (with leading Attribute nodes) ==="
dotnet trash xpath -f grammar.json '//grammarDecl' | dotnet trash tree

# Apply the XQuery to strip attributes that precede LEXER/PARSER.
dotnet trash xquery -q strip-leading-attrs.xq -f grammar.json > result.pt

echo ""
echo "=== Cleaned grammarDecl (leading Attribute nodes removed) ==="
dotnet trash tree -f result.pt

echo ""
echo "Done."
