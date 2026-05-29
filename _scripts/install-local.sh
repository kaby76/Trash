#!/usr/bin/bash
set -x
dotnet new tool-manifest --force
cd src
exes=`find . -name 'tr*.exe' | grep -v publish`
for i in $exes
do
	d=`echo $i | awk -F '/' '{print $2}'`
	pushd $d
	tool=${d##*/}
	dotnet tool install $tool
	dotnet $tool --version
	popd
done
