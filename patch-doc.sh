#!/usr/bin/bash
where=`dirname -- "$0"`
cd "$where"
where=`pwd`
cd src
directories=`find . -maxdepth 1 -type d -name "tr*"`
rm -f "$where/output"
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
	tool=${i##*/}
	echo -n "1) <a href=\"src/$tool/readme.md\">$tool</a> -- " >> "$where/output"
	cat readme.md | awk '/## Summary/{getline;getline;print}' >> "$where/output"
	cd ..
done
