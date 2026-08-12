#!/bin/bash
# Generate a Java-target Antlr4 parser for the Java grammar, build it,
# and parse example .java files.
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

# Generate the Java-target parser into Generated-Java/.
rm -rf Generated-Java
dotnet trash gen -t Java

# Build the generated project.
cd Generated-Java
make

# Parse each example .java file and print the parse tree.
for f in ../examples/*.java; do
    [ -f "$f" ] || continue
    echo "--- Parsing $f ---"
    bash run.sh -input "$f"
done

echo "Done."
