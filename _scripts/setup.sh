#!/bin/bash

unameOut="$(uname -s)"
case "${unameOut}" in
    Linux*)     machine=Linux;;
    Darwin*)    machine=Mac;;
    CYGWIN*)    machine=Cygwin;;
    MINGW*)     machine=MinGw;;
    *)          machine="UNKNOWN:${unameOut}"
esac
if [[ "$machine" != "MinGW" ]]
then
    cwd=`pwd | sed 's%/c%c:%' | sed 's%/%\\\\%g'`
else
    cwd=`pwd`
fi
for i in tranalyze trcombine trconvert trdelabel trdelete trfold trfoldlit trgen trgroup trjson trkleene trparse trprint trrename trsplit trsponge trst trstrip trtext trtokens trtree trunfold trungroup trwdog trxgrep trxml trxml2
do
    echo $i
    dotnet nuget add source $cwd/$i/bin/Debug/ --name nuget-$i
done
dotnet nuget list source
