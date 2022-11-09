#!/bin/bash
export MSYS2_ARG_CONV_EXCL="*"
where=`dirname -- "$0"`
cd "$where"
rm -rf Generated/
trgen
dotnet restore Generated/Test.csproj
dotnet build Generated/Test.csproj
trperf -i "1+2"  | tail -n +2 | sort -k6 -n -r | column -t | sed 's/[0-9][0-9]*[.][0-9][0-9]*/xxx/g'> output
rm -rf Generated/
dos2unix output
diff output Gold/
if [ "$?" != "0" ]
then
	echo Test failed.
	exit 1
else
	echo Test succeeded.
fi

