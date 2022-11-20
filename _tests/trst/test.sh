#!/bin/bash
export MSYS2_ARG_CONV_EXCL="*"
where=`dirname -- "$0"`
cd "$where"
rm -rf Generated-CSharp/
trgen
dotnet restore Generated-CSharp/Test.csproj
dotnet build Generated-CSharp/Test.csproj
cd Generated-CSharp
trparse -i "1+2" | trst > ../output
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
fi
