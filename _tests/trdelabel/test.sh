#!/bin/sh

export MSYS2_ARG_CONV_EXCL="*"
where=`dirname -- "$0"`
for i in "$where/*.g4"
do
	echo $i
	extension="${i##*.}"
	filename="${i%.*}"
	trparse $i | trdelabel | trsponge -c -o "$where/Generated"
done
rm -f "$where"/Generated/*.txt2
diff -r "$where/Gold" "$where/Generated"
if [ "$?" != "0" ]
then
	echo Test failed.
	exit 1
else
	echo Test succeeded.
fi
