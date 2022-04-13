#!/bin/bash

dotnet tool install -g trxml2 > /dev/null 2>&1

framework="net6.0"
version_base="4.0.0"
version_tree="5.0.0"
version_stringtemplate="4.2.0"
version_antlrruntime="4.10.0"
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
	cat *.csproj | sed -e "s%\"Domemtech.TrashBase\" Version=\".*\"%\"Domemtech.TrashBase\" Version=\"$version_base\"%" > asdfasdf
	mv asdfasdf *.csproj	
	cat *.csproj | sed -e "s%\"AntlrTreeEditing\" Version=\".*\"%\"AntlrTreeEditing\" Version=\"$version_tree\"%" > asdfasdf
	mv asdfasdf *.csproj	
	cat *.csproj | sed -e "s%\"Domemtech.StringTemplate4\" Version=\".*\"%\"Domemtech.StringTemplate4\" Version=\"$version_stringtemplate\"%" > asdfasdf
	mv asdfasdf *.csproj	
	cd ..
done

files=`find . -name '*.csproj'`
subset=`grep -l -e Antlr4.Runtime.Standard $files`
for i in $files
do
	cat $i | sed -e "s%\"Antlr4.Runtime.Standard\" Version=\".*\"%\"Antlr4.Runtime.Standard\" Version=\"$version_antlrruntime\"%" > asdfasdf
	mv asdfasdf $i
done
