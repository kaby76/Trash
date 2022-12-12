#!/usr/bin/bash
cd src
tools=`dotnet nuget list source | grep trtool | awk '{print $2}'`
echo $tools
for i in $tools
do
	dotnet nuget remove source $i > /dev/null 2>&1
done
dotnet nuget list source
