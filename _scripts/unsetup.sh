#!/usr/bin/bash
cd src
tools=`dotnet nuget list source | grep nuget-tr | awk '{print $2}'`
echo $tools
for i in $tools
do
	dotnet nuget remove source $i > /dev/null 2>&1
done
tools=`dotnet nuget list source | grep trtool- | awk '{print $2}'`
echo $tools
for i in $tools
do
	dotnet nuget remove source $i > /dev/null 2>&1
done
dotnet nuget remove source nuget-Docs > /dev/null 2>&1
dotnet nuget remove source nuget-AntlrJson > /dev/null 2>&1
dotnet nuget list source
