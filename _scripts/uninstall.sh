#!/bin/bash

for i in tranalyze trcombine trconvert trdelabel trdelete trfold trfoldlit trgen trgroup trjson trkleene trparse trprint trrename trsplit trsponge trst trstrip trtext trtokens trtree trunfold trungroup trwdog trxgrep trxml trxml2
do
	echo $i
	dotnet tool uninstall -g $i
done
