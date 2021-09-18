#!/bin/bash

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
	dotnet tool uninstall -g $i
	cd ..
done
