#!/bin/sh

export MSYS2_ARG_CONV_EXCL="*"
where=`dirname -- "$0"`
for i in "$where/*.g2"; do dos2unix.exe $i; done
for i in "$where/*.g2"
do
	echo $i
	extension="${i##*.}"
	filename="${i%.*}"
	trparse $i -t antlr2 | trconvert | trsponge -c -o "$where/Generated"
done
for i in "$where/Generated/*.g4"; do dos2unix.exe $i; done
for i in "$where/Gold/*.g4"; do dos2unix.exe $i; done
rm -f "$where"/Generated/*.txt2
diff -r "$where/Gold" "$where/Generated"
if [ "$?" != "0" ]
then
	echo Test failed.
	exit 1
else
	echo Test succeeded.
fi
