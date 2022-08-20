#!/bin/bash

# "Setting MSYS2_ARG_CONV_EXCL so that Trash XPaths do not get mutulated."
oexport MSYS2_ARG_CONV_EXCL="*"
where=`dirname -- "$0"`
trparse "$where/Expression.g4" | trdelete '//parserRuleSpec[RULE_REF/text()="a"]' | trsponge -c -o "$where/Generated"
diff -r "$where/Gold" "$where/Generated"
if [ "$?" != "0" ]
then
	echo Test failed.
	exit 1
else
	echo Test succeeded.
fi
