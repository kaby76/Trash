#!/bin/sh

export MSYS2_ARG_CONV_EXCL="*"
where=`dirname -- "$0"`
for i in "$where/*.g2"
do
	echo $i
	extension="${i##*.}"
	filename="${i%.*}"
	dotnet trparse -- $i -t antlr2 | dotnet trconvert | dotnet trsponge -- -c -o "$where/Generated"
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
