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
echo "$machine"
echo "$cwd"
dotnet tool install -g trxml2
cd src
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
		cd ..
		continue
	fi
	trxml2 "$i.csproj" | grep -i PackAsTool 2> /dev/null 1> /dev/null
	if [[ "$?" != "0" ]]
	then
		cd ..
		continue
	fi
	tool=${i##*/}
	if [[ -d "$cwd/src/$tool/bin/Debug/" ]]
	then
		echo $i
		dotnet nuget add source $cwd/src/$tool/bin/Debug/ --name nuget-$tool > /dev/null 2>&1
	fi
	cd ..
done
dotnet nuget add source "$cwd/src/AntlrJson/bin/Debug/" --name nuget-AntlrJson > /dev/null 2>&1
dotnet nuget add source "$cwd/src/Docs/bin/Debug/" --name nuget-Docs > /dev/null 2>&1
dotnet nuget list source
