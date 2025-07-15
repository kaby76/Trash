#!/usr/bin/bash
set -e
templates=`pwd`/_templates
for i in [a-z]*
do
	if [ ! -d $i ]
	then
		continue
	fi
	pushd $i > /dev/null 2>&1
	rm -rf Generated-*
	dotnet trgen -t CSharp --template-sources-directory $templates > /dev/null 2>&1
	if [ $? -ne 0 ]
	then
		echo Grammar `pwd` failed.
		exit 1
	fi
	cd Generated-*
	mv Test.csproj $i.csproj
	sed -i -e "s/Test.csproj/$i.csproj/g" build.sh
	popd > /dev/null 2>&1
done
