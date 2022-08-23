#!/bin/bash
export MSYS2_ARG_CONV_EXCL="*"
count=`trparse $1 | trxgrep '//(altList | labeledAlt)/alternative/element[ebnf[not(child::blockSuffix)]/block/altList[not(@ChildCount > 1)]] | //(altList | labeledAlt)[not(@ChildCount > 1)]/alternative[not(@ChildCount > 1)]/element[ebnf[not(child::blockSuffix)]/block/altList[@ChildCount > 1]]' | trtext -c`
if [ $count -gt 0 ]
then
	echo $1 has $count useless parentheses.
	trparse $1 | trxgrep 'for $i in //(altList | labeledAlt)/alternative/element[ebnf[not(child::blockSuffix)]/block/altList[not(@ChildCount > 1)]] | //(altList | labeledAlt)[not(@ChildCount > 1)]/alternative[not(@ChildCount > 1)]/element[ebnf[not(child::blockSuffix)]/block/altList[@ChildCount > 1]] return concat("line ", $i/@Line, " col ", $i/@Column, " """, $i/@Text,"""")'
#else
#	echo No useless parentheses.
fi
