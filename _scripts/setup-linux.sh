#!/bin/bash

unameOut="$(uname -s)"
case "${unameOut}" in
    Linux*)     machine=Linux;;
    Darwin*)    machine=Mac;;
    CYGWIN*)    machine=Cygwin;;
    MINGW*)     machine=MinGw;;
    *)          machine="UNKNOWN:${unameOut}"
esac
if [[ "$machine" == "MinGw" || "$machine" == "Msys" ]]
then
    cwd=`pwd | sed 's%/c%c:%' | sed 's%/%\\\\%g'`
else
    cwd=`pwd`
fi
echo "$machine"
echo $cwd
directories=`find . -maxdepth 1 -type d -name "tr*"`
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
		cd ..
		continue
	fi
	tool=${i##*/}
	if [[ -d "$cwd/$tool/bin/Debug/" ]]
	then
		echo $i
		dotnet nuget add source $cwd/$tool/bin/Debug/ --name nuget-$tool > /dev/null 2>&1
	fi
	cd ..
done
dotnet nuget list source
