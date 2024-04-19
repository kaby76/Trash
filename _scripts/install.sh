#!/usr/bin/bash
set -x
version=""
version="--prerelease"
cd src
dirs=`find .  -name net8.0 | grep Release | grep -v Generated | grep '^./tr' | grep -v publish`
for i in $dirs
do
	d=`echo $i | awk -F '/' '{print $2}'`
	pushd $i
	if [ ! -f $d.dll ]
	then
		popd
		continue
	fi
	popd
	pushd $d
	tool=$d
	dotnet tool uninstall -g $tool $version > /dev/null 2>&1
	dotnet tool install -g $tool $version > /dev/null 2>&1
	$tool --version
	popd
done
