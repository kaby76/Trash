#!/bin/bash
# Transform a Lark grammar parse tree toward Antlr4 syntax.
#
# Usage: bash run-example.sh

set -x
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

# Pass 1: Apply Lark-to-Antlr4 syntax transforms (steps 1-4).
dotnet trash xquery -q lark-to-antlr4.xq -f o.pt > transformed.pt

# Pass 2: Inline common.lark rules and remove %import statements (step 5).
dotnet trash xquery -q import-common.xq -f transformed.pt > imported.pt

# Pass 3: Convert %ignore TERMINAL to '-> skip' lexer commands (step 6).
dotnet trash xquery -q ignore-to-skip.xq -f imported.pt > ignored.pt

# Pass 4: Convert /regex/ literals to Antlr4 character-class syntax (step 7).
dotnet trash xquery -q regexp-to-antlr4.xq -f ignored.pt \
    | dotnet trash sponge -o xxx
