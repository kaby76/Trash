#!/bin/bash
export MSYS2_ARG_CONV_EXCL="*"
where=`dirname -- "$0"`
cd "$where"
rm -rf Generated/
trgen
dotnet restore Generated/Test.csproj
dotnet build Generated/Test.csproj
trparse -i "1+2" | trjson > output
dos2unix output
dos2unix Gold/output
diff output Gold/
rm -rf Generated/
if [ "$?" != "0" ]
then
	echo Test failed.
	exit 1
else
	echo Test succeeded.
fi
