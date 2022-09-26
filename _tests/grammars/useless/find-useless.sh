#!/bin/bash
export MSYS2_ARG_CONV_EXCL="*"
count=`trparse $1 | trxgrep -e 'for $i in (//(altList | labeledAlt)/alternative/element[ebnf[not(child::blockSuffix)]/block/altList[not(@ChildCount > 1)]] | //(altList | labeledAlt)[not(@ChildCount > 1)]/alternative[not(@ChildCount > 1)]/element[ebnf[not(child::blockSuffix)]/block/altList[@ChildCount > 1]] | //ebnf/block[altList[@ChildCount = 1]/alternative[@ChildCount = 1]/element/atom]) return concat("line ", $i/@Line, " col ", $i/@Column, " """, $i/@Text,"""")'`
if [ "$count" != "0" ]
then
	trparse $1 | trxgrep -e 'for $i in (//(altList | labeledAlt)/alternative/element[ebnf[not(child::blockSuffix)]/block/altList[not(@ChildCount > 1)]] | //(altList | labeledAlt)[not(@ChildCount > 1)]/alternative[not(@ChildCount > 1)]/element[ebnf[not(child::blockSuffix)]/block/altList[@ChildCount > 1]] | //ebnf/block[altList[@ChildCount = 1]/alternative[@ChildCount = 1]/element/atom]) return concat("line ", $i/@Line, " col ", $i/@Column, " """, $i/@Text,"""")' | sort -u
else
	echo No useless parentheses.
fi
