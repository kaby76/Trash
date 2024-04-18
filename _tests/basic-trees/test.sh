#!/bin/bash

set -e
trap 'LAST_COMMAND=$CURRENT_COMMAND; CURRENT_COMMAND=$BASH_COMMAND' DEBUG
trap 'ERROR_CODE=$?; FAILED_COMMAND=$LAST_COMMAND; tput setaf 1; echo "ERROR: command \"$FAILED_COMMAND\" failed with exit code $ERROR_CODE"; put sgr0;' ERR INT TERM
export MSYS2_ARG_CONV_EXCL="*"
where=`dirname -- "$0"`
cd "$where"
where=`pwd`
cd "$where"
echo "$where"

# Test.
rm -rf Generated-CSharp
trgen -t CSharp
pushd Generated-CSharp
make
popd
echo "1 + 2 + 3" | trparse | trtree > trparse.tree
rm -rf Generated-CSharp

# Diff result.
dos2unix trparse.tree
dos2unix Gold/trparse.tree
diff trparse.tree Gold/trparse.tree
if [ "$?" != "0" ]
then
	echo Test failed.
	exit 1
else
	echo Test succeeded.
	exit 0
fi
