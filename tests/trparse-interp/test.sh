#!/bin/bash

set -e
trap 'LAST_COMMAND=$CURRENT_COMMAND; CURRENT_COMMAND=$BASH_COMMAND' DEBUG
trap 'ERROR_CODE=$?; FAILED_COMMAND=$LAST_COMMAND; tput setaf 1; echo "ERROR: command \"$FAILED_COMMAND\" failed with exit code $ERROR_CODE"; tput sgr0;' ERR INT TERM
export MSYS2_ARG_CONV_EXCL="*"
where=`dirname -- "$0"`
cd "$where"
where=`pwd`
echo "$where"

rm -rf Generated-CSharp
rm -rf interp *.json *.tree

echo Generate native CSharp parser and parse.
dotnet trash gen -t CSharp
cd Generated-CSharp
bash build.sh
printf "1 + 2 + 3" | dotnet trash parse | dotnet trash tree > trparse.tree
dos2unix trparse.tree
dos2unix ../Gold/trparse.tree
diff trparse.tree Gold/trparse.tree
echo Diff against the golden file.
if [ "$?" != "0" ]
then
    echo Test failed.
    cd ..
    rm -rf Generated-CSharp
    exit 1
fi
cd ..
rm -rf Generated-CSharp

echo Generate .interp files from the grammar and parse.
dotnet trash parse Expression.g4 > grammar.json
dotnet trash interp -o interp/ < grammar.json
printf "1 + 2 + 3" | dotnet trash parse --lib interp/ | dotnet trash tree > trparse.tree

echo Diff against the golden file.
dos2unix trparse.tree
dos2unix Gold/trparse.tree
diff trparse.tree Gold/trparse.tree
if [ "$?" != "0" ]
then
    echo Test failed.
    rm -rf Generated-CSharp
    rm -rf interp *.json *.tree
    exit 1
fi

echo Test succeeded.
rm -rf Generated-CSharp
rm -rf interp *.json *.tree
exit 0
