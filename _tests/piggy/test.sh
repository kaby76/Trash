#!/bin/bash

# BASH error handling:
#   exit on command failure
set -e
#   keep track of the last executed command
trap 'LAST_COMMAND=$CURRENT_COMMAND; CURRENT_COMMAND=$BASH_COMMAND' DEBUG
#   on error: print the failed command
trap 'ERROR_CODE=$?; FAILED_COMMAND=$LAST_COMMAND; tput setaf 1; echo "ERROR: command \"$FAILED_COMMAND\" failed with exit code $ERROR_CODE"; put sgr0;' ERR INT TERM

export MSYS2_ARG_CONV_EXCL="*"
where=`dirname -- "$0"`
cd "$where/antlr4"
rm -rf Generated
trgen
dotnet build Generated/Test.csproj
cd ..
trparse -p antlr4/Generated "$where/Repeat.g4" | trpiggy "$where/repeat.pig" | trtree > "$where/Generated/Repeat.g4"
diff -r "$where/Gold" "$where/Generated"
echo "Test suceeded."
