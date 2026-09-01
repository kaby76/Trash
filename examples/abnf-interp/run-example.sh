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

rm -rf interp grammar.json Generated-CSharp

# Generate native app
dotnet trash gen -t CSharp
pushd Generated-CSharp
files=`find ../examples -name '*.*bnf'`
make
popd

# Generate .interp and .tokens files from the ABNF grammar.
dotnet trash parse -l Abnf.g4 > grammar.json
dotnet trash interp -o interp/ --atn < grammar.json

# Run native
pushd Generated-CSharp
find ../examples -name '*.*bnf' | dotnet trash parse -t gen -x > native.pt
dotnet trash tree -f native.pt > native.tree
popd

# Parse each .abnf and .bnf example file using the interpreter.
pushd interp
dotnet trash parse --lib . $files > earley.pt
dotnet trash tree -f earley.pt > earley.tree
dotnet trash parse --lib . --allstar $files > allstar.pt
dotnet trash tree -f allstar.pt > allstar.tree
popd

echo "Done."
