#!/bin/sh

export MSYS2_ARG_CONV_EXCL="*"
where=`dirname -- "$0"`
cd "$where"
for i in "*.g4"
do
	echo $i
	extension="${i##*.}"
	filename="${i%.*}"
	trparse $i | trdelabel | trsponge -c -o "Generated"
done
rm -f Generated/*.txt2
diff -r "Gold" "Generated"
if [ "$?" != "0" ]
then
	echo Test failed.
	exit 1
else
	echo Test succeeded.
fi
