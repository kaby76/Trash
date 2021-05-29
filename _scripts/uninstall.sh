#!/bin/bash

for i in tranalyze trconvert trfold trfoldlit trgen trgroup trjson trkleene trparse trprint trrename trst trstrip trtext trtokens trtree trunfold trungroup trwdog trxgrep trxml trxml2
do
	echo $i
	if [[ ! -d $i ]]
	then
		continue
	fi
	dotnet tool uninstall -g $i
done
