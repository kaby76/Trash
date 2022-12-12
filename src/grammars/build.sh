#!/bin/sh

cwd=`pwd`
for i in *
do
    if [[ -f $i ]]
    then
        continue
    fi
    echo $i
    cd $cwd/$i
    rm -rf Generated
    if [ -f "pom.xml" ]
    then
        trgen --template-sources-directory ../templates > /dev/null 2>&1
	if [ $? != 0 ]
	then
	    echo "Failed trgen of $i"
            rm -rf Generated
	    continue;
	fi
	dotnet build Generated/*.csproj > /dev/null 2>&1
	if [ $? != 0 ]
	then
	    echo "Failed build of $i"
            rm -rf Generated
	    continue;
	fi
    else
        echo "No pom.xml in $i"
    fi
    rm -rf Generated
done
