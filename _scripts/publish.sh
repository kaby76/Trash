#!/usr/bin/bash
version=0.18.0
cd src
directories=`find . -maxdepth 1 -type d -name "tr*"`
cwd=`pwd`
for i in $directories
do
	if [ "$i" == "." ]
	then
		continue
	fi
	cd "$cwd/$i"
	if [[ "$?" != "0" ]]
	then
		continue
	fi
	if [[ ! -f "$i.csproj" ]]
	then
		continue
	fi
	if [[ ! -f "bin/Debug/$i.$version.nupkg" ]]
	then
		continue
	fi
	echo $i
	cd bin/Debug
	if [[ "$?" != "0" ]]
	then
		continue
	fi
	tool=${i##*/}
	dotnet nuget push $tool.$version.nupkg --api-key $trashkey --source https://api.nuget.org/v3/index.json
done
