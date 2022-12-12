#!/usr/bin/bash
cd src
exes=`find . -name 'tr*.exe' | grep -v publish`
for i in $exes
do
	d=`echo $i | awk -F '/' '{print $2}'`
	cd $d
	tool=$d
	dotnet tool uninstall -g $tool > /dev/null 2>&1
done
