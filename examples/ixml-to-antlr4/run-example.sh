#!/bin/bash
# Convert an iXML grammar to Antlr4 syntax using the Trash XQuery pipeline.
#
# Usage: bash run-example.sh

# set -x
set -e
trap 'LAST_COMMAND=$CURRENT_COMMAND; CURRENT_COMMAND=$BASH_COMMAND' DEBUG
trap 'ERROR_CODE=$?; FAILED_COMMAND=$LAST_COMMAND; tput setaf 1; echo "ERROR: command \"$FAILED_COMMAND\" failed with exit code $ERROR_CODE"; tput sgr0;' ERR INT TERM
export MSYS2_ARG_CONV_EXCL="*"
where=$(dirname -- "$0")
cd "$where"
where=$(pwd)
echo "$where"

rm -rf xxx
mkdir -p xxx

# Parse ixml.ixml using the built-in ixml grammar parser.
dotnet trash parse -t ixml ixml.ixml > ixml.pt

# Pass 1: Core structural syntax transforms (marks, separators, terminators).
dotnet trash xquery -q ixml-to-antlr4.xq -f ixml.pt > structural.pt

# Pass 2: Convert encoded character references '#hex' to Antlr4 '\uXXXX'.
dotnet trash xquery -q encoded-to-antlr4.xq -f structural.pt > encoded.pt

# Pass 3: Convert character-set member syntax (unquote chars, rewrite class codes).
dotnet trash xquery -q charset-to-antlr4.xq -f encoded.pt > charsets.pt

# Pass 4: Extract inline character sets to named Antlr4 lexer rules (SET_N).
dotnet trash xquery -q sets-to-lexer.xq -f charsets.pt > sets.pt

# Pass 5: Expand separator quantifiers '++' and '**' to standard repetition,
# then write the result to xxx/.
dotnet trash xquery -q sep-repeat-to-antlr4.xq -f sets.pt > final.pt
dotnet trash tree -f final.pt > final.tree
dotnet trash sponge -f final.pt -o xxx

# Rename each *.ixml output file to *.g4 (trparse preserves the original name).
for f in xxx/*.ixml; do
    name=$(basename "$f" .ixml)
    mv "$f" "xxx/$name.g4"
done

# Prepend 'grammar NAME;' header to each generated .g4 file.
for f in xxx/*.g4; do
    name=$(basename "$f" .g4)
    { printf "grammar %s;\n\n" "$name"; cat "$f"; } > "$f.tmp" && mv "$f.tmp" "$f"
done
