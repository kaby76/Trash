#!/bin/bash

# "Setting MSYS2_ARG_CONV_EXCL so that Trash XPaths do not get mutulated."
export MSYS2_ARG_CONV_EXCL="*"
where=`dirname -- "$0"`
trparse "$where/Arithmetic.g4" | trsplit | trsponge -c -o "$where/Generated"
trcombine "$where/Generated/ArithmeticLexer.g4" "$where/Generated/ArithmeticParser.g4" | trprint > "$where/Generated/Arithmetic.g4"
diff "$where/Gold/Arithmetic.g4" "$where/Generated/Arithmetic.g4"
if [ "$?" != "0" ]
then
	echo Test failed.
	exit 1
else
	echo Test succeeded.
fi
