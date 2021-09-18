#!/bin/bash

unameOut="$(uname -s)"
case "${unameOut}" in
    Linux*)     machine=Linux;;
    Darwin*)    machine=Mac;;
    CYGWIN*)    machine=Cygwin;;
    MINGW*)     machine=MinGw;;
    *)          machine="UNKNOWN:${unameOut}"
esac
if [[ "$machine" == "MinGw" ]]
then
    cwd=`pwd | sed 's%/c%c:%' | sed 's%/%\\\\%g'`
else
    cwd=`pwd`
fi
echo "$machine"
echo "$cwd"
directories=`find . -maxdepth 1 -type d`
for i in $directories
do
	if [ "$i" == "." ]
	then
		continue
	fi
	cd $i
	csproj=`find . -maxdepth 1 -name '*.csproj'`
	if [[ "$csproj" == "" ]]
	then
		cd ..
		continue
	fi
	if [[ ! -f "$i.csproj" ]]
	then
		echo $i
		echo nope
		exit 1
	fi
	echo $i
	dotnet nuget add source $cwd/$i/bin/Debug/ --name nuget-$i
	cd ..
done
dotnet nuget list source
