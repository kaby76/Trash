#!/usr/bin/bash
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
cd src
directories=`find . -maxdepth 1 -type d -name "tr*"`
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
	if [[ -d "$cwd/src/$tool/bin/Release/" ]]
	then
		echo $i
		dotnet nuget add source $cwd/$tool/bin/Release/ --name nuget-$tool > /dev/null 2>&1
	fi
	popd
done
dotnet nuget list source
