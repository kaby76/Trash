#!/bin/bash

set -e
trap 'LAST_COMMAND=$CURRENT_COMMAND; CURRENT_COMMAND=$BASH_COMMAND' DEBUG
trap 'ERROR_CODE=$?; FAILED_COMMAND=$LAST_COMMAND; tput setaf 1; echo "ERROR: command \"$FAILED_COMMAND\" failed with exit code $ERROR_CODE"; tput sgr0;' ERR INT TERM
export MSYS2_ARG_CONV_EXCL="*"
where=`dirname -- "$0"`
cd "$where"
where=`pwd`
echo "$where"

# Generate .interp files from the grammar.
rm -rf interp grammar.json
dotnet trash parse Expression.g4 > grammar.json
dotnet trash interp -o interp/ < grammar.json

# Parse a sample expression using the Earley ATN-based path.
printf "1 + 2 + 3" | dotnet trash parse --lib interp/ | dotnet trash tree > trparse-interp.tree

# Diff against the golden file.
dos2unix trparse-interp.tree
dos2unix Gold/trparse-interp.tree
diff trparse-interp.tree Gold/trparse-interp.tree
if [ "$?" != "0" ]
then
    echo Test failed.
    exit 1
else
    echo Test succeeded.
    exit 0
fi
