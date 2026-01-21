#!/usr/bin/bash
#set -e
#set -x
version="0.23.38"
cd src
directories=`find . -maxdepth 1 -type d -name "tr*"`
cwd=`pwd`
dotnet tool install -g trxml2
for i in $directories
do
        echo $i
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
	for csproj in *.csproj
	do
		sed -i -e "s%[<][Vv]ersion[>].*[<][/][Vv]ersion[>]%<Version\>$version</Version>%" $csproj
	done
        sed -i -e 's%^0[.][0-9]*[.][0-9]*.*$'"%$version"' Fixes to trgen templates.%' readme.md
	for cs in *.cs
	do
		sed -i -e "s%public string Version { get; set; } = \"0[.][0-9]*[.][0-9]*\";%public string Version { get; set; } = \"$version\";%" $cs
	done
        cd ..
done

for i in tragl
do
        echo $i
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
        if [[ ! -f "readme.md" ]]
        then
                continue;
        fi
        echo $i
        rm -f asdfasdf
        cat *.csproj | sed -e "s%[<][Vv]ersion[>].*[<][/][Vv]ersion[>]%<Version\>$version</Version>%" > asdfasdf
        mv asdfasdf *.csproj    
        sed -i -e 's%^0[.][0-9]*[.][0-9]*.*$'"%$version"' Fixes to trgen templates.%' readme.md
        cd ..
done

for i in trgen
do
    pushd $i
        rm -f asdfasdf
        cat Command.cs | sed -e 's%public static string version = "[^"]*";%public static string version = "'$version'";%' > asdfasdf
        mv asdfasdf Command.cs
        popd
done
