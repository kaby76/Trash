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
dotnet trash parse Abnf.g4 > grammar.json
dotnet trash interp -o interp/ < grammar.json

# Parse each .abnf and .bnf example file using the interpreter.
for f in examples/*.abnf examples/*.bnf examples/apg-java/*.bnf; do
    [ -f "$f" ] || continue
    echo "--- Parsing $f ---"
    dotnet trash parse --lib interp/ < "$f" | dotnet trash tree
done

echo "Done."
