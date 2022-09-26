#!/bin/bash
is_grammar=`trparse $1 -t antlr4 | trxgrep ' /grammarSpec/grammarDecl[not(grammarType/LEXER)]' | trtext -c`
if [ "$is_grammar" != "1" ]
then
	echo $1 is not a combined or parser Antlr4 grammar.
	exit
fi
names=`trparse $1 | trxgrep ' //parserRuleSpec/RULE_REF/@Text' | sort -u`
for i in $names
do
	fixed_name=`echo -n $i | tr -d '\n' | tr -d '\r'`
	count=`trparse $1 | trxgrep ' count(//parserRuleSpec/ruleBlock//atom/ruleref/RULE_REF[text()="'$fixed_name'"])'`
	if [ "$count" == "0" ]
	then
		dirname=`dirname $1`
		if [ ! -f "$dirname/pom.xml" ]
		then
			echo file is $1
			echo $fixed_name not used.
		else
			entry=`trxml2 $dirname/pom.xml | grep -i entryPoint | tr -d '\n' | tr -d '\r'`
			entry=`echo -n $entry | sed 's/.*=//'`
			if [ "$entry" != "$fixed_name" ]
			then
				echo file is $1
				echo $fixed_name not used.
			fi
		fi
	fi
done
