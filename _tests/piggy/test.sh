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
rm -rf "$where/Generated"
pwd
trgen
dotnet build Generated/Test.csproj
echo PWD:
pwd
echo ls -R
ls -R
cd ..
echo PWD:
pwd
rm -rf Generated
mkdir Generated
echo Right before trparse call
echo PWD:
pwd
echo ls -R
ls -R
trparse -p "antlr4/Generated" "Repeat.g4" | trpiggy "repeat.pig" | trtext > "Generated/Repeat.g4"
diff -r "Gold" "Generated"
echo "Test suceeded."
