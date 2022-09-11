#!/usr/bin/bash
version=""
#version="--version 0.8.1"
dotnet tool install -g trxml2 $version > /dev/null 2>&1
cd src
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
	trxml2 "$i.csproj" | grep -i PackAsTool 2> /dev/null 1> /dev/null
	if [[ "$?" != "0" ]]
	then
		cd ..
		continue
	fi
	echo $i
	tool=${i##*/}
	dotnet tool install -g $tool $version > /dev/null 2>&1
	$tool --version
	cd ..
done
