#!/bin/bash
set +e
set -x
export TERM=xterm-mono
export MSYS2_ARG_CONV_EXCL="*"
where=`dirname -- "$0"`
cd "$where"
rm -rf Generated-CSharp/
trgen
cd Generated-CSharp
dotnet restore Test.csproj
dotnet build Test.csproj
trperf -i "1+2"  | tail -n +2 | sort -k6 -n -r | sed 's/[0-9][0-9]*[.][0-9][0-9]*/xxx/g'> ../output
rm -rf Generated-CSharp/
dos2unix output
diff output Gold/
if [ "$?" != "0" ]
then
	echo Test failed.
	exit 1
else
	echo Test succeeded.
fi

