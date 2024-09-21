#!/bin/sh

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
git clone https://github.com/kaby76/g4-scripts.git Generated/g4-scripts

for i in "*.g4"
do
	echo $i
	extension="${i##*.}"
	filename="${i%.*}"
	dotnet trparse -- $i | dotnet trquery -- -c Generated/g4-scripts/delabel.xq | dotnet trsponge -- -c -o "Generated"
done
rm -rf Generated/g4-scripts
diff -r Gold Generated
if [ "$?" != "0" ]
then
	echo Test failed.
	exit 1
else
	echo Test succeeded.
	exit 0
fi
