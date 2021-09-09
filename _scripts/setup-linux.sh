#!/bin/bash

unameOut="$(uname -s)"
case "${unameOut}" in
    Linux*)     machine=Linux;;
    Darwin*)    machine=Mac;;
    CYGWIN*)    machine=Cygwin;;
    MINGW*)     machine=MinGw;;
    *)          machine="UNKNOWN:${unameOut}"
esac
    cwd=`pwd`
echo $cwd
for i in tranalyze trcombine trconvert trdelabel trdelete trdot trfold trfoldlit trformat trgen trgroup trinsert trjson trkleene trmvsr trparse trprint trrename trrr trrup trsplit trsponge trst trstrip trtext trthompson trtokens trtree trunfold trungroup trwdog trxgrep trxml trxml2
do
    echo $i
    dotnet nuget add source $cwd/$i/bin/Debug/ --name nuget-$i
done
dotnet nuget list source
