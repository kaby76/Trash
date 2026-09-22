#!/bin/bash
# Run the reusable iXML-to-ANTLR4 converter on the bundled iXML grammar.
#
# Usage: bash run-example.sh

set -euo pipefail
CURRENT_COMMAND=
LAST_COMMAND=
trap 'LAST_COMMAND=$CURRENT_COMMAND; CURRENT_COMMAND=$BASH_COMMAND' DEBUG
trap 'ERROR_CODE=$?; FAILED_COMMAND=$LAST_COMMAND; tput setaf 1; echo "ERROR: command \"$FAILED_COMMAND\" failed with exit code $ERROR_CODE"; tput sgr0;' ERR INT TERM
export MSYS2_ARG_CONV_EXCL="*"
where=$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)
cd "$where"
echo "$where"

rm -rf xxx
mkdir -p xxx

bash "$where/convert-ixml-to-antlr4.sh" "$where/ixml.ixml" > xxx/ixml.g4
