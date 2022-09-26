#!/usr/bin/bash
echo -n $1 ": "
type=""
trparse -e -t antlr2 $1 > /dev/null 2>&1
if [ "$?" == "0" ]
then
	type="$type antlr2"
fi
trparse -e -t antlr3 $1 > /dev/null 2>&1
if [ "$?" == "0" ]
then
	type="$type antlr3"
fi
trparse -e -t antlr4 $1 > /dev/null 2>&1
if [ "$?" == "0" ]
then
	type="$type antlr4"
	l=`trparse $1 -t antlr4 | trxgrep '/grammarSpec/grammarDecl/grammarType/LEXER' | trtext -c`
	if [ "$l" != "0" ]
	then
		type="$type lexer"
	fi
	p=`trparse $1 -t antlr4 | trxgrep '/grammarSpec/grammarDecl/grammarType/PARSER' | trtext -c`
	if [ "$p" != "0" ]
	then
		type="$type parser"
	fi
	if [ "$l" = "0" ] && [ "$p" = "0" ]
	then
		type="$type combined"
	fi
fi

echo $type

