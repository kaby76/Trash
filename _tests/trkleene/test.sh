#!/bin/bash

# "Setting MSYS2_ARG_CONV_EXCL so that Trash XPaths do not get mutulated."
export MSYS2_ARG_CONV_EXCL="*"
where=`dirname -- "$0"`
cd "$where"
rm -rf Generated
trparse t1.g4 | trkleene | trsponge -c -o "$where/Generated"
trparse t2.g4 | trkleene | trsponge -c -o "$where/Generated"
trparse t3.g4 | trkleene | trsponge -c -o "$where/Generated"
trparse t4.g4 | trkleene | trsponge -c -o "$where/Generated"
trparse t5.g4 | trkleene | trsponge -c -o "$where/Generated"
trparse PostgreSQLParser.g4 PostgreSQLLexer.g4 | trkleene | trsponge -c -o "$where/Generated"
for i in "$where/Generated/*"
do
	dos2unix $i
done
for i in "$where/Gold/*"
do
	dos2unix $i
done
diff -r "$where/Gold" "$where/Generated"
if [ "$?" != "0" ]
then
	echo Test failed.
	exit 1
else
	echo Test succeeded.
fi
