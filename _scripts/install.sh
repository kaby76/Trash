#!/bin/bash

version=""
#version="--version 0.8.1"

for i in tranalyze trconvert trdelete trfold trfoldlit trgen trgroup trjson trkleene trparse trprint trrename trst trstrip trtext trtokens trtree trunfold trungroup trwdog trxgrep trxml trxml2
do
	echo $i
	dotnet tool install -g $i $version
done
