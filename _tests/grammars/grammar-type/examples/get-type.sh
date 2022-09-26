#!/usr/bin/bash

type=""
trparse -e -t antlr2 $1
echo $?
exit
if [ "$?" == "0" ]
then
echo 2
	type="$type antlr2"
fi
trparse -e -t antlr3 $1
if [ "$?" == "0" ]
then
echo 3
	type="$type antlr3"
fi
trparse -e -t antlr4 $1
if [ "$?" == "0" ]
then
echo 4
	type="$type antlr4"
fi
exit 0
if [ "$?" == "0" ]
then
	type="$type antlr4"
	c=`trparse $1 -t antlr4 | trxgrep '/grammarSpec/grammarDecl/grammarType/LEXER' | trtext -c`
	if [ "$c" != "0" ]
	then
		type="$type lexer"
	fi
	c=`trparse $1 -t antlr4 | trxgrep '/grammarSpec/grammarDecl/grammarType/PARSER' | trtext -c`
	if [ "$c" != "0" ]
	then
		type="$type parser"
	fi
fi

echo $type

