#!/bin/bash

set -e
trap 'LAST_COMMAND=$CURRENT_COMMAND; CURRENT_COMMAND=$BASH_COMMAND' DEBUG
trap 'ERROR_CODE=$?; FAILED_COMMAND=$LAST_COMMAND; tput setaf 1; echo "ERROR: command \"$FAILED_COMMAND\" failed with exit code $ERROR_CODE"; put sgr0;' ERR INT TERM
export MSYS2_ARG_CONV_EXCL="*"
where=`dirname -- "$0"`
cd "$where"
where=`pwd`
cd "$where"
rm -rf Generated
mkdir Generated

# Test.
dotnet trparse Expression.g4 | dotnet trquery 'insert //lexerRuleSpec/TOKEN_REF[text()="INT"] "fragment "' | dotnet trsponge -c -o Generated

# Diff.
for i in "$where/Generated/*"
do
	dos2unix $i
done
for i in "$where/Gold/*"
do
	dos2unix $i
done
diff -r "$where/Gold" "$where/Generated"
if [ "$?" != "0" ]
then
	echo Test failed.
	exit 1
else
	echo Test succeeded.
	exit 0
fi
