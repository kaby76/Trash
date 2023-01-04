#!/usr/bin/bash
version=0.19.0-alpha2
cd src
exes=`find . -name 'tr*.exe' | grep -v publish`
for i in $exes
do
	d=`echo $i | awk -F '/' '{print $2}'`
	echo $d
	tool=$d
	pushd $d/bin/Debug
	dotnet nuget push $tool.$version.nupkg --api-key $trashkey --source https://api.nuget.org/v3/index.json
	popd
done
