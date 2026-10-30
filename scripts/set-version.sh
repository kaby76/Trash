#!/usr/bin/bash
#set -e
#set -x
version="4.9.0"
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd src
directories=`find . -maxdepth 1 -type d -name "tr*"`
cwd=`pwd`
dotnet tool install -g trash
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
    dotnet trash xml2 "$i.csproj" | grep -i PackAsTool 2> /dev/null 1> /dev/null
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
    sed -i -E 's%^Release [0-9]*[.][0-9]*([.][0-9]*)*.*$%Release '"$version"'.%' readme.md
    for cs in *.cs
    do
        sed -i -E "s%public string Version \{ get; set; \} = \"[0-9][.][0-9]*([.][0-9]*)*\";%public string Version \{ get; set; \} = \"$version\";%" $cs
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
    sed -i -e "s%[<][Vv]ersion[>].*[<][/][Vv]ersion[>]%<Version\>$version</Version>%" "$i.csproj"
    sed -i -E 's%^Release [0-9]*[.][0-9]*([.][0-9]*)*.*$%Release '"$version"'.%' readme.md
    sed -i -E "s%public string Version \{ get; set; \} = \"[0-9][.][0-9]*([.][0-9]*)*\";%public string Version \{ get; set; \} = \"$version\";%" Config.cs
    cd ..
done

for i in trgen trash
do
    pushd $i
    rm -f asdfasdf
    for f in Command.cs Program.cs
    do
        if [ ! -f $f ]
        then
            continue
        fi
        cat $f | sed -e 's%public static string version = "[^"]*";%public static string version = "'$version'";%' > asdfasdf
        mv asdfasdf $f
    done
    popd
done

pushd $SCRIPT_DIR
rm -f asdfasdf
for f in publish.sh
do
    if [ ! -f $f ]
    then
        continue
    fi
    cat $f | sed -e 's%version=.*$%version='$version'%' > asdfasdf
    mv asdfasdf $f
done
popd
