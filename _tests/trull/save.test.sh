#!/bin/bash

echo "Setting MSYS2_ARG_CONV_EXCL so that Trash XPaths do not get mutulated."
export MSYS2_ARG_CONV_EXCL="*"

trparse KeywordFun.g4 | trull "//lexerRuleSpec[TOKEN_REF/text() = 'A' or TOKEN_REF/text() = 'B']//STRING_LITERAL" | \
	trsponge -c -o Gold
