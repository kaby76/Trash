#!/usr/bin/bash
set -x
dotnet new tool-manifest
cd src
dirs=`find .  -name net8.0 | fgrep 'bin/Release' | fgrep -v Generated | grep '^./tr' | fgrep -v publish | sort -u`
for i in $dirs
do
	d=`echo $i | awk -F '/' '{print $2}'`
	pushd $i
	if [ ! -f $d.dll ]
	then
		popd
		continue
	fi
	popd
	pushd $d
	tool=$d
	dotnet tool install $tool
	dotnet $tool -- --version
	popd
done
