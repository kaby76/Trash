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
exes=`find . -name 'tr*.exe' | grep -v publish`
for i in $exes
do
	d=`echo $i | awk -F '/' '{print $2}'`
	cd $d
	tool=$d
	dotnet nuget add source $cwd/src/$tool/bin/Debug/ --name nuget-$tool > /dev/null 2>&1
	cd ..
done
#dotnet nuget add source "$cwd/src/AntlrJson/bin/Debug/" --name nuget-AntlrJson > /dev/null 2>&1
#dotnet nuget add source "$cwd/src/Docs/bin/Debug/" --name nuget-Docs > /dev/null 2>&1
dotnet nuget list source
