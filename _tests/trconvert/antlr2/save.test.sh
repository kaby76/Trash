#!/bin/sh

for i in *.g2
do
	echo $i
	extension="${i##*.}"
	filename="${i%.*}"
	trparse $i -t antlr2 | trconvert | trsponge -c true
done
