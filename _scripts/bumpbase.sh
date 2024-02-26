#!/bin/bash

version_framework="net8.0"
version_antlrruntime="4.13.1"

files=`find . -name '*.csproj'`
subset=`grep -l -e Antlr4.Runtime.Standard $files`
for i in $subset
do
	cat $i | sed -e "s%\"Antlr4.Runtime.Standard\" Version=\".*\"%\"Antlr4.Runtime.Standard\" Version=\"$version_antlrruntime\"%" > asdfasdf
	mv asdfasdf $i
done

subset=`grep -l -e TargetFramework $files`
for i in $subset
do
	cat $i | sed -e "s%<TargetFramework>[^\]*</TargetFramework>%<TargetFramework>"$version_framework"</TargetFramework>%" > asdfasdf
	mv asdfasdf $i
done
