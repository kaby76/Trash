#!/bin/bash
is_grammar=`trparse $1 -t antlr4 | trxgrep ' /grammarSpec/grammarDecl[not(grammarType/PARSER)]' | trtext -c`
if [ "$is_grammar" != "1" ]
then
	echo $1 is not a combined or lexer Antlr4 grammar.
	exit
fi
trparse $1 | trxgrep ' //lexerRuleSpec[not(FRAGMENT) and TOKEN_REF/@Text=//lexerRuleBlock//TOKEN_REF/text()]' | trtext
