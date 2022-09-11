#!/usr/bin/bash

for i in `find . -name pom.xml | grep -v _grammar-test | grep -v generated | grep -v target`
do
	base=`dirname $i`
	echo $i
	grep -qs . $base/*.g4
	if [ "$?" = "0" ]
	then
		trparse $base/*.g4 | trxgrep ' //parserRuleSpec//TOKEN_REF[text()=doc("*")//lexerRuleSpec[./lexerRuleBlock/lexerAltList/lexerAlt/lexerCommands]/TOKEN_REF/text()]' | trtext
	fi
done
