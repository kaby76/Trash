#!/bin/bash

framework="net6.0"
version_antlr="3.3.0"
version_tree="4.2.0"
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
	echo ${i##*/}
	rm -f asdfasdf
	cat *.csproj | sed -e "s%<TargetFramework>.*</TargetFramework>%<TargetFramework>"$framework"</TargetFramework>%" > asdfasdf
	mv asdfasdf *.csproj	
	cat *.csproj | sed -e "s%\"Domemtech.TrashBase\" Version=\".*\"%\"Domemtech.TrashBase\" Version=\"$version_antlr\"%" > asdfasdf
	mv asdfasdf *.csproj	
	cat *.csproj | sed -e "s%\"AntlrTreeEditing\" Version=\".*\"%\"AntlrTreeEditing\" Version=\"$version_tree\"%" > asdfasdf
	mv asdfasdf *.csproj	
	cd ..
done
