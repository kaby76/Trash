#!/bin/bash

set -e
trap 'LAST_COMMAND=$CURRENT_COMMAND; CURRENT_COMMAND=$BASH_COMMAND' DEBUG
trap 'ERROR_CODE=$?; FAILED_COMMAND=$LAST_COMMAND; tput setaf 1; echo "ERROR: command \"$FAILED_COMMAND\" failed with exit code $ERROR_CODE"; put sgr0;' ERR INT TERM
export MSYS2_ARG_CONV_EXCL="*"
where=`dirname -- "$0"`
cd "$where"
where=`pwd`
cd "$where"

# Test.
rm -rf Generated
dotnet trparse Arithmetic.g4 > o1.pt
cat o1.pt | dotnet trsplit > o2.pt
cat o2.pt | dotnet trsponge -c -o Generated
dotnet trparse Generated/* | dotnet trcombine > o3.pt
cat o3.pt | dotnet trtext > Generated/Arithmetic.g4

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
	exit 0
fi
