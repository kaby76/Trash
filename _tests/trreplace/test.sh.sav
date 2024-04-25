#!/bin/bash
# BASH error handling:
#   exit on command failure
set -e
#   keep track of the last executed command
trap 'LAST_COMMAND=$CURRENT_COMMAND; CURRENT_COMMAND=$BASH_COMMAND' DEBUG
#   on error: print the failed command
trap 'ERROR_CODE=$?; FAILED_COMMAND=$LAST_COMMAND; tput setaf 1; echo "ERROR: command \"$FAILED_COMMAND\" failed with exit code $ERROR_CODE"; put sgr0;' ERR INT TERM
# Setting MSYS2_ARG_CONV_EXCL so that Trash XPaths do not get mutulated.
export MSYS2_ARG_CONV_EXCL="*"

# Start in the directory containing test script.
where=`dirname -- "$0"`
cd "$where"
where=`pwd`

# Test.
rm -rf Generated
trparse Expression.g4 | trreplace '//parserRuleSpec/RULE_REF[text()="a"]' 'atom' | trsponge -c -o Generated

# Diff result.
for i in Gold/*
do
	dos2unix $i
done
for i in Generated/*
do
	dos2unix $i
done
diff -r Generated Gold
if [ "$?" != "0" ]
then
	echo Test failed.
	exit 1
else
	echo Test succeeded.
fi
exit 0
