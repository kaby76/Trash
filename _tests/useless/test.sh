#!/bin/bash

# "Setting MSYS2_ARG_CONV_EXCL so that Trash XPaths do not get mutulated."
export MSYS2_ARG_CONV_EXCL="*"
where=`dirname -- "$0"`
rm -rf "$where/Generated"
mkdir "$where/Generated"
bash "$where/find-useless.sh" "$where/g1.g4" > "$where/Generated/g1.out"
bash "$where/find-useless.sh" "$where/g2.g4" > "$where/Generated/g2.out"
diff -r "$where/Gold" "$where/Generated"
if [ "$?" != "0" ]
then
	echo Test failed.
	exit 1
else
	echo Test succeeded.
fi
