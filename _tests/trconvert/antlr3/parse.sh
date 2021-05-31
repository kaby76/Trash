#!/bin/sh

for i in *.g3
do
	echo $i
	trparse $i -t antlr3 | trprint > o
	diff o $i
done
for i in *.g3
do
	echo $i
	extension="${i##*.}"
	filename="${i%.*}"
	trparse $i -t antlr3 | trconvert | trprint > "$filename.g4"
done
