#!/usr/bin/bash
unameOut="$(uname -s)"
case "${unameOut}" in
    Linux*)     machine=Linux;;
    Darwin*)    machine=Mac;;
    CYGWIN*)    machine=Cygwin;;
    MINGW*)     machine=MinGw;;
    MSYS_NT*)   machine=Msys;;
    *)          machine="UNKNOWN:${unameOut}"
esac
if [[ "$machine" == "MinGw" || "$machine" == "Msys" ]]
then
    cwd=`pwd | sed 's%/c%c:%' | sed 's%/%\\\\%g'`
else
    cwd=`pwd`
fi
CONFIG=Release
echo "$machine"
echo "$cwd"
cd src
directories=`find . -maxdepth 1 -type d -name "tr*"`
tools=""
for i in $directories
do
	if [ "$i" == "." ]
	then
		continue
	fi
	pushd $i
	csproj=`find . -maxdepth 1 -name '*.csproj'`
	if [[ "$csproj" == "" ]]
	then
		popd
		continue
	fi
	if [[ ! -f "$i.csproj" ]]
	then
		popd
		continue
	fi
	tool=${i##*/}
	tools="$tools $tool"
	popd
done
for i in $tools
do
    echo dotnet nuget add source $cwd/src/$i/bin/$CONFIG/ --name trtool-$i
    dotnet nuget add source $cwd/src/$i/bin/$CONFIG/ --name trtool-$i > /dev/null 2>&1
done
dotnet nuget list source
