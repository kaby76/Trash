#!/bin/bash

version=""
#version="--version 0.8.1"

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
	echo $i
	tool=${i##*/}
	dotnet tool install -g $tool $version > /dev/null 2>&1
	cd ..
done
