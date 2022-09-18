#!/usr/bin/bash
export MSYS2_ARG_CONV_EXCL="*"
rm -rf Generated
trparse Expression.g4 | trrename -r 'e,exp;a,atom;INT,Int;MUL,OpMul;DIV,OpDiv;ADD,OpAdd;SUB,OpSub' | trsponge -c -o Generated
diff -r Gold Generated
if [ "$?" != "0" ]
then
	echo Test failed.
	exit 1
else
	echo Test succeeded.
fi
