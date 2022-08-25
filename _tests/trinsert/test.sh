#!/bin/bash

# "Setting MSYS2_ARG_CONV_EXCL so that Trash XPaths do not get mutulated."
export MSYS2_ARG_CONV_EXCL="*"
where=`dirname -- "$0"`
trparse "$where/Expression.g4" | trinsert '//lexerRuleSpec/TOKEN_REF[text()="INT"]' 'fragment ' | trsponge -c -o "$where/Generated"
diff -r "$where/Gold" "$where/Generated"
if [ "$?" != "0" ]
then
	echo Test failed.
	exit 1
else
	echo Test succeeded.
fi
