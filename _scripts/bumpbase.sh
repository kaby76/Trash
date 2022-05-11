#!/bin/bash

version_framework="net6.0"
version_base="4.3.0"
version_tree="5.1.0"
version_antlr4buildtasks="10.3"
version_stringtemplate="4.2.0"
version_antlrruntime="4.10.1"

files=`find . -name '*.csproj'`
subset=`grep -l -e Antlr4.Runtime.Standard $files`
for i in $subset
do
	cat $i | sed -e "s%\"Antlr4.Runtime.Standard\" Version=\".*\"%\"Antlr4.Runtime.Standard\" Version=\"$version_antlrruntime\"%" > asdfasdf
	mv asdfasdf $i
done

subset=`grep -l -e AntlrTreeEditing $files`
for i in $subset
do
	cat $i | sed -e "s%\"AntlrTreeEditing\" Version=\".*\"%\"AntlrTreeEditing\" Version=\"$version_tree\"%" > asdfasdf
	mv asdfasdf $i
done

subset=`grep -l -e Antlr4BuildTasks $files`
for i in $subset
do
	cat $i | sed -e "s%\"Antlr4BuildTasks\" Version=\".*\"%\"AntlrTreeEditing\" Version=\"$version_antlr4buildtasks\"%" > asdfasdf
	mv asdfasdf $i
done

subset=`grep -l -e Antlr4BuildTasks $files`
for i in $subset
do
	cat $i | sed -e "s%<TargetFramework>.*</TargetFramework>%<TargetFramework>"$version_framework"</TargetFramework>%" > asdfasdf
	mv asdfasdf $i
done

subset=`grep -l -e Domemtech.TrashBase $files`
for i in $subset
do
	cat $i | sed -e "s%\"Domemtech.TrashBase\" Version=\".*\"%\"Domemtech.TrashBase\" Version=\"$version_base\"%" > asdfasdf
	mv asdfasdf $i
done

subset=`grep -l -e Domemtech.StringTemplate4 $files`
for i in $subset
do
	cat $i | sed -e "s%\"Domemtech.StringTemplate4\" Version=\".*\"%\"Domemtech.StringTemplate4\" Version=\"$version_stringtemplate\"%" > asdfasdf
	mv asdfasdf $i
done
