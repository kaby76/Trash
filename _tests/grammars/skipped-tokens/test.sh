#!/usr/bin/bash

# BASH error handling:
#   exit on command failure
set -e
#   print
# set -x
#   keep track of the last executed command
trap 'LAST_COMMAND=$CURRENT_COMMAND; CURRENT_COMMAND=$BASH_COMMAND' DEBUG
#   on error: print the failed command
trap 'ERROR_CODE=$?; FAILED_COMMAND=$LAST_COMMAND; tput setaf 1; echo "ERROR: command \"$FAILED_COMMAND\" failed with exit code $ERROR_CODE"; put sgr0;' ERR INT TERM
export MSYS2_ARG_CONV_EXCL="*"
where=`dirname -- "$0"`
cd "$where"
where=`pwd`
cd "$where"
rm -rf Generated
mkdir Generated

# Test.
cd Generated
git init grammars-v4
cd grammars-v4
git remote add -f origin https://github.com/antlr/grammars-v4.git
git config core.sparseCheckout true
echo "abb/" >> .git/info/sparse-checkout
echo "abnf/" >> .git/info/sparse-checkout
echo "/asm" >> .git/info/sparse-checkout
git pull origin 0d8be4573d284a89444a46beeface54f48853c8a
for i in `find . -name pom.xml | sort`
do
    base=`dirname $i`
    echo $i
    if [ "$base" != "./asm" ]
    then
	for j in `ls "$base"/*.g4 | sort`
	do
		bash "$where/check.sh" "$j" >> "$where/Generated/output" 2>&1
	done
    fi
done
echo Done

# Diff.
dos2unix "$where/Generated/output"
dos2unix "$where/Gold/output"
diff -r "$where/Gold/output" "$where/Generated/output"
if [ "$?" != "0" ]
then
	echo Test failed.
	echo "generated:"
	cat "$where/Generated/output"
	echo "gold:"
	cat "$where/Gold/output"
	exit 1
else
	echo Test succeeded.
fi
exit 0
