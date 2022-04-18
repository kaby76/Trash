version="0.16.1"

directories=`find . -maxdepth 1 -type d -name "tr*"`
cwd=`pwd`
for i in $directories
do
	if [ "$i" == "." ]
	then
		continue
	fi
	cd $cwd/$i
	if [[ "$?" != "0" ]]
	then
		continue
	fi
	csproj=`find . -maxdepth 1 -name '*.csproj'`
	if [[ "$csproj" == "" ]]
	then
		cd ..
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
	if [[ ! -f "readme.md" ]]
	then
		continue;
	fi
	echo $i
	rm -f asdfasdf
	cat *.csproj | sed -e "s%[<][Vv]ersion[>].*[<][/][Vv]ersion[>]%<Version\>$version</Version>%" > asdfasdf
	mv asdfasdf *.csproj	
	rm -f asdfasdf2
	touch readme.md
	cat readme.md | sed -e 's%^0[.][0-9]*[.][0-9]*[ ]*[-][-].*$'"%$version -- Fixes to Antlr4BuildTasks.%" > asdfasdf2
	mv asdfasdf2 readme.md
	cd ..
done

for i in trgen trgen2
do
    pushd $i
	rm -f asdfasdf
	cat Command.cs | sed -e 's%public static string version = "[^"]*";%public static string version = "'$version'";%' > asdfasdf
	mv asdfasdf Command.cs
	popd
done
