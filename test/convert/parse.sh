#!/bin/sh

for i in *.g2
do
	echo $i
	trparse -f $i -t antlr2 | trprint > o
	diff o $i
done
for i in *.g2
do
	echo $i
	extension="${i##*.}"
	filename="${i%.*}"
	trparse -f $i -t antlr2 | trconvert | trprint > "$filename.g4"
done
