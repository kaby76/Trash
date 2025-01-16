#!/bin/bash
set -e
set -x
export MSYS2_ARG_CONV_EXCL="*"
where=`dirname -- "$0"`
cd "$where"
rm -rf Generated-CSharp
dotnet trgen -t CSharp --arithmetic
cd Generated-CSharp
make
dotnet trparse -i "1+2" | dotnet trxml > ../output
cd ..
rm -rf Generated-CSharp/
dos2unix output
diff output Gold/
if [ "$?" != "0" ]
then
	echo Test failed.
	exit 1
else
	echo Test succeeded.
	exit 0
fi
