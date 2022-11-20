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
cd antlr4
pwd
rm -rf Generated-CSharp
trgen
cd Generated-CSharp
dotnet build
cd "$where"
rm -rf Generated
mkdir Generated
echo trparse -p "antlr4/Generated" "Repeat.g4"
trparse -p "antlr4/Generated-CSharp" "Repeat.g4" > o1.pt
echo trpiggy "repeat.pig"
cat o1.pt | trpiggy "repeat.pig" > o2.pt
echo trtext
cat o2.pt | trtext > "Generated/Repeat.g4"
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
fi
