#!/bin/bash

version_antlr="3.0.0"
version_tree="4.0.0"
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
	echo ${i##*/}
	rm -f asdfasdf
	cat *.csproj | sed -e "s%\"Domemtech.TrashBase\" Version=\".*\"%\"Domemtech.TrashBase\" Version=\"$version_antlr\"%" > asdfasdf
	mv asdfasdf *.csproj	
	cat *.csproj | sed -e "s%\"AntlrTreeEditing\" Version=\".*\"%\"AntlrTreeEditing\" Version=\"$version_tree\"%" > asdfasdf
	mv asdfasdf *.csproj	
	cd ..
done
