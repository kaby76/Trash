#!/usr/bin/bash

for i in *
do
	if [ ! -d $i ]
	then
		continue
	fi
	pushd $i
	rm -rf Generated-*
	dotnet trgen -t CSharp
	cd Generated-*
	mv Test.csproj $i.csproj
	sed -i -e "s/Test.csproj/$i.csproj/g" build.sh
	make
	popd
done
