#!/bin/bash
# Generate .interp files from the ABNF grammar and parse example ABNF/BNF files.
#
# Usage: bash run.sh

set -e
trap 'LAST_COMMAND=$CURRENT_COMMAND; CURRENT_COMMAND=$BASH_COMMAND' DEBUG
trap 'ERROR_CODE=$?; FAILED_COMMAND=$LAST_COMMAND; tput setaf 1; echo "ERROR: command \"$FAILED_COMMAND\" failed with exit code $ERROR_CODE"; tput sgr0;' ERR INT TERM
export MSYS2_ARG_CONV_EXCL="*"
where=$(dirname -- "$0")
cd "$where"
where=$(pwd)
echo "$where"

# Generate .interp and .tokens files from the ABNF grammar.
rm -rf interp grammar.json
dotnet trash parse -l Abnf.g4 > grammar.json
dotnet trash interp -o interp/ --atn < grammar.json

# Parse each .abnf and .bnf example file using the interpreter.
files=`find examples -name '*.*bnf'`
dotnet trash parse --lib interp/ $files > earley.pt
dotnet trash tree -f earley.pt > earley.tree
dotnet trash parse --lib interp/ --allstar $files > allstar.pt
dotnet trash tree -f earley.pt > allstar.tree

echo "Done."
