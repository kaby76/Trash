#!/bin/bash

for i in tranalyze trcombine trconvert trdelete trfold trfoldlit trgen trgroup trjson trkleene trparse trprint trrename trst trstrip trtext trtokens trtree trunfold trungroup trwdog trxgrep trxml trxml2
do
    echo $i
    dotnet nuget remove source nuget-$i
done
dotnet nuget list source
