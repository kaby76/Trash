#!/usr/bin/bash
where=`dirname -- "$0"`
cd "$where"
where=`pwd`
cat - > output <<EOF
# Trash, a shell for transforming grammars

## Commands
EOF
cd ../src
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
	tool=${i##*/}
	cat readme.md | sed 's/^#/###/' >> "$where/output"
	cd ..
done
mv "$where/output" "$where/commands.md"
