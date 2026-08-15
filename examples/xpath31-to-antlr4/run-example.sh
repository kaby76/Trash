#!/bin/bash
# Generate parser tables for the XPath 3.1 EBNF using the Trash toolkit.
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


# Generate parser tables for xpath3 meta.
rm -rf meta-interp grammar.json
dotnet trash parse -l xpath31-meta/*.g4 > grammar.json
dotnet trash interp -o xpath31-meta-interp/ < grammar.json

# Generate parser tables for xpath3*
rm -rf xpath31-interp
dotnet trash parse -L xpath31-meta-interp xpath31.ebnf > xpath31.pt

echo "Done."
