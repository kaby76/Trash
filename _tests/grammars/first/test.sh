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
cd "$where"
where=`pwd`
cd "$where"

# Test.
rm -rf Generated
mkdir Generated
for i in *.g4
do
	rm -rf grammar-temp
	mkdir grammar-temp
	cp $i grammar-temp
	cd grammar-temp
	trgen -s s
	cd Generated-CSharp
	dotnet build
	echo $i >> ../../Generated/output
	trfirst >> ../../Generated/output
	cd ..
	cd ..
	rm -rf grammar-temp
done

# Diff.
cd "$where"
dos2unix Generated/output
dos2unix Gold/output
diff -r Gold/output Generated/output
if [ "$?" != "0" ]
then
	echo Test failed.
	exit 1
else
	echo Test succeeded.
fi
exit 0
