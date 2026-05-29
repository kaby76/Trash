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
exes=`find . -name 'tr*.exe' | grep -v publish`
tools=""
for i in $exes
do
	d=`echo $i | awk -F '/' '{print $2}'`
	pushd $d
	tool=${d##*/}
	tools="$tools $tool"
	popd
done
for i in $tools
do
    echo dotnet nuget add source $cwd/src/$i/bin/$CONFIG/ --name trtool-$i
    dotnet nuget add source $cwd/src/$i/bin/$CONFIG/ --name trtool-$i > /dev/null 2>&1
done
dotnet nuget list source
