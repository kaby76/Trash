#!/bin/bash

set -e
set -x
trap 'LAST_COMMAND=$CURRENT_COMMAND; CURRENT_COMMAND=$BASH_COMMAND' DEBUG
trap 'ERROR_CODE=$?; FAILED_COMMAND=$LAST_COMMAND; tput setaf 1; echo "ERROR: command \"$FAILED_COMMAND\" failed with exit code $ERROR_CODE"; put sgr0;' ERR INT TERM
export MSYS2_ARG_CONV_EXCL="*"
where=`dirname -- "$0"`
cd "$where"
where=`pwd`
cd "$where"
dotnet trash parse --version

# Test.
rm -rf Generated
dotnet trash parse Arithmetic.g4 > o1.pt
cat o1.pt | dotnet trash split > o2.pt
cat o2.pt | dotnet trash sponge -c -o Generated
dotnet trash parse Generated/* | dotnet trash combine > o3.pt
cat o3.pt | dotnet trash text > Generated/Arithmetic.g4

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
