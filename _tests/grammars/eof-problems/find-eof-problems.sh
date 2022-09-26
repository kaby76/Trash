#!/usr/bin/bash

# "Setting MSYS2_ARG_CONV_EXCL so that Trash XPaths do not get mutulated."
export MSYS2_ARG_CONV_EXCL="*"
# Sanity check.
is_grammar=`trparse $1 -t antlr4 | trxgrep '/grammarSpec/grammarDecl[not(grammarType/LEXER)]' | trtext -c`
if [ "$is_grammar" != "1" ]
then
 echo $1 is not a combined or parser Antlr4 grammar.
 exit 1
fi
count=`trparse $1 -t antlr4 2> /dev/null \
 | trxgrep '//parserRuleSpec//alternative/element[.//TOKEN_REF/text()="EOF"]/following-sibling::element' \
 | trtext -c`
if [ "$count" != "0" ]
then
 echo $1 has an EOF usage followed by another element.
fi
count=`trparse $1 -t antlr4 2> /dev/null \
 | trxgrep '//labeledAlt[.//TOKEN_REF/text()="EOF" and count(../labeledAlt) > 1]' \
 | trtext -c`
if [ "$count" != "0" ]
then
 echo $1 has an EOF in one alt, but not in another.
fi
