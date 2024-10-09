#!/bin/bash
export MSYS2_ARG_CONV_EXCL="*"
where=`dirname -- "$0"`
cd "$where"
rm -rf Generated-CSharp
dotnet trgen -t CSharp --arithmetic
cd Generated-CSharp
dotnet restore Test.csproj
dotnet build Test.csproj
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
