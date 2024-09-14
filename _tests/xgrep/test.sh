#!/bin/bash

set -e
trap 'LAST_COMMAND=$CURRENT_COMMAND; CURRENT_COMMAND=$BASH_COMMAND' DEBUG
trap 'ERROR_CODE=$?; FAILED_COMMAND=$LAST_COMMAND; tput setaf 1; echo "ERROR: command \"$FAILED_COMMAND\" failed with exit code $ERROR_CODE"; put sgr0;' ERR INT TERM
export MSYS2_ARG_CONV_EXCL="*"

where=`dirname -- "$0"`
cd "$where"
where=`pwd`

rm -rf Generated-CSharp
trgen -t CSharp
cd Generated-CSharp
make
echo "1 + 2 + 3" | trparse | trquery grep ' //INT' | trtree > ../output
cd ..
#rm -rf Generated-CSharp

# Diff result.
for i in output Gold/output
do
	dos2unix $i
done
diff output Gold
if [ "$?" != "0" ]
then
	echo Test failed.
	exit 1
else
	echo Test succeeded.
	exit 0
fi
