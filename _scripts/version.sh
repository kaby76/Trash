directories=`find . -maxdepth 1 -type d -name "tr*"`
cwd=`pwd`
for i in $directories
do
	if [ "$i" == "." ]
	then
		continue
	fi
	cd "$cwd/$i"
	csproj=`find . -maxdepth 1 -name '*.csproj'`
	if [[ "$csproj" == "" ]]
	then
		continue
	fi
	if [[ ! -f "$i.csproj" ]]
	then
		continue
	fi
	trxml2 "$i.csproj" | grep -i PackAsTool 2> /dev/null 1> /dev/null
	if [[ "$?" != "0" ]]
	then
		continue
	fi
	echo $i
	tool=${i##*/}
	$tool --version
done
