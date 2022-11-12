#!/bin/bash

# BASH error handling:
#   exit on command failure
set -e
#   keep track of the last executed command
trap 'LAST_COMMAND=$CURRENT_COMMAND; CURRENT_COMMAND=$BASH_COMMAND' DEBUG
#   on error: print the failed command
trap 'ERROR_CODE=$?; FAILED_COMMAND=$LAST_COMMAND; tput setaf 1; echo "ERROR: command \"$FAILED_COMMAND\" failed with exit code $ERROR_CODE"; put sgr0;' ERR INT TERM

# "Setting MSYS2_ARG_CONV_EXCL so that Trash XPaths do not get mutulated."
export MSYS2_ARG_CONV_EXCL="*"
where=`dirname -- "$0"`
cd "$where"
where=`pwd`
cd "$where"

# Test.
rm -rf Generated
trparse Arithmetic.g4 > o1.pt
cat o1.pt | trsplit > o2.pt
cat o2.pt | trsponge -c -o Generated
trcombine Generated/ArithmeticLexer.g4 Generated/ArithmeticParser.g4 > o3.pt
cat o3.pt | trprint > Generated/Arithmetic.g4

# Diff.
for i in "$where/Generated/*"
do
	dos2unix $i
done
for i in "$where/Gold/*"
do
	dos2unix $i
done
diff -r Gold/Arithmetic.g4 Generated/Arithmetic.g4
if [ "$?" != "0" ]
then
	echo Test failed.
	exit 1
else
	echo Test succeeded.
fi
exit 0
